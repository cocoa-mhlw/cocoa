using Newtonsoft.Json;

namespace Covid19Radar.Model
{
    /// <summary>
    /// export file information model
    /// </summary>
    public class TemporaryExposureKeyExportFileModel
    {
        /// <summary>
        /// region
        /// </summary>
        [JsonProperty("region")]
        public string Region { get; set; }
        /// <summary>
        /// download url
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        /// <summary>
        /// created unix time secs
        /// </summary>
        [JsonProperty("created")]
        public long Created { get; set; }

    }
}
