// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using Chino;
using Newtonsoft.Json;

namespace Covid19Radar.Model
{
    public class ExposureData
    {
        [JsonProperty("platform")]
        public readonly string Platform;

        [JsonProperty("platform_version")]
        public readonly string PlatformVersion;

        [JsonProperty("model")]
        public readonly string Model;

        [JsonProperty("device_type")]
        public readonly string DeviceType;

        [JsonProperty("app_version")]
        public readonly string AppVersion;

        [JsonProperty("build_number")]
        public readonly string BuildNumber;

        [JsonProperty("en_version")]
        public readonly string EnVersion;

        [JsonProperty("daily_summaries")]
        public readonly List<DailySummary> DailySummaryList;

        [JsonProperty("exposure_windows")]
        public readonly List<ExposureWindow> ExposureWindowList;

        public ExposureData(
            string platform,
            string platformVersion,
            string model,
            string deviceType,
            string appVersion,
            string buildNumber,
            string enVersion,
            List<DailySummary> dailySummaryList,
            List<ExposureWindow> exposureWindowList
            )
        {
            Platform = platform;
            PlatformVersion = platformVersion;
            Model = model;
            DeviceType = deviceType;
            AppVersion = appVersion;
            BuildNumber = buildNumber;
            EnVersion = enVersion;
            DailySummaryList = dailySummaryList;
            ExposureWindowList = exposureWindowList;
        }
    }
}
