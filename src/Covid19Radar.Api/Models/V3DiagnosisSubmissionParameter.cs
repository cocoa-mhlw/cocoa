/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Common;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Covid19Radar.Api.Models
{

	public class V3DiagnosisSubmissionParameter : IPayload, IDeviceVerification
	{
		public const string FORMAT_SYMPTOM_ONSET_DATE = "yyyy-MM-dd'T'HH:mm:ss.fffzzz";

		private const int TRANSMISSION_RISK_LEVEL = 4;

		// RFC3339
		// e.g. 2021-09-20T23:52:57.436+00:00
		[JsonProperty("symptomOnsetDate")]
		public string SymptomOnsetDate { get; set; }

		[JsonProperty("keys")]
		public Key[] Keys { get; set; }

		[JsonProperty("regions")]
		public string[] Regions { get; set; }

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

		[JsonIgnore]
		public string KeyString
			=> string.Join(",", Keys.OrderBy(k => k.KeyData).Select(k => k.GetKeyString()));

		[JsonIgnore]
		public string KeysText {
			get
			{
				return Keys.OrderBy(_ => _.KeyData)
					.Select(_ => _.KeyData)
					.Aggregate((a, b) => a + b);
			}
		}

        public class Key
		{
			[JsonProperty("keyData")]
			public string KeyData { get; set; }

			[JsonProperty("rollingStartNumber")]
			public uint RollingStartNumber { get; set; }

			[JsonProperty("rollingPeriod")]
			public uint RollingPeriod { get; set; }

			[JsonProperty("reportType")]
			public uint ReportType { get; set; }

			[JsonIgnore]
			public int DaysSinceOnsetOfSymptoms { get; set; }

			public TemporaryExposureKeyModel ToModel(V3DiagnosisSubmissionParameter _, ulong timestamp)
			{
				return new TemporaryExposureKeyModel()
				{
					KeyData = Convert.FromBase64String(KeyData),
					RollingPeriod = ((int)RollingPeriod == 0 ? (int)Constants.ActiveRollingPeriod : (int)RollingPeriod),
					RollingStartIntervalNumber = (int)RollingStartNumber,
					TransmissionRiskLevel = TRANSMISSION_RISK_LEVEL,
					ReportType = (int)ReportType,
					DaysSinceOnsetOfSymptoms = DaysSinceOnsetOfSymptoms,
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
				var nowRollingStartNumber = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / TemporaryExposureKeyModel.TIME_WINDOW_IN_SEC;
				var oldestRollingStartNumber = new DateTimeOffset(DateTime.UtcNow.AddDays(Constants.OutOfDateDays).Date.Ticks, TimeSpan.Zero).ToUnixTimeSeconds() / TemporaryExposureKeyModel.TIME_WINDOW_IN_SEC;
				if (RollingStartNumber != 0 && (RollingStartNumber < oldestRollingStartNumber || RollingStartNumber > nowRollingStartNumber)) return false;
				return true;
			}

            public string GetKeyString() => string.Join(".", KeyData, RollingStartNumber, RollingPeriod, ReportType, DaysSinceOnsetOfSymptoms);
        }

		/// <summary>
		/// Validation
		/// </summary>
		/// <returns>true if valid</returns>
		public virtual bool IsValid()
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
