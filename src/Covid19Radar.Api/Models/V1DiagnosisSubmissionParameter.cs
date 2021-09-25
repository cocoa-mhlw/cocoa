/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Common;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Covid19Radar.Api.Models
{
	public class V1DiagnosisSubmissionParameter : DiagnosisSubmissionParameter, IUser, IDeviceVerification
	{
		private const int TRANSMISSION_RISK_LEVEL = 4;

		[JsonProperty("userUuid")]
		public string UserUuid { get; set; }

		[JsonProperty("keys")]
		public new Key[] Keys { get; set; }

		[JsonIgnore]
		public new string KeyString
			=> string.Join(",", Keys.OrderBy(k => k.KeyData).Select(k => k.GetKeyString()));

		[JsonIgnore]
		public new string KeysText
		{
			get
			{
				return Keys.OrderBy(_ => _.KeyData)
					.Select(_ => _.KeyData)
					.Aggregate((a, b) => a + b);
			}
		}

		public new class Key
		{
			[JsonProperty("keyData")]
			public string KeyData { get; set; }
			[JsonProperty("rollingStartNumber")]
			public uint RollingStartNumber { get; set; }
			[JsonProperty("rollingPeriod")]
			public uint RollingPeriod { get; set; }
			[JsonProperty("transmissionRisk")]
			public int TransmissionRisk { get; set; }
			public TemporaryExposureKeyModel ToModel(V1DiagnosisSubmissionParameter _, ulong timestamp)
			{
				return new TemporaryExposureKeyModel()
				{
					KeyData = Convert.FromBase64String(this.KeyData),
					RollingPeriod = ((int)this.RollingPeriod == 0 ? (int)Constants.ActiveRollingPeriod : (int)this.RollingPeriod),
					RollingStartIntervalNumber = (int)this.RollingStartNumber,
					TransmissionRiskLevel = TRANSMISSION_RISK_LEVEL,

					// TODO: We should consider which report-type choose when accept submission from Legacy-COCOA.
					//ReportType = (int)ReportType,

					// TODO: We should consider what days-since-onset-of-symptoms assign when accept submission from Legacy-COCOA.
					//DaysSinceOnsetOfSymptoms = DaysSinceOnsetOfSymptoms,

					Timestamp = timestamp,
					Exported = false
				};
			}
			/// <summary>
			/// Validation
			/// </summary>
			/// <returns>true if valid</returns>
			public bool IsValid()
			{
				if (string.IsNullOrWhiteSpace(KeyData)) return false;
				if (RollingPeriod != 0 && RollingPeriod > Constants.ActiveRollingPeriod) return false;
				var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 600;
				var oldest = new DateTimeOffset(DateTime.UtcNow.AddDays(Constants.OutOfDateDays).Date.Ticks, TimeSpan.Zero).ToUnixTimeSeconds() / 600;
				if (RollingStartNumber != 0 && (RollingStartNumber < oldest || RollingStartNumber > now)) return false;
				return true;
			}

            internal string GetKeyString() => string.Join(".", KeyData, RollingStartNumber, RollingPeriod, TransmissionRisk);
		}

		public override bool IsValid()
		{
			if (string.IsNullOrWhiteSpace(VerificationPayload)) return false;
			if ((Regions?.Length ?? 0) == 0) return false;
			if (string.IsNullOrWhiteSpace(Platform)) return false;
			if (string.IsNullOrWhiteSpace(DeviceVerificationPayload)) return false;
			if (string.IsNullOrWhiteSpace(AppPackageName)) return false;
			if (Keys.Any(_ => !_.IsValid())) return false;
			return true;
		}

	}
}
