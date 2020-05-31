using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using ExposureNotification.Backend.Network;
using ExposureNotification.Backend.Proto;
using Google.Protobuf;

namespace ExposureNotification.Backend.Database
{
	public class DbTemporaryExposureKey
	{
		[Key, Column(Order = 0)]
		public string Id { get; set; } = Guid.NewGuid().ToString();

		public bool Processed { get; set; } = false;

		public string Region { get; set; }

		public string Base64KeyData { get; set; }

		public long TimestampMsSinceEpoch { get; set; }

		public long RollingStartSecondsSinceEpoch { get; set; }

		public int RollingDuration { get; set; }

		public int TransmissionRiskLevel { get; set; }

		[IgnoreDataMember]
		public long RollingDurationSeconds
			=> RollingDuration * 10 * 60;

		[IgnoreDataMember]
		public long RollingEndSecondsSinceEpoch
			=> RollingStartSecondsSinceEpoch + RollingDurationSeconds;

		public TemporaryExposureKey ToKey()
			=> new TemporaryExposureKey()
			{
				KeyData = ByteString.CopyFrom(Convert.FromBase64String(Base64KeyData)),
				RollingStartIntervalNumber = (int)RollingStartSecondsSinceEpoch,
				RollingPeriod = RollingDuration,
				TransmissionRiskLevel = TransmissionRiskLevel
			};

		public static DbTemporaryExposureKey FromKey(ExposureKey key, string region)
			=> new DbTemporaryExposureKey
			{
				Base64KeyData = key.Key,
				TimestampMsSinceEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
				RollingStartSecondsSinceEpoch = key.RollingStart,
				RollingDuration = key.RollingDuration,
				TransmissionRiskLevel = key.TransmissionRisk,
				Region = region
			};
	}
}
