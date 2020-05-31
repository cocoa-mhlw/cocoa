using System;

namespace Xamarin.ExposureNotifications
{
	public class ExposureInfo
	{
		public ExposureInfo()
		{
		}

		public ExposureInfo(DateTime timestamp, TimeSpan duration, int attenuationValue, int totalRiskScore, RiskLevel riskLevel)
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

		public RiskLevel TransmissionRiskLevel { get; }
	}

	public class ExposureDetectionSummary
	{
		public ExposureDetectionSummary(int daysSinceLastExposure, ulong matchedKeyCount, byte maximumRiskScore)
		{
			DaysSinceLastExposure = daysSinceLastExposure;
			MatchedKeyCount = matchedKeyCount;
			MaximumRiskScore = maximumRiskScore;
		}

		public int DaysSinceLastExposure { get; }

		public ulong MatchedKeyCount { get; }

		public byte MaximumRiskScore { get; }
	}

	public enum RiskLevel
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
