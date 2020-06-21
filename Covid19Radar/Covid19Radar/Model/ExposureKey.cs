using Newtonsoft.Json;

namespace Covid19Radar.Model
{
    public class ExposureKey
    {
        // Base64 encoded temporary exposure key from the device
        [JsonProperty("keyData")]
        public string KeyData { get; set; }
        // Intervals are 10 minute increments since the UTC epoch
        [JsonProperty("rollingStartNumber")]
        public long RollingStart { get; set; }
        // Number of intervals that the key is valid for
        [JsonProperty("rollingPeriod")]
        public int RollingDuration { get; set; }
        // Must be >= 0 and <= 8
        [JsonProperty("transmissionRisk")]
        public int TransmissionRisk { get; set; }
    }

}
