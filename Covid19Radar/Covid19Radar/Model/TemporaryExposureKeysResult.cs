using Newtonsoft.Json;
using System.Collections.Generic;

namespace Covid19Radar.Model
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
