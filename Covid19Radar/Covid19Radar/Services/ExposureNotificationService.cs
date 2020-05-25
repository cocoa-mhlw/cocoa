using Covid19Radar.Common;
using Covid19Radar.Model;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    class ExposureNotificationService : IExposureNotificationHandler
    {
        private readonly HttpDataService httpDataService;
        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public ExposureNotificationService(HttpDataService httpDataService, UserDataService userDataService)
        {
            this.httpDataService = httpDataService;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
            this.userDataService.UserDataChanged += _userDataChanged;
        }

        private void _userDataChanged(object sender, UserDataModel e)
        {
            userData = this.userDataService.Get();
        }


        #region Exposure Notification

        public string UserExplanation
            => "We need to make use of the keys to keep you healthy.";

        // this configuration should be obtained from a server and it should be cached locally/in memory as it may be called multiple times
        public Task<Configuration> GetConfigurationAsync()
    => Task.FromResult(new Configuration());


        public async Task FetchExposureKeyBatchFilesFromServerAsync(Func<IEnumerable<string>, Task> submitBatches)
        {
            try
            {
                var downloadedFiles = new List<string>();

                var resonseContentResult = await httpDataService.GetTemporaryExposureKeys(userData.ServerLastTime);
                foreach (var key in resonseContentResult.Keys)
                {
                    var tmpFile = Path.Combine(FileSystem.CacheDirectory, Guid.NewGuid().ToString() + ".zip");
                    if (await httpDataService.GetFileAsync(key.Url, tmpFile))
                    {
                        downloadedFiles.Add(tmpFile);
                    }
                }

                // process the current directory, if there were any
                if (downloadedFiles.Count > 0)
                {
                    // TODO: waiting release new nuget package
                    //await submitBatches(downloadedFiles);

                    // delete all temporary files
                    foreach (var file in downloadedFiles)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch { }
                    }
                }

                userData.ServerLastTime = resonseContentResult.Timestamp;
                await userDataService.SetAsync(userData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        public async Task ExposureDetectedAsync(ExposureDetectionSummary summary, IEnumerable<ExposureInfo> exposureInfo)
        {
            userData.ExposureSummary = summary;

            // Add these on main thread in case the UI is visible so it can update
            await Device.InvokeOnMainThreadAsync(() =>
            {
                foreach (var i in exposureInfo)
                    userData.ExposureInformation.Add(i);
            });

            await userDataService.SetAsync(userData);

            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = "Possible COVID-19 Exposure",
                Description = "It is possible you have been exposed to someone who was a confirmed diagnosis of COVID-19.  Tap for more details."
            };

            NotificationCenter.Current.Show(notification);
        }

        public async Task UploadSelfExposureKeysToServerAsync(IEnumerable<TemporaryExposureKey> temporaryExposureKeys)
        {
            if (userData.PendingDiagnosis == null || string.IsNullOrEmpty(userData.PendingDiagnosis.DiagnosisUid))
                throw new InvalidOperationException();

            try
            {
                var request = new DiagnosisSubmissionHttpRequestModel()
                {
                    SubmissionNumber = userData.PendingDiagnosis.DiagnosisUid,
                    AppPackageName = Xamarin.Essentials.AppInfo.PackageName, // experimental
                    UserUuid = userData.UserUuid,
                    Region = userData.Region ?? AppConstants.DefaultRegion,
                    Platform = Device.RuntimePlatform.ToLower(),
                    Keys = temporaryExposureKeys.Select(_ => DiagnosisSubmissionHttpRequestModel.Key.FromTemporaryExposureKey(_)).ToArray(),
                    DeviceVerificationPayload = "" // TODO: device payload
                };

                // TODO check implementation
                await httpDataService.PostSelfExposureKeysAsync(request);

                // Update pending status
                userData.PendingDiagnosis.Shared = true;
                await userDataService.SetAsync(userData);
            }
            catch
            {
                throw;
            }
        }
        #endregion

    }
}
