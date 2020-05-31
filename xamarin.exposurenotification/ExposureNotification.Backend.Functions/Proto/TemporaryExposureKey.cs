using System;
using System.Text;
using Google.Protobuf;

namespace ExposureNotification.Backend.Proto
{
	public partial class TemporaryExposureKeyExport
	{
		public const int MaxKeysPerFile = 750000;

		public const string HeaderContents = "EK Export v1    ";

		public static readonly byte[] Header = Encoding.UTF8.GetBytes(HeaderContents);
	}

	public partial class TemporaryExposureKey
	{
		public TemporaryExposureKey(byte[] keyData, DateTimeOffset rollingStart, TimeSpan rollingDuration, RiskLevel transmissionRisk)
		{
			KeyData = ByteString.CopyFrom(keyData);
			RollingStartIntervalNumber = (int)rollingStart.ToUnixTimeSeconds();
			RollingPeriod = (int)rollingDuration.TotalMinutes;
			TransmissionRiskLevel = (int)transmissionRisk;
		}
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
