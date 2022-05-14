/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Common;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Covid19Radar.Api.Models
{
    public class V3DiagnosisSubmissionParameter : IPayload, IDeviceVerification
    {
        [JsonProperty("hasSymptom")]
        public bool HasSymptom { get; set; }

        // RFC3339
        // e.g. 2021-09-20T23:52:57.436+00:00
        [JsonProperty("onsetOfSymptomOrTestDate")]
        public string OnsetOfSymptomOrTestDate { get; set; }

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
        public virtual string KeysTextForDeviceVerification
        {
            get
            {
                if (Keys is null)
                {
                    return string.Empty;
                }
                return string.Join(",", Keys.OrderBy(k => k.KeyData).Select(k => k.GetKeyString()));
            }
        }

        #region Apple Device Check

        [JsonIgnore]
        public string DeviceToken
            => DeviceVerificationPayload;

        [JsonIgnore]
        public string TransactionIdSeed
        {
            get
            {
                var hasSymptom = HasSymptom ? "HasSymptom" : "NoSymptom";
                return
                    AppPackageName
                    + OnsetOfSymptomOrTestDate
                    + hasSymptom
                    + KeysTextForDeviceVerification
                    + IAndroidDeviceVerification.GetRegionString(Regions);
            }
        }

        #endregion

        #region Android SafetyNet Attestation API

        [JsonIgnore]
        public string JwsPayload
            => DeviceVerificationPayload;

        [JsonIgnore]
        public string ClearText
        {
            get
            {
                var hasSymptom = HasSymptom ? "HasSymptom" : "NoSymptom";
                return string.Join("|", AppPackageName, OnsetOfSymptomOrTestDate, hasSymptom, KeysTextForDeviceVerification, IAndroidDeviceVerification.GetRegionString(Regions), VerificationPayload);
            }
        }

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
            var onsetOfSymptomOrTestDate = DateTime.ParseExact(OnsetOfSymptomOrTestDate, Constants.FORMAT_TIMESTAMP, null).ToUniversalTime().Date;
            foreach (var key in Keys)
            {
                var dateOffset = key.GetDate() - onsetOfSymptomOrTestDate;
                key.DaysSinceOnsetOfSymptoms = dateOffset.Days;
            }
        }
    }
}
