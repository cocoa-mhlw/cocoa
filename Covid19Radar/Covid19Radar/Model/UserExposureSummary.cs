using System;
using System.Collections.Generic;
using System.Text;

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

		public int DaysSinceLastExposure { get; }

		public ulong MatchedKeyCount { get; }

		public int HighestRiskScore { get; }

		public TimeSpan[] AttenuationDurations { get; }

		public int SummationRiskScore { get; }
	}

}
