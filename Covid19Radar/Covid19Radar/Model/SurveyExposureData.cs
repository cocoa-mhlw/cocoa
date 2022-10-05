// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using Chino;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Covid19Radar.Model
{
    public class SurveyExposureData
    {
        [JsonProperty("en_version")]
        public string EnVersion { get; set; }

        [JsonProperty("daily_summaries")]
        public List<DailySummary> DailySummaryList { get; set; }

        [JsonProperty("exposure_windows")]
        public List<ExposureWindow> ExposureWindowList { get; set; }
    }
}

