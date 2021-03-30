/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using System;

namespace Covid19Radar.Model
{
    public class UserExposureInfo
    {
        public UserExposureInfo(DateTime timestamp, TimeSpan duration, int attenuationValue, int totalRiskScore, UserRiskLevel riskLevel)
        {
            Timestamp = timestamp;
            Duration = duration;
            AttenuationValue = attenuationValue;
            TotalRiskScore = totalRiskScore;
            TransmissionRiskLevel = riskLevel;
        }

        // When the contact occurred
        public DateTime Timestamp { get; set; }

        // How long the contact lasted in 5 min increments
        public TimeSpan Duration { get; set; }

        public int AttenuationValue { get; set; }

        public int TotalRiskScore { get; set; }

        public UserRiskLevel TransmissionRiskLevel { get; set; }
    }

    public enum UserRiskLevel
    {
        Invalid = 0,
        Lowest = 1,
        Low = 2,
        MediumLow = 3,
        Medium = 4,
        MediumHigh = 5,
        High = 6,
        VeryHigh = 7,
        Highest = 8
    }
}
