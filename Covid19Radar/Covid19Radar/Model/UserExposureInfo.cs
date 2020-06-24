using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

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
        public DateTime Timestamp { get; }

        // How long the contact lasted in 5 min increments
        public TimeSpan Duration { get; }

        public int AttenuationValue { get; }

        public int TotalRiskScore { get; }

        public UserRiskLevel TransmissionRiskLevel { get; }
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
