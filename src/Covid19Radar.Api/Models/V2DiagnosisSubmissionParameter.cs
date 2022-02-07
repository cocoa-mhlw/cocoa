﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Common;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Covid19Radar.Api.Models
{

    public class V2DiagnosisSubmissionParameter : IPayload, IDeviceVerification
    {
        public const int TRANSMISSION_RISK_LEVEL = 4;

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
        public virtual string DeviceToken
            => DeviceVerificationPayload;

        [JsonIgnore]
        public virtual string TransactionIdSeed
            => AppPackageName
                + KeysTextForDeviceVerification
                + IAndroidDeviceVerification.GetRegionString(Regions);

        #endregion

        #region Android SafetyNet Attestation API

        [JsonIgnore]
        public virtual string JwsPayload
            => DeviceVerificationPayload;

        [JsonIgnore]
        public virtual string ClearText
            => string.Join("|", AppPackageName, KeysTextForDeviceVerification, IAndroidDeviceVerification.GetRegionString(Regions), VerificationPayload);

        #endregion
        public class Key
        {
            [JsonProperty("keyData")]
            public string KeyData { get; set; }
            [JsonProperty("rollingStartNumber")]
            public uint RollingStartNumber { get; set; }
            [JsonProperty("rollingPeriod")]
            public uint RollingPeriod { get; set; }

            public TemporaryExposureKeyModel ToModel(V2DiagnosisSubmissionParameter _, ulong timestamp)
            {
                return new TemporaryExposureKeyModel()
                {
                    KeyData = Convert.FromBase64String(this.KeyData),
                    RollingPeriod = ((int)this.RollingPeriod == 0 ? (int)Constants.ActiveRollingPeriod : (int)this.RollingPeriod),
                    RollingStartIntervalNumber = (int)this.RollingStartNumber,
                    TransmissionRiskLevel = TRANSMISSION_RISK_LEVEL,
                    ReportType = Constants.ReportTypeMissingValue,
                    DaysSinceOnsetOfSymptoms = Constants.DaysSinceOnsetOfSymptomsMissingValue,
                    Timestamp = timestamp,
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
                if (RollingPeriod != 0 && RollingPeriod > Constants.ActiveRollingPeriod) return false;
                var nowRollingStartNumber = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / TemporaryExposureKeyModel.TIME_WINDOW_IN_SEC;
                var oldestRollingStartNumber = new DateTimeOffset(DateTime.UtcNow.AddDays(Constants.OutOfDateDays).Date.Ticks, TimeSpan.Zero).ToUnixTimeSeconds() / TemporaryExposureKeyModel.TIME_WINDOW_IN_SEC;
                if (RollingStartNumber != 0 && (RollingStartNumber < oldestRollingStartNumber || RollingStartNumber > nowRollingStartNumber)) return false;
                return true;
            }

            public string GetKeyString() => string.Join(".", KeyData, RollingStartNumber, RollingPeriod);
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
            if (Keys.Any(_ => !_.IsValid())) return false;
            return true;
        }
    }
}
