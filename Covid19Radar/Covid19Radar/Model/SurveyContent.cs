// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Covid19Radar.Model
{
    public class SurveyContent
    {
        [JsonProperty("q1")]
        public int? Q1 { get; set; }

        [JsonProperty("q2")]
        public int? Q2 { get; set; }

        [JsonProperty("start_date")]
        public long? StartDate { get; set; }

        [JsonProperty("exposure_data")]
        public SurveyExposureData ExposureData { get; set; }
    }
}

