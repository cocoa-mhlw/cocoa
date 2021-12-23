using System;
using System.Linq;
using Covid19Radar.Api.Common;
using Newtonsoft.Json;

namespace Covid19Radar.Api.Models
{
    public class EventLogSubmissionParameter : IDeviceVerification
    {
        [JsonProperty("idempotency_key")]
        public string IdempotencyKey { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("appPackageName")]
        public string AppPackageName { get; set; }

        [JsonProperty("deviceVerificationPayload")]
        public string DeviceVerificationPayload { get; set; }

        [JsonProperty("event_logs")]
        public EventLog[] EventLogs { get; set; }

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

        public class EventLog
        {
            [JsonProperty("has_concent")]
            public bool HasConsent { get; } = false;

            [JsonProperty("epoch")]
            public long Epoch { get; }

            [JsonProperty("type")]
            public string Type { get; }

            [JsonProperty("subtype")]
            public string Subtype { get; }

            [JsonProperty("content")]
            public readonly string Content;

            [JsonProperty("timestamp")]
            public string Timestamp
            {
                get => DateTime.UnixEpoch.AddSeconds(Epoch).ToString(Constants.FORMAT_TIMESTAMP);
            }

            [JsonIgnore]
            public long Created { get; internal set; }

            public string ClearText => string.Join(".", HasConsent, Epoch, Type, Subtype, Content);
        }
    }
}
