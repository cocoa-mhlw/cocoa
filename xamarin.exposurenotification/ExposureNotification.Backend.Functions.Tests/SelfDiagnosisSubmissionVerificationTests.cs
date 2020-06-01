using System;
using System.Collections.Generic;
using System.Text;
using ExposureNotification.Backend.Network;
using Xunit;

namespace ExposureNotification.Backend.Functions.Tests
{
	public class SelfDiagnosisSubmissionVerificationTests
	{
		[Fact]
		public void DetectKeysOver14Days()
		{
			var s = new SelfDiagnosisSubmission(true);

			for (var i = 1; i <= 15; i++)
			{
				s.Keys.Add(new ExposureKey
				{
					Key = "",
					RollingStart = new DateTimeOffset(2020, 1, i, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
					RollingDuration = 144,
					TransmissionRisk = 1
				}); ;

			}

			Assert.False(s.Validate());
		}

		[Fact]
		public void DetectKeysUnder14Days()
		{
			var s = new SelfDiagnosisSubmission(true);

			for (var i = 1; i <= 14; i++)
			{
				s.Keys.Add(new ExposureKey
				{
					Key = "",
					RollingStart = new DateTimeOffset(2020, 1, i, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
					RollingDuration = 144,
					TransmissionRisk = 1
				}); ;

			}

			Assert.True(s.Validate());
		}

		[Fact]
		public void DetectKeyTimesOverlap()
		{
			var s = new SelfDiagnosisSubmission(true)
			{
				Keys = new List<ExposureKey>
				{
					new ExposureKey
					{
						Key = "",
						RollingStart = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
						RollingDuration = 144,
						TransmissionRisk = 1
					},
					new ExposureKey
					{
						Key = "",
						RollingStart = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
						RollingDuration = 144,
						TransmissionRisk = 1
					},
				}
			};

			Assert.False(s.Validate());
		}


		[Fact]
		public void DetectKeyTimesDoNotOverlap()
		{
			var s = new SelfDiagnosisSubmission(true)
			{
				Keys = new List<ExposureKey>
				{
					new ExposureKey
					{
						Key = "",
						RollingStart = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
						RollingDuration = 144,
						TransmissionRisk = 1
					},
					new ExposureKey
					{
						Key = "",
						RollingStart = new DateTimeOffset(2020, 1, 2, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
						RollingDuration = 144,
						TransmissionRisk = 1
					},
				}
			};

			Assert.True(s.Validate());
		}
	}
}
