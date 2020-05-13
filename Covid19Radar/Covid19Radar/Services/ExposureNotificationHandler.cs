
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Covid19Radar.Services
{
	[Preserve]
	public class ExposureNotificationHandler : IExposureNotificationHandler
	{
		const string apiUrlBase = "http://localhost:7071/api/";

		static readonly HttpClient http = new HttpClient();

		public string UserExplanation => throw new NotImplementedException();

		public Task<Configuration> GetConfigurationAsync()
			=> Task.FromResult(new Configuration());

		public async Task ExposureDetected(ExposureDetectionSummary summary, Func<Task<IEnumerable<ExposureInfo>>> getDetailsFunc)
		{
			LocalStateManager.Instance.ExposureSummary = summary;

			var details = await getDetailsFunc();

			LocalStateManager.Instance.ExposureInformation.AddRange(details);

			LocalStateManager.Save();

			MessagingCenter.Instance.Send(this, "exposure_info_changed");

			// TODO: Save this info and alert the user
			// Pop up a local notification
		}

		public async Task FetchExposureKeysFromServer(Func<IEnumerable<TemporaryExposureKey>, Task> addKeys)
		{
			var latestKeysResponseIndex = LocalStateManager.Instance.LatestKeysResponseIndex;

			var take = 1024;
			var skip = 0;

			var checkForMore = false;

			do
			{
				// Get the newest date we have keys from and request since then
				// or if no date stored, only return as much as the past 14 days of keys
				var url = $"{apiUrlBase.TrimEnd('/')}/keys?since={latestKeysResponseIndex}&skip={skip}&take={take}";

				var response = await http.GetAsync(url);

				response.EnsureSuccessStatusCode();

				var responseData = await response.Content.ReadAsStringAsync();

				if (string.IsNullOrEmpty(responseData))
					break;

				// Response contains the timestamp in seconds since epoch, and the list of keys
				var keys = JsonConvert.DeserializeObject<KeysResponse>(responseData);

				var numKeys = keys?.Keys?.Count() ?? 0;

				// See if keys were returned on this call
				if (numKeys > 0)
				{
					// Call the callback with the batch of keys to add
					await addKeys(keys.Keys);

					var newLatestKeysResponseIndex = keys.Latest;

					if (newLatestKeysResponseIndex > LocalStateManager.Instance.LatestKeysResponseIndex)
					{
						LocalStateManager.Instance.LatestKeysResponseIndex = newLatestKeysResponseIndex;
						LocalStateManager.Save();
					}

					// Increment our skip starting point for the next batch
					skip += take;
				}

				// If we got back more or the same amount of our requested take, there may be
				// more left on the server to request again
				checkForMore = numKeys >= take;

			} while (checkForMore);
		}

		public async Task UploadSelfExposureKeysToServer(IEnumerable<TemporaryExposureKey> temporaryExposureKeys)
		{
			var diagnosisUid = LocalStateManager.Instance.LatestDiagnosis.DiagnosisUid;

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
				var response = await http.PutAsync(url, new StringContent(json));

				response.EnsureSuccessStatusCode();

				LocalStateManager.Instance.LatestDiagnosis.Shared = true;
				LocalStateManager.Save();
			}
			catch
			{
				throw;
			}
		}

		internal static async Task<bool> VerifyDiagnosisUid(string diagnosisUid)
		{
			var url = $"{apiUrlBase.TrimEnd('/')}/selfdiagnosis";

			var http = new HttpClient();

			try
			{
				var response = await http.PostAsync(url, new StringContent(diagnosisUid));

				response.EnsureSuccessStatusCode();

				return true;
			}
			catch
			{
				return false;
			}
		}

		public Task FetchExposureKeysFromServerAsync(ITemporaryExposureKeyBatches batches)
		{
			throw new NotImplementedException();
		}

		public Task ExposureDetectedAsync(ExposureDetectionSummary summary, IEnumerable<ExposureInfo> ExposureInfo)
		{
			throw new NotImplementedException();
		}

		public Task UploadSelfExposureKeysToServerAsync(IEnumerable<TemporaryExposureKey> temporaryExposureKeys)
		{
			throw new NotImplementedException();
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
			[JsonProperty("latest")]
			public ulong Latest { get; set; }

			[JsonProperty("keys")]
			public IEnumerable<TemporaryExposureKey> Keys { get; set; }
		}
	}
}