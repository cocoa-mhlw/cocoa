using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Background.Models
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
        public ulong Created { get; set; }
    }
}
