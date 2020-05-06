using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms.Internals;

namespace Covid19Radar.Services
{
	public class ExposureNotificationHandler : IExposureNotificationHandler
	{
		const string apiUrlBase = "http://localhost:7071/api/";
		static readonly HttpClient http = new HttpClient();

		public Configuration Configuration
			=> new Configuration();

		public async Task ExposureDetected(ExposureDetectionSummary summary, Func<Task<IEnumerable<ExposureInfo>>> getDetailsFunc)
		{
			// TODO: Save this info and alert the user
			// Pop up a local notification
		}

		public async Task<IEnumerable<TemporaryExposureKey>> FetchExposureKeysFromServer()
		{
			const string prefsSinceKey = "keys_since";

			// Get the newest date we have keys from and request since then
			// or if no date stored, only return as much as the past 14 days of keys
			var sinceEpochSeconds = Preferences.Get(prefsSinceKey, DateTimeOffset.MinValue.ToUnixTimeSeconds());
			var url = $"{apiUrlBase.TrimEnd('/')}/keys?since={sinceEpochSeconds}";

			var response = await http.GetAsync(url);

			response.EnsureSuccessStatusCode();

			var responseData = await response.Content.ReadAsStringAsync();

			// Response contains the timestamp in seconds since epoch, and the list of keys
			var keys = JsonConvert.DeserializeObject<KeysResponse>(responseData);

			// Save newest timestamp for next request
			Preferences.Set(prefsSinceKey, keys.Timestamp);

			return keys.Keys;
		}

		const string prefsDiagnosisSubmissionDate = "prefs_diagnosis_submit_date";
		const string prefsDiagnosisSubmissionUid = "prefs_diagnosis_submit_uid";

		public static bool HasSubmittedDiagnosis
			=> Preferences.Get(prefsDiagnosisSubmissionDate, DateTime.MinValue)
				>= DateTime.UtcNow.AddDays(-14);

		public static string DiagnosisUid
		{
			get => Preferences.Get(prefsDiagnosisSubmissionUid, (string)null);
			set => Preferences.Set(prefsDiagnosisSubmissionUid, value);
		}

		public async Task UploadSelfExposureKeysToServer(IEnumerable<TemporaryExposureKey> temporaryExposureKeys)
		{
			var diagnosisUid = DiagnosisUid;

			if (string.IsNullOrEmpty(diagnosisUid))
				throw new InvalidOperationException();

			try
			{
				var url = $"{apiUrlBase.TrimEnd('/')}/selfdiagnosis";

				var json = JsonConvert.SerializeObject(new SelfDiagnosisSubmissionRequest
				{
					DiagnosisUid = diagnosisUid,
					Keys = temporaryExposureKeys
				});

				var http = new HttpClient();
				var response = await http.PostAsync(url, new StringContent(json));

				response.EnsureSuccessStatusCode();

				// Store the date we were diagnosed
				Preferences.Set(prefsDiagnosisSubmissionDate, DateTime.UtcNow);
			}
			catch
			{
				// Reset diagnosis status since we don't have one that was successfully submitted
				// and then re-throw
				Preferences.Set(prefsDiagnosisSubmissionDate, DateTime.UtcNow.AddDays(-100));
				throw;
			}
		}

		class SelfDiagnosisSubmissionRequest
		{
			[JsonProperty("diagnosisUid")]
			public string DiagnosisUid { get; set; }

			[JsonProperty("keys")]
			public IEnumerable<TemporaryExposureKey> Keys { get; set; }
		}

		class KeysResponse
		{
			[JsonProperty("timestamp")]
			public long Timestamp { get; set; }

			[JsonProperty("keys")]
			public IEnumerable<TemporaryExposureKey> Keys { get; set; }
		}
	}

}
