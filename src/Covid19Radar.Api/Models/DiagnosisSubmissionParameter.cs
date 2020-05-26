using Covid19Radar.Api.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Covid19Radar.Api.Models
{
	public class DiagnosisSubmissionParameter : IUser, IPayload
	{
		[JsonProperty("userUuid")]
		public string UserUuid { get; set; }
        [JsonProperty("keys")]
		public Key[] Keys { get; set; }
		[JsonProperty("region")]
		public string Region { get; set; }
		[JsonProperty("platform")]
		public string Platform { get; set; }
		[JsonProperty("deviceVerificationPayload")]
		public string DeviceVerificationPayload { get; set; }
		[JsonProperty("appPackageName")]
		public string AppPackageName { get; set; }
		// Some signature / code confirming authorization by the verification authority.
		[JsonProperty("verificationPayload")]
		public string VerificationPayload { get; set; }
		// Random data to obscure the size of the request network packet sniffers.
		[JsonProperty("padding")]
		public string Padding { get; set; }

		public class Key
		{
			[JsonProperty("keyData")]
			public string KeyData { get; set; }
			[JsonProperty("rollingStartNumber")]
			public uint RollingStartNumber { get; set; }
			[JsonProperty("rollingPeriod ")]
			public uint RollingPeriod { get; set; }
			[JsonProperty("transmissionRisk")]
			public int TransmissionRisk { get; set; }
			public TemporaryExposureKeyModel ToModel(DiagnosisSubmissionParameter p, ulong timestamp)
			{
				return new TemporaryExposureKeyModel()
				{
					KeyData = Convert.FromBase64String(this.KeyData),
					Region = p.Region,
					RollingPeriod = (int)this.RollingPeriod,
					RollingStartIntervalNumber = (int)this.RollingStartNumber,
					TransmissionRiskLevel = TransmissionRisk,
					Timestamp = timestamp
				};
			}
			/// <summary>
			/// Validation
			/// </summary>
			/// <returns>true if valid</returns>
			public bool IsValid()
			{
				if (string.IsNullOrWhiteSpace(KeyData)) return false;
				if (RollingPeriod != Constants.ActiveRollingPeriod) return false;
				if (RollingStartNumber < (DateTimeOffset.UtcNow.AddDays(Constants.OutOfDateDays).ToUnixTimeSeconds() / 600)) return false;
				return true;
			}
		}

		/// <summary>
		/// Validation
		/// </summary>
		/// <returns>true if valid</returns>
		public bool IsValid()
		{
			if (string.IsNullOrWhiteSpace(VerificationPayload)) return false;
			if (string.IsNullOrWhiteSpace(UserUuid)) return false;
			if (string.IsNullOrWhiteSpace(Region)) return false;
			if (string.IsNullOrWhiteSpace(Platform)) return false;
			if (string.IsNullOrWhiteSpace(DeviceVerificationPayload)) return false;
			if (string.IsNullOrWhiteSpace(AppPackageName)) return false;
			if (Keys.Any(_ => !_.IsValid())) return false;
			return true;
		}
	}
}
