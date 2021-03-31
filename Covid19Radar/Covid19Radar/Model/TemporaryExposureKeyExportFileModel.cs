/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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
