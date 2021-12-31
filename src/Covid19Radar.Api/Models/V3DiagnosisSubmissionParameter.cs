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
        /*
         * [Important]
         * The value `daysSinceOnsetOfSymptoms` must be less than or equal to `+14` and greater than or equal to `-14`.
         *
         * If any diagnosis-keys file CONTAMINATED by out of range value(e.g. -199, 62) that provide detectExposure/provideDiagnosisKeys method,
         * ExposureNotification API for Android doesn't return any result(ExposureDetected/ExposureNotDetected) to BroadcastReceiver.
         */
        private const int MIN_DAYS_SINCE_ONSET_OF_SYMPTOMS = -14;
        private const int MAX_DAYS_SINCE_ONSET_OF_SYMPTOMS = 14;

        // RFC3339
        // e.g. 2021-09-20T23:52:57.436+00:00
        [JsonProperty("symptomOnsetDate")]
        public string SymptomOnsetDate { get; set; }

        [JsonProperty("keys")]
        public Key[] Keys { get; set; }

        [JsonProperty("subRegions")]
        public string[] SubRegions { get; set; }

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

        #region Apple Device Check

        [JsonIgnore]
        public string DeviceToken
            => DeviceVerificationPayload;

        [JsonIgnore]
        public string TransactionIdSeed
            => SymptomOnsetDate
                +AppPackageName
                + KeysTextForDeviceVerification
                + IAndroidDeviceVerification.GetRegionString(Regions);

        #endregion

        #region Android SafetyNet Attestation API

        [JsonIgnore]
        public string JwsPayload
            => DeviceVerificationPayload;

        [JsonIgnore]
        public string ClearText
            => string.Join("|", SymptomOnsetDate, AppPackageName, KeysTextForDeviceVerification, IAndroidDeviceVerification.GetRegionString(Regions), VerificationPayload);

        #endregion

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
                if (RollingStartNumber < oldestRollingStartNumber || RollingStartNumber > todayRollingStartNumber)
                {
                    return false;
                }

                if (DaysSinceOnsetOfSymptoms < MIN_DAYS_SINCE_ONSET_OF_SYMPTOMS
                    || DaysSinceOnsetOfSymptoms > MAX_DAYS_SINCE_ONSET_OF_SYMPTOMS)
                {
                    return false;
                }
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
            var symptomOnsetDate = DateTime.ParseExact(SymptomOnsetDate, Constants.FORMAT_TIMESTAMP, null).ToUniversalTime().Date;
            foreach (var key in Keys)
            {
                var dateOffset = key.GetDate() - symptomOnsetDate;
                key.DaysSinceOnsetOfSymptoms = dateOffset.Days;
            }
        }
    }
}
