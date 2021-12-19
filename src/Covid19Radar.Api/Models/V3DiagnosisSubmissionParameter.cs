/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Common;
using Covid19Radar.Api.Extensions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;

namespace Covid19Radar.Api.Models
{
    public class V3DiagnosisSubmissionParameter : IPayload, IDeviceVerification
    {
        public const string FORMAT_SYMPTOM_ONSET_DATE = "yyyy-MM-dd'T'HH:mm:ss.fffzzz";

        // RFC3339
        // e.g. 2021-09-20T23:52:57.436+00:00
        [JsonProperty("symptomOnsetDate")]
        public string SymptomOnsetDate { get; set; }

        [JsonProperty("keys")]
        public Key[] Keys { get; set; }

        [JsonProperty("regions")]
        public string[] Regions { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("deviceVerificationPayload")]
        public string DeviceVerificationPayload { get; set; }

        [JsonProperty("appPackageName")]
        public string AppPackageName { get; set; }

        // Some signature / code confirming authorization by the verification authority.
        [JsonProperty("verificationPayload")]
        public string VerificationPayload { get; set; }

        [JsonProperty("idempotency_key")]
        public string IdempotencyKey { get; set; }

        // Random data to obscure the size of the request network packet sniffers.
        [JsonProperty("padding")]
        public string Padding { get; set; }

        [JsonIgnore]
        public string KeysTextForDeviceVerification
            => string.Join(",", Keys.OrderBy(k => k.KeyData).Select(k => k.GetKeyString()));

        public class Key
        {
            [JsonProperty("keyData")]
            public string KeyData { get; set; }

            [JsonProperty("rollingStartNumber")]
            public uint RollingStartNumber { get; set; }

            [JsonProperty("rollingPeriod")]
            public uint RollingPeriod { get; set; }

            [JsonProperty("transmissionRisk")]
            public int TransmissionRisk { get; set; }

            [JsonProperty("reportType")]
            public uint ReportType { get; set; }

            [JsonProperty("daysSinceOnsetOfSymptoms")]
            public int DaysSinceOnsetOfSymptoms { get; set; }

            public DateTime GetDate()
                => DateTimeOffset.FromUnixTimeSeconds(RollingStartNumber * TemporaryExposureKeyModel.TIME_WINDOW_IN_SEC).Date;

            public TemporaryExposureKeyModel ToModel()
            {
                var keyData = Convert.FromBase64String(KeyData);

                return new TemporaryExposureKeyModel()
                {
                    KeyData = keyData,
                    RollingPeriod = ((int)RollingPeriod == 0 ? (int)Constants.ActiveRollingPeriod : (int)RollingPeriod),
                    RollingStartIntervalNumber = (int)RollingStartNumber,
                    TransmissionRiskLevel = TransmissionRisk,
                    ReportType = (int)ReportType,
                    DaysSinceOnsetOfSymptoms = DaysSinceOnsetOfSymptoms,
                    Exported = false
                };
            }
            /// <summary>
            /// Validation
            /// </summary>
            /// <returns>true if valid</returns>
            public bool IsValid()
            {
                if (string.IsNullOrWhiteSpace(KeyData)) return false;
                if (RollingPeriod > Constants.ActiveRollingPeriod) return false;

                var dateTime = DateTime.UtcNow.Date;
                var todayRollingStartNumber = dateTime.ToRollingStartNumber();

                var oldestRollingStartNumber = dateTime.AddDays(Constants.OutOfDateDays).ToRollingStartNumber();
                if (RollingStartNumber < oldestRollingStartNumber || RollingStartNumber > todayRollingStartNumber) return false;
                return true;
            }

            public string GetKeyString() => string.Join(".", KeyData, RollingStartNumber, RollingPeriod, ReportType);
        }

        /// <summary>
        /// Validation
        /// </summary>
        /// <returns>true if valid</returns>
        public virtual bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(VerificationPayload)) return false;
            if ((Regions?.Length ?? 0) == 0) return false;
            if (string.IsNullOrWhiteSpace(Platform)) return false;
            if (string.IsNullOrWhiteSpace(DeviceVerificationPayload)) return false;
            if (string.IsNullOrWhiteSpace(AppPackageName)) return false;
            return true;
        }

        public void SetDaysSinceOnsetOfSymptoms()
        {
            var symptomOnsetDate = DateTime.ParseExact(SymptomOnsetDate, FORMAT_SYMPTOM_ONSET_DATE, null).ToUniversalTime().Date;
            foreach (var key in Keys)
            {
                var dateOffset = key.GetDate() - symptomOnsetDate;
                key.DaysSinceOnsetOfSymptoms = dateOffset.Days;
            }
        }
    }
}
