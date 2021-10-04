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
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Services
{
    public class DebugDiagnosisKeyRegisterServer : IDiagnosisKeyRegisterServer
    {
        private const string FORMAT_SYMPTOM_ONSET_DATE = "yyyy-MM-dd'T'HH:mm:ss.fffzzz";

        // https://github.com/keiji/en-calibration-server
        private const string API_ENDPOINT = "https://en.keiji.dev/diagnosis_keys";
        private const string CLUSTER_ID = "212458"; // 6 digits

        private readonly ILoggerService _loggerService;

        private readonly HttpClient _httpClient;

        public DebugDiagnosisKeyRegisterServer(
            ILoggerService loggerService,
            IHttpClientService httpClientService
            )
        {
            _loggerService = loggerService;
            _httpClient = httpClientService.Create();
        }

        public async Task<HttpStatusCode> SubmitDiagnosisKeysAsync(
            DateTime symptomOnsetDate,
            IList<TemporaryExposureKey> temporaryExposureKeys,
            string _,
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
                RequestDiagnosisKey request = new RequestDiagnosisKey(
                    symptomOnsetDate.ToString(FORMAT_SYMPTOM_ONSET_DATE),
                    temporaryExposureKeys,
                    idempotencyKey,
                    ReportType.ConfirmedClinicalDiagnosis
                    );
                string requestJson = JsonConvert.SerializeObject(request);

                StringContent httpContent = new StringContent(requestJson);

                Uri uri = new Uri($"{API_ENDPOINT}/{CLUSTER_ID}/{Guid.NewGuid()}.json");
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

    [JsonObject]
    public class RequestDiagnosisKey
    {
        [JsonProperty("symptomOnsetDate")]
        public string SymptomOnsetDate { get; set; }

        public IList<Tek> temporaryExposureKeys;

        [JsonProperty("idempotency_key")]
        public string IdempotencyKey { get; set; }

        public RequestDiagnosisKey(
            string symptomOnsetDate,
            IList<TemporaryExposureKey> teks,
            string idempotencyKey,
            ReportType defaultRportType = ReportType.ConfirmedTest
            )
        {
            SymptomOnsetDate = symptomOnsetDate;
            temporaryExposureKeys = teks.Select(tek =>
            {
                return new Tek(tek)
                {
                    reportType = (int)defaultRportType,
                };
            }).ToList();
            IdempotencyKey = idempotencyKey;
        }
    }

    [JsonObject]
    public class Tek
    {
        public readonly string key;
        public readonly long rollingStartNumber;
        public readonly long rollingPeriod;
        public int reportType;

        public Tek(TemporaryExposureKey tek)
        {
            key = Convert.ToBase64String(tek.KeyData);
            rollingStartNumber = tek.RollingStartIntervalNumber;
            rollingPeriod = tek.RollingPeriod;
            reportType = (int)ReportType.ConfirmedClinicalDiagnosis;
        }
    }
}
