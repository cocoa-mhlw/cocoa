using Newtonsoft.Json;

namespace Covid19Radar.Model
{
    public class LogStorageSas
    {
        [JsonProperty(PropertyName = "sas_token")]
        public string SasToken { get; set; }
    }
}
