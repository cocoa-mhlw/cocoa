using Covid19Radar.Api.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
