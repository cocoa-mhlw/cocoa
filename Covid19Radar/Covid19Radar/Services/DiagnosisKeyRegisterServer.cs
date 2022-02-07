﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using Xamarin.Essentials;

namespace Covid19Radar.Services
{
    public class DiagnosisKeyRegisterServer : IDiagnosisKeyRegisterServer
    {
        private readonly ILoggerService _loggerService;
        private readonly IHttpDataService _httpDataService;
        private readonly IDeviceVerifier _deviceVerifier;

        public DiagnosisKeyRegisterServer(
            ILoggerService loggerService,
            IHttpDataService httpDataService,
            IDeviceVerifier deviceVerifier
            )
        {
            _loggerService = loggerService;
            _httpDataService = httpDataService;
            _deviceVerifier = deviceVerifier;
        }

        public async Task<HttpStatusCode> SubmitDiagnosisKeysAsync(
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

                var diagnosisInfo = await CreateSubmissionAsync(symptomOnsetDate, temporaryExposureKeys, processNumber, idempotencyKey);
                return await _httpDataService.PutSelfExposureKeysAsync(diagnosisInfo);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        private async Task<DiagnosisSubmissionParameter> CreateSubmissionAsync(
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
                SymptomOnsetDate = symptomOnsetDate.ToString(AppConstants.FORMAT_TIMESTAMP),
                Keys = keys.ToArray(),
                Regions = AppSettings.Instance.SupportedRegions,
                Platform = DeviceInfo.Platform.ToString().ToLowerInvariant(),
                DeviceVerificationPayload = null,
                AppPackageName = AppInfo.PackageName,
                VerificationPayload = processNumber,
                IdempotencyKey = idempotencyKey,
                Padding = padding
            };

            submission.DeviceVerificationPayload = await _deviceVerifier.VerifyAsync(submission);

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
}
