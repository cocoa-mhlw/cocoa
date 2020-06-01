using Newtonsoft.Json;

namespace ExposureNotification.Backend.Network
{
	public class ExposureKey
	{
		// Base64 encoded temporary exposure key from the device
		[JsonProperty("key")]
		public string Key { get; set; }

		// Intervals are 10 minute increments since the UTC epoch
		[JsonProperty("rollingStart")]
		public long RollingStart { get; set; }

		// Number of intervals that the key is valid for
		[JsonProperty("rollingDuration")]
		public int RollingDuration { get; set; }

		// Must be >= 0 and <= 8
		[JsonProperty("transmissionRisk")]
		public int TransmissionRisk { get; set; }

		[JsonIgnore]
		public long RollingDurationSeconds
			=> RollingDuration * 10 * 60;

		[JsonIgnore]
		public long RollingEnd
			=> RollingStart + RollingDurationSeconds;
	}
}
