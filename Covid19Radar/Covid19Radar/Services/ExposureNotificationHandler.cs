using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Services;
using Newtonsoft.Json;
using Plugin.LocalNotification;
using Prism.Ioc;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Covid19Radar.Services
{
    [Preserve] // Ensure this isn't linked out
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

            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = "Possible COVID-19 Exposure",
                Description = "It is possible you have been exposed to someone who was a confirmed diagnosis of COVID-19.  Tap for more details."
            };

            NotificationCenter.Current.Show(notification);
        }

        // this will be called when they keys need to be collected from the server
        public async Task FetchExposureKeysFromServerAsync(ITemporaryExposureKeyBatches batches)
        {
            // This is "default" by default
            var region = LocalStateManager.Instance.Region ?? AppConstants.DefaultRegion;

            var checkForMore = true;
            do
            {
                try
                {
                    // Find next batch number
                    var batchNumber = LocalStateManager.Instance.ServerBatchNumber + 1;

                    // Build the blob storage url for the given batch file we are on next
                    var url = $"{AppConstants.ApiUrlBlobStorageBase}/{AppConstants.BlobStorageContainerNamePrefix}{region}/{batchNumber}.dat";

                    var response = await http.GetAsync(url);

                    // If we get a 404, there are no newer batch files available to download
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        checkForMore = false;
                        break;
                    }

                    response.EnsureSuccessStatusCode();

                    // Skip batch files which are older than 14 days
                    if (response.Content.Headers.LastModified.HasValue)
                    {
                        if (response.Content.Headers.LastModified < DateTimeOffset.UtcNow.AddDays(-14))
                        {
                            LocalStateManager.Instance.ServerBatchNumber = batchNumber;
                            LocalStateManager.Save();
                            checkForMore = true;
                            continue;
                        }
                    }

                    // Read the batch file stream
                    using var responseStream = await response.Content.ReadAsStreamAsync();

                    // Parse into a Proto.File
                    var batchFile = TemporaryExposureKeyBatch.Parser.ParseFrom(responseStream);

                    // Submit to the batch processor
                    await batches.AddBatchAsync(batchFile);

                    // Update the number we are on
                    LocalStateManager.Instance.ServerBatchNumber = batchNumber;
                    LocalStateManager.Save();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    checkForMore = false;
                }
            } while (checkForMore);
        }

        // this will be called when the user is submitting a diagnosis and the local keys need to go to the server
        public async Task UploadSelfExposureKeysToServerAsync(IEnumerable<TemporaryExposureKey> temporaryExposureKeys)
        {
            var pendingDiagnosis = LocalStateManager.Instance.PendingDiagnosis;

            if (pendingDiagnosis == null || string.IsNullOrEmpty(pendingDiagnosis.DiagnosisUid))
                throw new InvalidOperationException();

            try
            {
                //var url = $"{apiUrlBase.TrimEnd('/')}/selfdiagnosis";
                var url = $"{AppConstants.ApiUrlBase.TrimEnd('/')}/diagnosis";

                //var json = JsonConvert.SerializeObject(new SelfDiagnosisSubmissionRequest
                //{
                //    DiagnosisUid = pendingDiagnosis.DiagnosisUid,
                //    TestDate = pendingDiagnosis.DiagnosisDate.ToUnixTimeMilliseconds(),
                //    Keys = temporaryExposureKeys
                //});

                UserDataService userDataService = App.Current.Container.Resolve<UserDataService>();
                var request = new DiagnosisSubmissionRequest()
                {
                    SubmissionNumber = pendingDiagnosis.DiagnosisUid,
                    AppPackageName = Xamarin.Essentials.AppInfo.PackageName, // experimental
                    UserUuid = userDataService.Get().UserUuid,
                    Region = LocalStateManager.Instance.Region ?? AppConstants.DefaultRegion,
                    Platform = Device.RuntimePlatform.ToLower(),
                    Keys = temporaryExposureKeys.Select(_ => DiagnosisSubmissionRequest.Key.FromTemporaryExposureKey(_)).ToArray(),
                    DeviceVerificationPayload = "" // TODO: device payload
                };

                var http = new HttpClient();
                var response = await http.PutAsync(url, new StringContent(JsonConvert.SerializeObject(request)));

                response.EnsureSuccessStatusCode();

                // Update pending status
                pendingDiagnosis.Shared = true;
                LocalStateManager.Save();
            }
            catch
            {
                throw;
            }
        }

        internal static async Task<bool> VerifyDiagnosisUid(string diagnosisUid)
        {
            var url = $"{AppConstants.ApiUrlBase.TrimEnd('/')}/selfdiagnosis";

            var http = new HttpClient();

            try
            {
                var json = "{\"diagnosisUid\":\"" + diagnosisUid + "\"}";
                var response = await http.PostAsync(url, new StringContent(json));

                response.EnsureSuccessStatusCode();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //class SelfDiagnosisSubmissionRequest
        //{
        //    [JsonProperty("diagnosisUid")]
        //    public string DiagnosisUid { get; set; }

        //    [JsonProperty("testDate")]
        //    public long TestDate { get; set; }

        //    [JsonProperty("keys")]
        //    public IEnumerable<TemporaryExposureKey> Keys { get; set; }
        //}

        public class DiagnosisSubmissionRequest
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
                [JsonProperty("transmissionRisk")]
                public int TransmissionRisk { get; set; }
                public static Key FromTemporaryExposureKey(TemporaryExposureKey key)
                {
                    return new Key()
                    {
                        KeyData = Convert.ToBase64String(key.KeyData),
                        RollingStartNumber = (uint)(key.RollingStart.ToUnixTimeSeconds() / 60 / 10),
                        RollingPeriod = (uint)(key.RollingDuration.TotalSeconds / 60 / 10),
                        TransmissionRisk = (int)key.TransmissionRiskLevel
                    };
                }
            }
        }
    }
}