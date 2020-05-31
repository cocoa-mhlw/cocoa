using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExposureNotification.Backend.Database
{
	public class AuthorizedAppConfig
	{
		public string PackageName { get; set; }

		public string Platform { get; set; }

		public string[] AllowedRegions { get; set; }

		// SafetyNet configuration

		public string[] SafetyNetApkDigestSha256 { get; set; }

		public bool SafetyNetBasicIntegrity { get; set; }

		public bool SafetyNetCtsProfileMatch { get; set; }

		public int SafetyNetPastTimeSeconds { get; set; }

		public int SafetyNetFutureTimeSeconds { get; set; }

		// DeviceCheck configuration

		public string DeviceCheckKeyId { get; set; }

		public string DeviceCheckTeamId { get; set; }

		public string DeviceCheckPrivateKey { get; set; }

		// Utils

		public static DevicePlatform ParsePlatform(string platform) =>
			platform.Equals("android", StringComparison.OrdinalIgnoreCase)
				? DevicePlatform.Android
				: DevicePlatform.iOS;

		public enum DevicePlatform
		{
			iOS,
			Android
		}
	}
}
