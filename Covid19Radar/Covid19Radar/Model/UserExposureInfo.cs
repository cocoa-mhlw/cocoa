/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Chino;

namespace Covid19Radar.Model
{
    public class UserExposureInfo
    {
        public UserExposureInfo(DateTime timestamp, TimeSpan duration, int attenuationValue, int totalRiskScore, RiskLevel riskLevel)
        {
            Timestamp = timestamp;
            Duration = duration;
            AttenuationValue = attenuationValue;
            TotalRiskScore = totalRiskScore;
            TransmissionRiskLevel = riskLevel;
        }

        public UserExposureInfo(ExposureInformation exposureInformation)
        {
            Timestamp = DateTimeOffset.UnixEpoch.AddMilliseconds(exposureInformation.DateMillisSinceEpoch).UtcDateTime;
            Duration = TimeSpan.FromMilliseconds(exposureInformation.DurationInMillis);
            AttenuationValue = exposureInformation.AttenuationValue;
            TotalRiskScore = exposureInformation.TotalRiskScore;
            TransmissionRiskLevel = exposureInformation.TransmissionRiskLevel;
        }

        // When the contact occurred
        public DateTime Timestamp { get; set; }

        // How long the contact lasted in 5 min increments
        public TimeSpan Duration { get; set; }

        public int AttenuationValue { get; set; }

        public int TotalRiskScore { get; set; }

        public RiskLevel TransmissionRiskLevel { get; set; }
    }
}
