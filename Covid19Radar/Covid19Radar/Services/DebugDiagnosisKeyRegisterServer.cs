/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Services
{
    public class DebugDiagnosisKeyRegisterServer : IDiagnosisKeyRegisterServer
    {
        private const string FORMAT_SYMPTOM_ONSET_DATE = "yyyy-MM-dd'T'HH:mm:ss.fffzzz";

        private readonly ILoggerService _loggerService;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;

        private readonly string _platform;
        private readonly HttpClient _httpClient;

        public DebugDiagnosisKeyRegisterServer(
            ILoggerService loggerService,
            IServerConfigurationRepository serverConfigurationRepository,
            IHttpClientService httpClientService,
            IEssentialsService essentialsService
            )
        {
            _loggerService = loggerService;
            _serverConfigurationRepository = serverConfigurationRepository;
            _httpClient = httpClientService.Create();
            _platform = essentialsService.Platform;
        }

        public async Task<HttpStatusCode> SubmitDiagnosisKeysAsync(
            DateTime symptomOnsetDate,
            IList<TemporaryExposureKey> temporaryExposureKeys,
            string _,
            string[] regions,
            string[] subRegions,
            string idempotencyKey
            )
        {
#if DEBUG
            _loggerService.StartMethod();
            _loggerService.Warning("ChinoDiagnosisKeyRegisterServer is only support DEBUG build.");
#else
            _loggerService.Error("ChinoDiagnosisKeyRegisterServer is not support RELEASE build.");
            throw new NotSupportedException("ChinoDiagnosisKeyRegisterServer is not support RELEASE build.");
#endif
            try
            {
                await _serverConfigurationRepository.LoadAsync();

                var keys = temporaryExposureKeys.Select(key => new DiagnosisSubmissionParameter.Key()
                {
                    KeyData = Convert.ToBase64String(key.KeyData),
                    RollingStartNumber = (uint)key.RollingStartIntervalNumber,
                    RollingPeriod = (uint)key.RollingPeriod,
                    ReportType = (uint)key.ReportType
                }).ToArray();

                DiagnosisSubmissionParameter parameter = new DiagnosisSubmissionParameter()
                {
                    SymptomOnsetDate = symptomOnsetDate.ToString(FORMAT_SYMPTOM_ONSET_DATE),
                    Keys = keys,
                    Regions = regions,
                    SubRegions = subRegions,
                    Platform = _platform,
                    IdempotencyKey = idempotencyKey
                };

                return await SubmitDiagnosisKeysAsync(parameter, _serverConfigurationRepository.DiagnosisKeyRegisterApiUrl);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        private async Task<HttpStatusCode> SubmitDiagnosisKeysAsync(
            DiagnosisSubmissionParameter parameter,
            string diagnosisKeyRegisterApiEndpoint
        )
        {
            _loggerService.StartMethod();

            try
            {
                string requestJson = JsonConvert.SerializeObject(parameter);
                StringContent httpContent = new StringContent(requestJson);

                _loggerService.Debug($"diagnosisKeyRegisterApiEndpoint: {diagnosisKeyRegisterApiEndpoint}");
                Uri uri = new Uri(diagnosisKeyRegisterApiEndpoint);

                HttpResponseMessage response = await _httpClient.PutAsync(uri, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                }
                return response.StatusCode;
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }
}
