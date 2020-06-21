using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Resources;
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
        private UserDataModel userData;
        private Configuration configuration;

        public ExposureNotificationHandler()
        {
            this.httpDataService = Xamarin.Forms.DependencyService.Resolve<HttpDataService>();
            this.userDataService = Xamarin.Forms.DependencyService.Resolve<UserDataService>();
            userData = this.userDataService.Get();
        }

        // this string should be localized
        public string UserExplanation
            => "We need to make use of the keys to keep you healthy.";

        // this configuration should be obtained from a server and it should be cached locally/in memory as it may be called multiple times
        public Task<Configuration> GetConfigurationAsync()
        {
            //=> Task.FromResult(configuration);
            if (Application.Current.Properties.ContainsKey("ExposureNotificationConfigration"))
            {
                return Task.FromResult(Utils.DeserializeFromJson<Configuration>(Application.Current.Properties["ExposureNotificationConfigration"].ToString()));
            }

            configuration = new Configuration
            {
                MinimumRiskScore = 1,
                AttenuationWeight = 50,
                TransmissionWeight = 50,
                DurationWeight = 50,
                DaysSinceLastExposureWeight = 50,
                TransmissionRiskScores = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                AttenuationScores = new[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                DurationScores = new[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                DaysSinceLastExposureScores = new[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                DurationAtAttenuationThresholds = new[] { 50, 70 }
            };

            return Task.FromResult(configuration);


        }

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
                    Title = AppResources.LocalNotificationTitle,
                    Description = AppResources.LocalNotificationDescription
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
                    cancellationToken.ThrowIfCancellationRequested();

                    var (batchNumber, downloadedFiles) = await DownloadBatchAsync(serverRegion, cancellationToken);
                    if (batchNumber == 0)
                    {
                        return;
                    }

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
                }
            }
            catch (Exception ex)
            {
                // any expections, bail out and wait for the next time
                Console.WriteLine(ex);
            }
        }

        private async Task<(int, List<string>)> DownloadBatchAsync(string region, CancellationToken cancellationToken)
        {
            var downloadedFiles = new List<string>();
            var batchNumber = 0;
            var tmpDir = Path.Combine(FileSystem.CacheDirectory, region);

            try
            {
                if (!Directory.Exists(tmpDir))
                {
                    Directory.CreateDirectory(tmpDir);
                }
            }
            catch
            {
                // catch error return batchnumber 0 / fileList 0
                return (batchNumber, downloadedFiles);
            }

            long sinceEpochSeconds = new DateTimeOffset(DateTime.UtcNow.AddDays(-14)).ToUnixTimeSeconds();
            List<TemporaryExposureKeyExportFileModel> tekList = await httpDataService.GetTemporaryExposureKeyList(region, cancellationToken);
            if (tekList.Count == 0)
            {
                return (batchNumber, downloadedFiles);
            }
            Console.WriteLine("Fetch Exposure Key");

            Dictionary<string, long> lastTekTimestamp = userData.LastProcessTekTimestamp;

            foreach (var tekItem in tekList)
            {
                long lastCreated = 0;
                if (lastTekTimestamp.ContainsKey(region))
                {
                    lastCreated = lastTekTimestamp[region];
                }
                else
                {
                    lastTekTimestamp.Add(region, 0);
                }

                if (tekItem.Created > lastCreated || lastCreated == 0)
                {
                    var tmpFile = Path.Combine(tmpDir, Guid.NewGuid().ToString() + ".zip");
                    Console.WriteLine(Utils.SerializeToJson(tekItem));
                    Console.WriteLine(tmpFile);
                    Stream responseStream = await httpDataService.GetTemporaryExposureKey(tekItem.Url, cancellationToken);
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
                    lastTekTimestamp[region] = tekItem.Created;
                    downloadedFiles.Add(tmpFile);
                    batchNumber++;
                }
            }
            Console.WriteLine(batchNumber.ToString());
            Console.WriteLine(downloadedFiles.Count());
            userData.LastProcessTekTimestamp = lastTekTimestamp;
            await userDataService.SetAsync(userData);
            return (batchNumber, downloadedFiles);
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

            HttpStatusCode httpStatusCode = await httpDataService.PutSelfExposureKeysAsync(selfDiag);
            if (httpStatusCode == HttpStatusCode.NotAcceptable)
            {
                await UserDialogs.Instance.AlertAsync(
                    "",
                    AppResources.ExposureNotificationHandler1ErrorMessage,
                    Resources.AppResources.ButtonOk);
                throw new InvalidOperationException();
            }
            else if (httpStatusCode == HttpStatusCode.ServiceUnavailable || httpStatusCode == HttpStatusCode.InternalServerError)
            {
                await UserDialogs.Instance.AlertAsync(
                    "",
                    AppResources.ExposureNotificationHandler2ErrorMessage,
                    Resources.AppResources.ButtonOk);
                throw new InvalidOperationException();
            }
            // Update pending status
            pendingDiagnosis.Shared = true;
            await userDataService.SetAsync(userData);
        }


        private async Task<DiagnosisSubmissionParameter> CreateSubmissionAsync(IEnumerable<TemporaryExposureKey> temporaryExposureKeys, PositiveDiagnosisState pendingDiagnosis)
        {
            // Create the network keys
            var keys = temporaryExposureKeys.Select(k => new DiagnosisSubmissionParameter.Key
            {
                KeyData = Convert.ToBase64String(k.Key),
                RollingStartNumber = (uint)(k.RollingStart - DateTime.UnixEpoch).TotalMinutes / 10,
                RollingPeriod = (uint)(k.RollingDuration.TotalMinutes / 10),
                TransmissionRisk = (int)k.TransmissionRiskLevel
            });

            foreach (var key in keys)
            {
                if (!key.IsValid())
                {
                    throw new InvalidDataException();
                }
            }

            // Generate Padding
            var padding = GetPadding();

            // Create the submission
            var submission = new DiagnosisSubmissionParameter()
            {
                UserUuid = userData.UserUuid,
                Keys = keys.ToArray(),
                Regions = AppSettings.Instance.SupportedRegions,
                Platform = DeviceInfo.Platform.ToString().ToLowerInvariant(),
                DeviceVerificationPayload = null,
                AppPackageName = AppInfo.PackageName,
                VerificationPayload = pendingDiagnosis.DiagnosisUid,
                Padding = padding
            };

            // See if we can add the device verification
            if (DependencyService.Get<IDeviceVerifier>() is IDeviceVerifier verifier)
            {
                submission.DeviceVerificationPayload = await verifier?.VerifyAsync(submission);
            }
            return submission;

        }

        private string GetPadding()
        {
            var random = new Random();
            var size = random.Next(1024, 2048);

            // Approximate the base64 blowup.
            size = (int)(size * 0.75);

            var padding = new byte[size];
            random.NextBytes(padding);
            return Convert.ToBase64String(padding);
        }
    }
}
