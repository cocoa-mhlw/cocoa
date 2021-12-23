/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Newtonsoft.Json;

namespace Covid19Radar.Api.Models
{
    public class EventLog
    {
        public const string FORMAT_SYMPTOM_ONSET_DATE = "yyyy-MM-dd'T'HH:mm:ss.fffzzz";

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
            get => DateTime.UnixEpoch.AddSeconds(Epoch).ToString(FORMAT_SYMPTOM_ONSET_DATE);
        }

        [JsonIgnore]
        public long Created { get; internal set; }

        public string ClearText => string.Join(".", HasConsent, Epoch, Type, Subtype, Content);
    }
}
