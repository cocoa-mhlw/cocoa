/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Covid19Radar.Services
{
    public class DiagnosisKeyRegisterServer : IDiagnosisKeyRegisterServer
    {
        private readonly ILoggerService _loggerService;
        private readonly IHttpClientService _httpClientService;
        private readonly IDeviceVerifier _deviceVerifier;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;

        public DiagnosisKeyRegisterServer(
            ILoggerService loggerService,
            IHttpClientService httpClientService,
            IDeviceVerifier deviceVerifier,
            IServerConfigurationRepository serverConfigurationRepository
            )
        {
            _loggerService = loggerService;
            _httpClientService = httpClientService;
            _deviceVerifier = deviceVerifier;
            _serverConfigurationRepository = serverConfigurationRepository;
        }

        public async Task<HttpStatusCode> SubmitDiagnosisKeysAsync(
            bool hasSymptom,
            DateTime symptomOnsetDate,
            IList<TemporaryExposureKey> temporaryExposureKeys,
            string processNumber,
            string idempotencyKey
            )
        {
            try
            {
                _loggerService.StartMethod();

                if (string.IsNullOrEmpty(processNumber))
                {
                    _loggerService.Error($"Process number is null or empty.");
                    throw new InvalidOperationException();
                }

                _loggerService.Info($"TemporaryExposureKey count: {temporaryExposureKeys.Count()}");
                foreach (var tek in temporaryExposureKeys)
                {
                    _loggerService.Info(
                        $"TemporaryExposureKey: " +
                        $"RollingStartIntervalNumber: {tek.RollingStartIntervalNumber}({tek.GetRollingStartIntervalNumberAsUnixTimeInSec()}), " +
                        $"RollingPeriod: {tek.RollingPeriod}, " +
                        $"RiskLevel: {tek.RiskLevel}"
                        );
                }

                var diagnosisInfo = await CreateSubmissionAsync(
                    hasSymptom,
                    symptomOnsetDate,
                    temporaryExposureKeys,
                    processNumber,
                    idempotencyKey
                    );
                return await PutSelfExposureKeysAsync(diagnosisInfo);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public async Task<HttpStatusCode> PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request)
        {
            _loggerService.StartMethod();

            try
            {
                await _serverConfigurationRepository.LoadAsync();

                var diagnosisKeyRegisterApiUrls = _serverConfigurationRepository.DiagnosisKeyRegisterApiUrls;
                if (diagnosisKeyRegisterApiUrls.Count() == 0)
                {
                    _loggerService.Error("DiagnosisKeyRegisterApiUrls count 0");
                    throw new InvalidOperationException("DiagnosisKeyRegisterApiUrls count 0");
                }
                else if (diagnosisKeyRegisterApiUrls.Count() > 1)
                {
                    _loggerService.Warning("Multi DiagnosisKeyRegisterApiUrl are detected.");
                }

                var url = diagnosisKeyRegisterApiUrls.First();
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                return await PutAsync(url, content);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        private async Task<HttpStatusCode> PutAsync(string url, HttpContent body)
        {
            var result = await _httpClientService.ApiClient.PutAsync(url, body);
            await result.Content.ReadAsStringAsync();
            return result.StatusCode;
        }

        private async Task<DiagnosisSubmissionParameter> CreateSubmissionAsync(
            bool hasSymptom,
            DateTime symptomOnsetDate,
            IList<TemporaryExposureKey> temporaryExposureKeys,
            string processNumber,
            string idempotencyKey
            )
        {
            _loggerService.StartMethod();

            // Create the network keys
            var keys = temporaryExposureKeys.Select(k => new DiagnosisSubmissionParameter.Key
            {
                KeyData = Convert.ToBase64String(k.KeyData),
                RollingStartNumber = (uint)k.RollingStartIntervalNumber,
                RollingPeriod = (uint)k.RollingPeriod,
                ReportType = (uint)k.ReportType,
            });

            // Generate Padding
            var padding = GetPadding();

            // Create the submission
            var submission = new DiagnosisSubmissionParameter()
            {
                HasSymptom = hasSymptom,
                OnsetOfSymptomOrTestDate = symptomOnsetDate.ToString(AppConstants.FORMAT_TIMESTAMP),
                Keys = keys.ToArray(),
                Regions = AppSettings.Instance.SupportedRegions,
                Platform = DeviceInfo.Platform.ToString().ToLowerInvariant(),
                DeviceVerificationPayload = null,
                AppPackageName = AppInfo.PackageName,
                VerificationPayload = processNumber,
                IdempotencyKey = idempotencyKey,
                Padding = padding
            };

            // Create device verification payload
            var tries = 0;
            var delay = 4 * 1000;
            while (true) {
                var deviceVerificationPayload = await _deviceVerifier.VerifyAsync(submission);
                if (!_deviceVerifier.IsErrorPayload(deviceVerificationPayload))
                {
                    _loggerService.Info("Payload creation successful.");
                    submission.DeviceVerificationPayload = deviceVerificationPayload;
                    break;
                }
                else if (tries >= 3)
                {
                    _loggerService.Error("Payload creation failed all.");
                    throw new DiagnosisKeyRegisterException(DiagnosisKeyRegisterException.Codes.FailedCreatePayload);
                }

                _loggerService.Warning($"Payload creation failed. {tries + 1} time(s).");
                _loggerService.Info($"delay {delay} msec");
                await Task.Delay(delay);
                delay *= 2;

                tries++;
            }

            _loggerService.Info($"DeviceVerificationPayload is {(string.IsNullOrEmpty(submission.DeviceVerificationPayload) ? "null or empty" : "set")}.");
            _loggerService.Info($"VerificationPayload is {(string.IsNullOrEmpty(submission.VerificationPayload) ? "null or empty" : "set")}.");

            _loggerService.EndMethod();

            return submission;
        }

        private static string GetPadding(Random random = null)
        {
            if (random is null)
            {
                random = new Random();
            }

            var size = random.Next(1024, 2048);

            // Approximate the base64 blowup.
            size = (int)(size * 0.75);

            var padding = new byte[size];
            random.NextBytes(padding);
            return Convert.ToBase64String(padding);
        }
    }

    public class DiagnosisKeyRegisterServerMock : IDiagnosisKeyRegisterServer
    {
        public Task<HttpStatusCode> SubmitDiagnosisKeysAsync(
            bool hasSymptom,
            DateTime symptomOnsetDate,
            IList<TemporaryExposureKey> temporaryExposureKeys,
            string processNumber,
            string idempotencyKey
            )
            => Task.FromResult(HttpStatusCode.NoContent);

        public Task<HttpStatusCode> PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request)
            => Task.FromResult(HttpStatusCode.NoContent);
    }

    public class DiagnosisKeyRegisterException : Exception
    {
        private const string DataKeyCode = "code";

        public enum Codes {
            FailedCreatePayload,
        }

        public Codes Code => (Codes)Data[DataKeyCode];

        public DiagnosisKeyRegisterException(Codes code) : base("Failed to register the diagnostic key.")
        {
            Data[DataKeyCode] = code;
        }
    }
}
