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
