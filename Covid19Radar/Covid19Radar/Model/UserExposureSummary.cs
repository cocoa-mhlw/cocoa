/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;

namespace Covid19Radar.Model
{
    public class UserExposureSummary
    {
        public UserExposureSummary(int daysSinceLastExposure, ulong matchedKeyCount, int highestRiskScore, TimeSpan[] attenuationDurations, int summationRiskScore)
        {
            DaysSinceLastExposure = daysSinceLastExposure;
            MatchedKeyCount = matchedKeyCount;
            HighestRiskScore = highestRiskScore;
            AttenuationDurations = attenuationDurations;
            SummationRiskScore = summationRiskScore;
        }

        public int DaysSinceLastExposure { get; set; }

        public ulong MatchedKeyCount { get; set; }

        public int HighestRiskScore { get; set; }

        public TimeSpan[] AttenuationDurations { get; set; }

        public int SummationRiskScore { get; set; }
    }
}
