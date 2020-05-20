using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
	public class DiagnosisSubmissionParameter : IUser
	{
		[JsonProperty("submissionNumber")]
		public string SubmissionNumber { get; set; }
		[JsonProperty("userUuid")]
		public string UserUuid { get; set; }
        [JsonProperty("keys")]
		public Key[] Keys { get; set; }
		[JsonProperty("region")]
		public string Region { get; set; }
		[JsonProperty("platform")]
		public string Platform { get; set; }
		[JsonProperty("transmissionRisk")]
		public int TransmissionRisk { get; set; }
		[JsonProperty("deviceVerificationPayload")]
		public string DeviceVerificationPayload { get; set; }
		[JsonProperty("appPackageName")]
		public string AppPackageName { get; set; }
		public class Key
		{
			[JsonProperty("keyData")]
			public string KeyData { get; set; }
			[JsonProperty("rollingStartNumber")]
			public uint RollingStartNumber { get; set; }
			[JsonProperty("rollingPeriod ")]
			public uint RollingPeriod { get; set; }

			public TemporaryExposureKeyModel ToModel(DiagnosisSubmissionParameter p, ulong timestamp)
			{
				return new TemporaryExposureKeyModel()
				{
					KeyData = Convert.FromBase64String(this.KeyData),
					Region = p.Region,
					RollingPeriod = (int)this.RollingPeriod,
					RollingStartIntervalNumber = (int)this.RollingStartNumber,
					TransmissionRiskLevel = p.TransmissionRisk,
					Timestamp = timestamp
				};
			}
		}
	}
}
