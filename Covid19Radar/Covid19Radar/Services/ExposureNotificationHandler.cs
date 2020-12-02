using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
//using Plugin.LocalNotification;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    [Xamarin.Forms.Internals.Preserve] // Ensure this isn't linked out
    public class ExposureNotificationHandler : IExposureNotificationHandler
    {
        private readonly ILoggerService loggerService;
        private readonly IHttpDataService httpDataService;
        private readonly UserDataService userDataService;
        private UserDataModel userData;
        private Configuration configuration;

        public ExposureNotificationHandler()
        {
            loggerService = DependencyService.Resolve<ILoggerService>();
            this.httpDataService = Xamarin.Forms.DependencyService.Resolve<IHttpDataService>();
            this.userDataService = Xamarin.Forms.DependencyService.Resolve<UserDataService>();
            userData = this.userDataService.Get();
            userDataService.UserDataChanged += (s, e) =>
            {
                userData = userDataService.Get();
                loggerService.Info("Updated user data.");
            };
            loggerService.Info($"userData is {(userData == null ? "null" : "set")}.");
        }

        // this string should be localized
        public string UserExplanation
            => AppResources.LocalNotificationDescription;

        // this configuration should be obtained from a server and it should be cached locally/in memory as it may be called multiple times
        public Task<Configuration> GetConfigurationAsync()
        {
            loggerService.StartMethod();

            if (Application.Current.Properties.ContainsKey("ExposureNotificationConfigration"))
            {
                loggerService.Info("Get configuration from config");

                var configurationJson = Application.Current.Properties["ExposureNotificationConfigration"].ToString();
                loggerService.Info($"configuration: {configurationJson}");

                loggerService.EndMethod();
                return Task.FromResult(Utils.DeserializeFromJson<Configuration>(configurationJson));
            }

            configuration = new Configuration
            {
                MinimumRiskScore = 21,
                AttenuationWeight = 50,
                TransmissionWeight = 50,
                DurationWeight = 50,
                DaysSinceLastExposureWeight = 50,
                TransmissionRiskScores = new int[] { 7, 7, 7, 7, 7, 7, 7, 7 },
                AttenuationScores = new[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                DurationScores = new[] { 0, 0, 0, 0, 1, 1, 1, 1 },
                DaysSinceLastExposureScores = new[] { 1, 1, 1, 1, 1, 1, 1, 1 },
                DurationAtAttenuationThresholds = new[] { 50, 70 }
            };

            loggerService.Info("Get default configuration");

            var defaultConfiguration = Task.FromResult(configuration);
            loggerService.Info($"configuration: {Utils.SerializeToJson(configuration)}");

            loggerService.EndMethod();
            return defaultConfiguration;
        }

        // this will be called when a potential exposure has been detected
        public async Task ExposureDetectedAsync(ExposureDetectionSummary summary, Func<Task<IEnumerable<ExposureInfo>>> getExposureInfo)
        {
            loggerService.StartMethod();

            UserExposureSummary userExposureSummary = new UserExposureSummary(summary.DaysSinceLastExposure, summary.MatchedKeyCount, summary.HighestRiskScore, summary.AttenuationDurations, summary.SummationRiskScore);
            userData.ExposureSummary = userExposureSummary;

            loggerService.Info($"ExposureSummary.MatchedKeyCount: {userExposureSummary.MatchedKeyCount}");
            loggerService.Info($"ExposureSummary.DaysSinceLastExposure: {userExposureSummary.DaysSinceLastExposure}");
            loggerService.Info($"ExposureSummary.HighestRiskScore: {userExposureSummary.HighestRiskScore}");
            loggerService.Info($"ExposureSummary.AttenuationDurations: {string.Join(",", userExposureSummary.AttenuationDurations)}");
            loggerService.Info($"ExposureSummary.SummationRiskScore: {userExposureSummary.SummationRiskScore}");

            var config = await GetConfigurationAsync();

            if (userData.ExposureSummary.HighestRiskScore >= config.MinimumRiskScore)
            {
                var exposureInfo = await getExposureInfo();
                loggerService.Info($"ExposureInfo: {exposureInfo.Count()}");

                // Add these on main thread in case the UI is visible so it can update
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    foreach (var exposure in exposureInfo)
                    {
                        loggerService.Info($"Exposure.Timestamp: {exposure.Timestamp}");
                        loggerService.Info($"Exposure.Duration: {exposure.Duration}");
                        loggerService.Info($"Exposure.AttenuationValue: {exposure.AttenuationValue}");
                        loggerService.Info($"Exposure.TotalRiskScore: {exposure.TotalRiskScore}");
                        loggerService.Info($"Exposure.TransmissionRiskLevel: {exposure.TransmissionRiskLevel}");

                        UserExposureInfo userExposureInfo = new UserExposureInfo(exposure.Timestamp, exposure.Duration, exposure.AttenuationValue, exposure.TotalRiskScore, (Covid19Radar.Model.UserRiskLevel)exposure.TransmissionRiskLevel);
                        userData.ExposureInformation.Add(userExposureInfo);
                    }
                });
            }

            loggerService.Info($"Save ExposureSummary. MatchedKeyCount: {userData.ExposureSummary.MatchedKeyCount}");
            loggerService.Info($"Save ExposureInformation. Count: {userData.ExposureInformation.Count}");
            await userDataService.SetAsync(userData);

            // If Enabled Local Notifications
            //if (userData.IsNotificationEnabled)
            //{
            //    var notification = new NotificationRequest
            //    {
            //        NotificationId = 100,
            //        Title = AppResources.LocalNotificationTitle,
            //        Description = AppResources.LocalNotificationDescription
            //    };

            //    NotificationCenter.Current.Show(notification);
            //}

            loggerService.EndMethod();
        }

        // this will be called when they keys need to be collected from the server
        public async Task FetchExposureKeyBatchFilesFromServerAsync(Func<IEnumerable<string>, Task> submitBatches, CancellationToken cancellationToken)
        {
            loggerService.StartMethod();
            // This is "default" by default
            var rightNow = DateTimeOffset.UtcNow;

            try
            {
                foreach (var serverRegion in AppSettings.Instance.SupportedRegions)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    loggerService.Info("Start download files");
                    var (batchNumber, downloadedFiles) = await DownloadBatchAsync(serverRegion, cancellationToken);
                    loggerService.Info("End to download files");
                    loggerService.Info($"Batch number: {batchNumber}, Downloaded files: {downloadedFiles.Count}");

                    if (batchNumber == 0)
                    {
                        continue;
                    }

                    if (downloadedFiles.Count > 0)
                    {
                        loggerService.Info("C19R Submit Batches");
                        await submitBatches(downloadedFiles);

                        // delete all temporary files
                        foreach (var file in downloadedFiles)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch (Exception ex)
                            {
                                // no-op
                                loggerService.Exception("Fail to delete downloaded files", ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // any expections, bail out and wait for the next time
                loggerService.Exception("Fail to download files", ex);
            }
            loggerService.EndMethod();
        }

        private async Task<(int, List<string>)> DownloadBatchAsync(string region, CancellationToken cancellationToken)
        {
            loggerService.StartMethod();

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
            catch (Exception ex)
            {
                loggerService.Exception("Failed to create directory", ex);
                loggerService.EndMethod();
                // catch error return batchnumber 0 / fileList 0
                return (batchNumber, downloadedFiles);
            }

            //long sinceEpochSeconds = new DateTimeOffset(DateTime.UtcNow.AddDays(-14)).ToUnixTimeSeconds();
            List<TemporaryExposureKeyExportFileModel> tekList = await httpDataService.GetTemporaryExposureKeyList(region, cancellationToken);
            if (tekList.Count == 0)
            {
                loggerService.EndMethod();
                return (batchNumber, downloadedFiles);
            }
            Debug.WriteLine("C19R Fetch Exposure Key");

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

                loggerService.Info($"tekItem.Created: {tekItem.Created}");
                if (tekItem.Created > lastCreated || lastCreated == 0)
                {
                    var tmpFile = Path.Combine(tmpDir, Guid.NewGuid().ToString() + ".zip");
                    Debug.WriteLine(Utils.SerializeToJson(tekItem));
                    Debug.WriteLine(tmpFile);

                    loggerService.Info($"Download TEK file. url: {tekItem.Url}");
                    using (Stream responseStream = await httpDataService.GetTemporaryExposureKey(tekItem.Url, cancellationToken))
                    using (var fileStream = File.Create(tmpFile))
                    {
                        try
                        {
                            await responseStream.CopyToAsync(fileStream, cancellationToken);
                            fileStream.Flush();
                        }
                        catch (Exception ex)
                        {
                            loggerService.Exception("Fail to copy", ex);
                        }
                    }
                    lastTekTimestamp[region] = tekItem.Created;
                    downloadedFiles.Add(tmpFile);
                    Debug.WriteLine($"C19R FETCH DIAGKEY {tmpFile}");
                    batchNumber++;
                }
            }
            loggerService.Info($"Batch number: {batchNumber}, Downloaded files: {downloadedFiles.Count()}");
            userData.LastProcessTekTimestamp = lastTekTimestamp;
            await userDataService.SetAsync(userData);
            loggerService.Info($"region: {region}, userData.LastProcessTekTimestamp[{region}]: {userData.LastProcessTekTimestamp[region]}");

            loggerService.EndMethod();

            return (batchNumber, downloadedFiles);
        }

        // this will be called when the user is submitting a diagnosis and the local keys need to go to the server
        public async Task UploadSelfExposureKeysToServerAsync(IEnumerable<TemporaryExposureKey> temporaryExposureKeys)
        {
            loggerService.StartMethod();

            loggerService.Info($"TemporaryExposureKey count: {temporaryExposureKeys.Count()}");
            foreach (var temporaryExposureKey in temporaryExposureKeys)
            {
                loggerService.Info($"TemporaryExposureKey: RollingStart: {temporaryExposureKey.RollingStart}({temporaryExposureKey.RollingStart.ToUnixTimeSeconds()}), RollingDuration: {temporaryExposureKey.RollingDuration}, TransmissionRiskLevel: {temporaryExposureKey.TransmissionRiskLevel}");
            }

            var latestDiagnosis = userData.LatestDiagnosis;

            if (latestDiagnosis == null || string.IsNullOrEmpty(latestDiagnosis.DiagnosisUid))
            {
                loggerService.Error($"Diagnostic number is null or empty.");
                loggerService.EndMethod();
                throw new InvalidOperationException();
            }

            var selfDiag = await CreateSubmissionAsync(temporaryExposureKeys, latestDiagnosis);
            HttpStatusCode httpStatusCode = await httpDataService.PutSelfExposureKeysAsync(selfDiag);
            loggerService.Info($"HTTP status is {httpStatusCode}({(int)httpStatusCode}).");

            if (httpStatusCode == HttpStatusCode.NotAcceptable)
            {
                await UserDialogs.Instance.AlertAsync(
                    "",
                    AppResources.ExposureNotificationHandler1ErrorMessage,
                    Resources.AppResources.ButtonOk);

                loggerService.Error($"The diagnostic number is incorrect.");
                loggerService.EndMethod();
                throw new InvalidOperationException();
            }
            else if (httpStatusCode == HttpStatusCode.ServiceUnavailable || httpStatusCode == HttpStatusCode.InternalServerError)
            {
                await UserDialogs.Instance.AlertAsync(
                    "",
                    AppResources.ExposureNotificationHandler2ErrorMessage,
                    Resources.AppResources.ButtonOk);

                loggerService.Error($"Cannot connect to the center.");
                loggerService.EndMethod();
                throw new InvalidOperationException();
            }
            else if (httpStatusCode == HttpStatusCode.BadRequest)
            {
                await UserDialogs.Instance.AlertAsync(
                    "",
                    AppResources.ExposureNotificationHandler3ErrorMessage,
                    Resources.AppResources.ButtonOk);

                loggerService.Error($"There is a problem with the record data.");
                loggerService.EndMethod();
                throw new InvalidOperationException();
            }

            await userDataService.SetAsync(userData);

            loggerService.EndMethod();
        }


        private async Task<DiagnosisSubmissionParameter> CreateSubmissionAsync(IEnumerable<TemporaryExposureKey> temporaryExposureKeys, PositiveDiagnosisState pendingDiagnosis)
        {
            loggerService.StartMethod();

            // Create the network keys
            var keys = temporaryExposureKeys.Where(k => k.RollingStart > DateTimeOffset.UtcNow.Date.AddDays(AppConstants.OutOfDateDays)).Select(k => new DiagnosisSubmissionParameter.Key
            {
                KeyData = Convert.ToBase64String(k.Key),
                RollingStartNumber = (uint)(k.RollingStart - DateTime.UnixEpoch).TotalMinutes / 10,
                RollingPeriod = (uint)(k.RollingDuration.TotalMinutes / 10),
                TransmissionRisk = (int)k.TransmissionRiskLevel
            });

            var beforeKey = Utils.SerializeToJson(temporaryExposureKeys.ToList());
            var afterKey = Utils.SerializeToJson(keys.ToList());
            Debug.WriteLine($"C19R {beforeKey}");
            Debug.WriteLine($"C19R {afterKey}");

            if (keys.Count() == 0)
            {
                loggerService.Error($"Temporary exposure keys is empty.");
                loggerService.EndMethod();
                throw new InvalidDataException();
            }

            // Generate Padding
            var padding = GetPadding();

            loggerService.Info($"userData is {(userData == null ? "" : "not ")}null or empty.");

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

            loggerService.Info($"UserUuid is {(string.IsNullOrEmpty(submission.UserUuid) ? "null or empty" : "set")}.");
            loggerService.Info($"DeviceVerificationPayload is {(string.IsNullOrEmpty(submission.DeviceVerificationPayload) ? "null or empty" : "set")}.");
            loggerService.Info($"VerificationPayload is {(string.IsNullOrEmpty(submission.VerificationPayload) ? "null or empty" : "set")}.");

            loggerService.EndMethod();
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
