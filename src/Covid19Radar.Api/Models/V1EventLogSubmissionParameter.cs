/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Linq;
using Covid19Radar.Api.Common;
using Newtonsoft.Json;

namespace Covid19Radar.Api.Models
{
    public class V1EventLogSubmissionParameter : IDeviceVerification
    {
        [JsonProperty("idempotency_key")]
        public string IdempotencyKey;

        [JsonProperty("platform")]
        public string Platform;

        [JsonProperty("appPackageName")]
        public string AppPackageName;

        [JsonProperty("deviceVerificationPayload")]
        public string DeviceVerificationPayload;

        [JsonProperty("event_logs")]
        public EventLog[] EventLogs;

        #region Apple Device Check

        [JsonIgnore]
        public string DeviceToken
            => DeviceVerificationPayload;

        [JsonIgnore]
        public string TransactionIdSeed
            => string.Join("|", AppPackageName, KeysTextForDeviceVerification);

        #endregion

        #region Android SafetyNet Attestation API

        [JsonIgnore]
        public string JwsPayload
            => DeviceVerificationPayload;

        [JsonIgnore]
        public string ClearText
            => string.Join("|", AppPackageName, KeysTextForDeviceVerification);

        private string KeysTextForDeviceVerification
            => string.Join(",", EventLogs.Select(el => el.ClearText));

        #endregion
    }

    public class EventLog
    {
        [JsonProperty("has_consent")]
        public bool HasConsent;

        [JsonProperty("epoch")]
        public long Epoch;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("subtype")]
        public string Subtype;

        [JsonProperty("content")]
        public string Content;

        [JsonIgnore]
        public string Timestamp
        {
            get => DateTime.UnixEpoch.AddSeconds(Epoch).ToString(Constants.FORMAT_TIMESTAMP);
        }

        public string ClearText => string.Join(".", HasConsent, Epoch, Type, Subtype, Content);
    }
}
