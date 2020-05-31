using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExposureNotification.App.Services;
using ExposureNotification.Backend.Network;
using Newtonsoft.Json;
using Plugin.LocalNotification;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace ExposureNotification.App
{
	[Xamarin.Forms.Internals.Preserve] // Ensure this isn't linked out
	public class ExposureNotificationHandler : IExposureNotificationHandler
	{
		static readonly HttpClient http = new HttpClient();

		// this string should be localized
		public string UserExplanation
			=> "We need to make use of the keys to keep you healthy.";

		// this configuration should be obtained from a server and it should be cached locally/in memory as it may be called multiple times
		public Task<Configuration> GetConfigurationAsync()
			=> Task.FromResult(new Configuration());

		// this will be called when a potential exposure has been detected
		public async Task ExposureDetectedAsync(ExposureDetectionSummary summary, IEnumerable<ExposureInfo> exposureInfo)
		{
			LocalStateManager.Instance.ExposureSummary = summary;

			// Add these on main thread in case the UI is visible so it can update
			await Device.InvokeOnMainThreadAsync(() =>
			{
				foreach (var i in exposureInfo)
					LocalStateManager.Instance.ExposureInformation.Add(i);
			});

			LocalStateManager.Save();
			// If Enabled Local Notifications
            if (LocalStateManager.Instance.EnableNotifications)
            {
				var notification = new NotificationRequest
				{
					NotificationId = 100,
					Title = "Possible COVID-19 Exposure",
					Description = "It is possible you have been exposed to someone who was a confirmed diagnosis of COVID-19.  Tap for more details."
				};

				NotificationCenter.Current.Show(notification);
			}
		}

		// this will be called when they keys need to be collected from the server
		public async Task FetchExposureKeyBatchFilesFromServerAsync(Func<IEnumerable<string>, Task> submitBatches, CancellationToken cancellationToken)
		{
			// This is "default" by default
			var rightNow = DateTimeOffset.UtcNow;

			try
			{
				foreach (var serverRegion in AppSettings.Instance.SupportedRegions)
				{
					// Find next directory to start checking
					var dirNumber = LocalStateManager.Instance.ServerBatchNumbers[serverRegion] + 1;

					// For all the directories
					while (true)
					{
						cancellationToken.ThrowIfCancellationRequested();

						// Download all the files for this directory
						var (batchNumber, downloadedFiles) = await DownloadBatchAsync(serverRegion, dirNumber, cancellationToken);
						if (batchNumber == 0)
							break;

						// Process the current directory, if there were any files
						if (downloadedFiles.Count > 0)
						{
							await submitBatches(downloadedFiles);

							// delete all temporary files
							foreach (var file in downloadedFiles)
							{
								try
								{
									File.Delete(file);
								}
								catch
								{
									// no-op
								}
							}
						}

						// Update the preferences
						LocalStateManager.Instance.ServerBatchNumbers[serverRegion] = dirNumber;
						LocalStateManager.Save();

						dirNumber++;
					}
				}
			}
			catch (Exception ex)
			{
				// any expections, bail out and wait for the next time

				// TODO: log the error on some server!
				Console.WriteLine(ex);
			}

			async Task<(int, List<string>)> DownloadBatchAsync(string region, ulong dirNumber, CancellationToken cancellationToken)
			{
				var downloadedFiles = new List<string>();
				var batchNumber = 0;

				// For all the batches in a directory
				while (true)
				{
					// Build the blob storage url for the given batch file we are on next
					var url = $"{AppSettings.Instance.BlobStorageUrlBase}/{AppSettings.Instance.BlobStorageContainerNamePrefix}{region.ToLowerInvariant()}/{dirNumber}/{batchNumber + 1}.dat";

					var response = await http.GetAsync(url, cancellationToken);

					// If we get a 404, there are no newer batch files available to download
					if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
						break;

					response.EnsureSuccessStatusCode();

					// Skip batch files which are older than 14 days
					if (response.Content.Headers.LastModified.HasValue && response.Content.Headers.LastModified < rightNow.AddDays(-14))
					{
						// If the first one is too old, the fake download it and pretend there was only one
						// We can do this because each batch was created at the same time
						batchNumber++;
						break;
					}

					var tmpFile = Path.Combine(FileSystem.CacheDirectory, Guid.NewGuid().ToString() + ".zip");

					// Read the batch file stream into a temporary file
					using var responseStream = await response.Content.ReadAsStreamAsync();
					using var fileStream = File.Create(tmpFile);
					await responseStream.CopyToAsync(fileStream, cancellationToken);

					downloadedFiles.Add(tmpFile);

					batchNumber++;
				}

				return (batchNumber, downloadedFiles);
			}
		}

		// this will be called when the user is submitting a diagnosis and the local keys need to go to the server
		public async Task UploadSelfExposureKeysToServerAsync(IEnumerable<TemporaryExposureKey> temporaryExposureKeys)
		{
			var pendingDiagnosis = LocalStateManager.Instance.PendingDiagnosis;

			if (pendingDiagnosis == null || string.IsNullOrEmpty(pendingDiagnosis.DiagnosisUid))
				throw new InvalidOperationException();

			var selfDiag = await CreateSubmissionAsync();

			var url = $"{AppSettings.Instance.ApiUrlBase.TrimEnd('/')}/selfdiagnosis";

			var json = JsonConvert.SerializeObject(selfDiag);

			using var http = new HttpClient();
			var response = await http.PutAsync(url, new StringContent(json));

			response.EnsureSuccessStatusCode();

			// Update pending status
			pendingDiagnosis.Shared = true;
			LocalStateManager.Save();

			async Task<SelfDiagnosisSubmission> CreateSubmissionAsync()
			{
				// Create the network keys
				var keys = temporaryExposureKeys.Select(k => new ExposureKey
				{
					Key = Convert.ToBase64String(k.Key),
					RollingStart = (long)(k.RollingStart - DateTime.UnixEpoch).TotalMinutes / 10,
					RollingDuration = (int)(k.RollingDuration.TotalMinutes / 10),
					TransmissionRisk = (int)k.TransmissionRiskLevel
				});

				// Create the submission
				var submission = new SelfDiagnosisSubmission(true)
				{
					AppPackageName = AppInfo.PackageName,
					DeviceVerificationPayload = null,
					Platform = DeviceInfo.Platform.ToString().ToLowerInvariant(),
					Regions = AppSettings.Instance.SupportedRegions,
					Keys = keys.ToArray(),
					VerificationPayload = pendingDiagnosis.DiagnosisUid,
				};

				// See if we can add the device verification
				if (DependencyService.Get<IDeviceVerifier>() is IDeviceVerifier verifier)
					submission.DeviceVerificationPayload = await verifier?.VerifyAsync(submission);

				return submission;
			}
		}
	}
}
