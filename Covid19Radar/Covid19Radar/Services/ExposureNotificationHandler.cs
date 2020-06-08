using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Newtonsoft.Json;
using Plugin.LocalNotification;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    [Xamarin.Forms.Internals.Preserve] // Ensure this isn't linked out
    public class ExposureNotificationHandler : IExposureNotificationHandler
    {
        private readonly HttpDataService httpDataService;
        private readonly UserDataService userDataService;
        private readonly ExposureNotificationService exposureNotificationService;
        private UserDataModel userData;
        private readonly Configuration configuration;

        public ExposureNotificationHandler()
        {
            this.httpDataService = Xamarin.Forms.DependencyService.Resolve<HttpDataService>();
            this.userDataService = Xamarin.Forms.DependencyService.Resolve<UserDataService>();
            userData = this.userDataService.Get();
            this.exposureNotificationService = Xamarin.Forms.DependencyService.Resolve<ExposureNotificationService>();
            configuration = exposureNotificationService.GetConfigration();
        }

        // this string should be localized
        public string UserExplanation
            => "We need to make use of the keys to keep you healthy.";

        // this configuration should be obtained from a server and it should be cached locally/in memory as it may be called multiple times
        public Task<Configuration> GetConfigurationAsync()
            => Task.FromResult(configuration);

        // this will be called when a potential exposure has been detected
        public async Task ExposureDetectedAsync(ExposureDetectionSummary summary, Func<Task<IEnumerable<ExposureInfo>>> getExposureInfo)
        {
            userData.ExposureSummary = summary;

            var exposureInfo = await getExposureInfo();

            // Add these on main thread in case the UI is visible so it can update
            await Device.InvokeOnMainThreadAsync(() =>
            {
                foreach (var i in exposureInfo)
                    userData.ExposureInformation.Add(i);
            });

            await userDataService.SetAsync(userData);
            // If Enabled Local Notifications
            if (userData.IsNotificationEnabled)
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
                    var dirNumber = userData.ServerBatchNumbers[serverRegion] + 1;

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
                        userData.ServerBatchNumbers[serverRegion] = dirNumber;
                        await userDataService.SetAsync(userData);

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

                long sinceEpochSeconds = new DateTimeOffset(DateTime.UtcNow.AddDays(-14)).ToUnixTimeSeconds();
                TemporaryExposureKeysResult tekResult = await httpDataService.GetTemporaryExposureKeys(region , sinceEpochSeconds, cancellationToken);
                Console.WriteLine("Fetch Exposure Key");

                foreach (var key in tekResult.Keys)
                {
                    // TODO 取得TimeStamp差分実装を行う
                    cancellationToken.ThrowIfCancellationRequested();
                    var tmpFile = Path.Combine(FileSystem.CacheDirectory, Guid.NewGuid().ToString() + ".zip");

                    // Read the batch file stream into a temporary file
                    Console.WriteLine(key.Url);
                    Console.WriteLine(tmpFile);
                    Stream responseStream = await httpDataService.GetTemporaryExposureKey(key.Url, cancellationToken);
                    var fileStream = File.Create(tmpFile);
                    try
                    {
                        await responseStream.CopyToAsync(fileStream, cancellationToken);
                        fileStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    downloadedFiles.Add(tmpFile);
                    batchNumber++;
                }
                Console.WriteLine(batchNumber.ToString());
                Console.WriteLine(downloadedFiles.Count());
                return (batchNumber, downloadedFiles);
            }
        }

        // this will be called when the user is submitting a diagnosis and the local keys need to go to the server
        public async Task UploadSelfExposureKeysToServerAsync(IEnumerable<TemporaryExposureKey> temporaryExposureKeys)
        {
            var pendingDiagnosis = userData.PendingDiagnosis;

            if (pendingDiagnosis == null || string.IsNullOrEmpty(pendingDiagnosis.DiagnosisUid))
            {
                throw new InvalidOperationException();
            }

            var selfDiag = await CreateSubmissionAsync(temporaryExposureKeys, pendingDiagnosis);
            await httpDataService.PutSelfExposureKeysAsync(selfDiag);
            // Update pending status
            pendingDiagnosis.Shared = true;
            await userDataService.SetAsync(userData);
        }


        private async Task<SelfDiagnosisSubmission> CreateSubmissionAsync(IEnumerable<TemporaryExposureKey> temporaryExposureKeys, PositiveDiagnosisState pendingDiagnosis)
        {
            // Create the network keys
            var keys = temporaryExposureKeys.Select(k => new ExposureKey
            {
                KeyData = Convert.ToBase64String(k.Key),
                RollingStart = (long)(k.RollingStart - DateTime.UnixEpoch).TotalMinutes / 10,
                RollingDuration = (int)(k.RollingDuration.TotalMinutes / 10),
                TransmissionRisk = (int)k.TransmissionRiskLevel
            });

            // Create the submission
            var submission = new SelfDiagnosisSubmission(true)
            {
                SubmissionNumber = userData.PendingDiagnosis.DiagnosisUid,
                AppPackageName = AppInfo.PackageName,
                UserUuid = userData.UserUuid,
                DeviceVerificationPayload = null,
                Platform = DeviceInfo.Platform.ToString().ToLowerInvariant(),
                Regions = AppSettings.Instance.SupportedRegions,
                Keys = keys.ToArray(),
                VerificationPayload = pendingDiagnosis.DiagnosisUid,
            };

            // See if we can add the device verification
            if (DependencyService.Get<IDeviceVerifier>() is IDeviceVerifier verifier)
            {
                submission.DeviceVerificationPayload = await verifier?.VerifyAsync(submission);
            }
            return submission;

        }
    }
}
