/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Covid19Radar.Api.Models
{
    public class TemporaryExposureKeysResult
    {
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("keys")]
        public IEnumerable<Key> Keys { get; set; }

        public class Key
        {
            public string Url;
        }
    }
}
