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
        public string IdempotencyKey;

        [JsonProperty("platform")]
        public string Platform;

        [JsonProperty("appPackageName")]
        public string AppPackageName;

        [JsonProperty("deviceVerificationPayload")]
        public string DeviceVerificationPayload;

        [JsonProperty("event_logs")]
        public List<EventLog> EventLogs;
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
            get => DateTime.UnixEpoch.AddSeconds(Epoch).ToString(AppConstants.FORMAT_TIMESTAMP);
        }

        internal string GetEventType() => $"{Type}-{Subtype}";
    }
}
