// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using Covid19Radar.Common;
using Newtonsoft.Json;

namespace Covid19Radar.Model
{
    public class V1EventLogRequest
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
        public List<EventLog> EventLogs { get; set; }
    }

    public class EventLog
    {
        [JsonProperty("has_consent")]
        public bool HasConsent { get; set; }

        [JsonProperty("epoch")]
        public long Epoch { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("subtype")]
        public string Subtype { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonIgnore]
        public string Timestamp
        {
            get => DateTime.UnixEpoch.AddSeconds(Epoch).ToString(AppConstants.FORMAT_TIMESTAMP);
        }

        internal string GetEventType() => $"{Type}-{Subtype}";
    }
}
