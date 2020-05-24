using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Model
{
    public class TemporaryExposureKeysHttpResultModel
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
