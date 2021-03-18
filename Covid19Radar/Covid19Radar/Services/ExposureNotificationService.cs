using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    public interface IExposureNotificationService
    {
        Task MigrateFromUserData(UserDataModel userData);

        Configuration GetConfiguration();
        void RemoveConfiguration();

        long GetLastProcessTekTimestamp(string region);
        void SetLastProcessTekTimestamp(string region, long created);
        void RemoveLastProcessTekTimestamp();

        Task FetchExposureKeyAsync();

        List<UserExposureInfo> GetExposureInformationList();
        int GetExposureCount();
        void SetExposureInformation(UserExposureSummary summary, List<UserExposureInfo> informationList);
        void RemoveExposureInformation();

        Task<string> UpdateStatusMessageAsync();
        Task<bool> StartExposureNotification();
        Task<bool> StopExposureNotification();

        string PositiveDiagnosis { get; set; }
        DateTime? DiagnosisDate { get; set; }
        IEnumerable<TemporaryExposureKey> FliterTemporaryExposureKeys(IEnumerable<TemporaryExposureKey> temporaryExposureKeys);
    }

    public class ExposureNotificationService : IExposureNotificationService
    {
        private readonly ILoggerService loggerService;
        private readonly IHttpClientService httpClientService;
        private readonly ISecureStorageService secureStorageService;
        private readonly IPreferencesService preferencesService;
        private readonly IApplicationPropertyService applicationPropertyService;

        public string CurrentStatusMessage { get; set; } = "初期状態";
        public Status ExposureNotificationStatus { get; set; }

        public ExposureNotificationService(ILoggerService loggerService, IHttpClientService httpClientService, ISecureStorageService secureStorageService, IPreferencesService preferencesService, IApplicationPropertyService applicationPropertyService)
        {
            this.loggerService = loggerService;
            this.httpClientService = httpClientService;
            this.secureStorageService = secureStorageService;
            this.preferencesService = preferencesService;
            this.applicationPropertyService = applicationPropertyService;

            _ = GetExposureNotificationConfig();
        }

        public async Task MigrateFromUserData(UserDataModel userData)
        {
            loggerService.StartMethod();

            const string ConfigurationPropertyKey = "ExposureNotificationConfigration";

            if (userData.LastProcessTekTimestamp != null && userData.LastProcessTekTimestamp.Count > 0)
            {
                var stringValue = Utils.SerializeToJson(userData.LastProcessTekTimestamp);
                preferencesService.SetValue(PreferenceKey.LastProcessTekTimestamp, stringValue);
                userData.LastProcessTekTimestamp.Clear();
                loggerService.Info("Migrated LastProcessTekTimestamp");
            }

            if (applicationPropertyService.ContainsKey(ConfigurationPropertyKey))
            {
                var configuration = applicationPropertyService.GetProperties(ConfigurationPropertyKey) as string;
                if (!string.IsNullOrEmpty(configuration))
                {
                    preferencesService.SetValue(PreferenceKey.ExposureNotificationConfiguration, configuration);
                }
                await applicationPropertyService.RemoveAsync(ConfigurationPropertyKey);
                loggerService.Info("Migrated ExposureNotificationConfiguration");
            }

            if (userData.ExposureInformation != null)
            {
                secureStorageService.SetValue(PreferenceKey.ExposureInformation, JsonConvert.SerializeObject(userData.ExposureInformation));
                userData.ExposureInformation = null;
                loggerService.Info("Migrated ExposureInformation");
            }

            if (userData.ExposureSummary != null)
            {
                secureStorageService.SetValue(PreferenceKey.ExposureSummary, JsonConvert.SerializeObject(userData.ExposureSummary));
                userData.ExposureSummary = null;
                loggerService.Info("Migrated ExposureSummary");
            }

            loggerService.EndMethod();
        }

        private async Task GetExposureNotificationConfig()
        {
            loggerService.StartMethod();

            string container = AppSettings.Instance.BlobStorageContainerName;
            string url = AppSettings.Instance.CdnUrlBase + $"{container}/Configration.json";
            HttpClient httpClient = httpClientService.Create();
            Task<HttpResponseMessage> response = httpClient.GetAsync(url);
            HttpResponseMessage result = await response;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                loggerService.Info("Success to download configuration");
                var content = await result.Content.ReadAsStringAsync();
                preferencesService.SetValue(PreferenceKey.ExposureNotificationConfiguration, content);
            }
            else
            {
                loggerService.Error("Fail to download configuration");
            }

            loggerService.EndMethod();
        }

        public Configuration GetConfiguration()
        {
            loggerService.StartMethod();
            Configuration result = null;
            var configurationJson = preferencesService.GetValue<string>(PreferenceKey.ExposureNotificationConfiguration, null);
            if (!string.IsNullOrEmpty(configurationJson))
            {
                loggerService.Info($"configuration: {configurationJson}");
                result = JsonConvert.DeserializeObject<Configuration>(configurationJson);
            }
            loggerService.EndMethod();
            return result;
        }

        public void RemoveConfiguration()
        {
            loggerService.StartMethod();
            preferencesService.RemoveValue(PreferenceKey.ExposureNotificationConfiguration);
            loggerService.EndMethod();
        }

        public async Task FetchExposureKeyAsync()
        {
            loggerService.StartMethod();
            await ExposureNotification.UpdateKeysFromServer();
            loggerService.EndMethod();
        }

        public long GetLastProcessTekTimestamp(string region)
        {
            loggerService.StartMethod();
            var result = 0L;
            var jsonString = preferencesService.GetValue<string>(PreferenceKey.LastProcessTekTimestamp, null);
            if (!string.IsNullOrEmpty(jsonString))
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, long>>(jsonString);
                if (dict.ContainsKey(region))
                {
                    result = dict[region];
                }
            }
            loggerService.EndMethod();
            return result;
        }

        public void SetLastProcessTekTimestamp(string region, long created)
        {
            loggerService.StartMethod();
            var jsonString = preferencesService.GetValue<string>(PreferenceKey.LastProcessTekTimestamp, null);
            Dictionary<string, long> newDict;
            if (!string.IsNullOrEmpty(jsonString))
            {
                newDict = JsonConvert.DeserializeObject<Dictionary<string, long>>(jsonString);
            }
            else
            {
                newDict = new Dictionary<string, long>();
            }
            newDict[region] = created;
            preferencesService.SetValue(PreferenceKey.LastProcessTekTimestamp, JsonConvert.SerializeObject(newDict));
            loggerService.EndMethod();
        }

        public void RemoveLastProcessTekTimestamp()
        {
            loggerService.StartMethod();
            preferencesService.RemoveValue(PreferenceKey.LastProcessTekTimestamp);
            loggerService.EndMethod();
        }

        public List<UserExposureInfo> GetExposureInformationList()
        {
            loggerService.StartMethod();
            List<UserExposureInfo> result = null;
            var exposureInformationJson = secureStorageService.GetValue<string>(PreferenceKey.ExposureInformation);
            if (!string.IsNullOrEmpty(exposureInformationJson))
            {
                result = JsonConvert.DeserializeObject<List<UserExposureInfo>>(exposureInformationJson);
            }
            loggerService.EndMethod();
            return result;
        }

        public int GetExposureCount()
        {
            loggerService.StartMethod();
            int result = 0;
            var exposureInformationList = GetExposureInformationList();
            if (exposureInformationList != null)
            {
                result = exposureInformationList.Count;
            }
            loggerService.EndMethod();
            return result;
        }

        public void SetExposureInformation(UserExposureSummary summary, List<UserExposureInfo> informationList)
        {
            loggerService.StartMethod();
            var summaryJson = JsonConvert.SerializeObject(summary);
            var informationListJson = JsonConvert.SerializeObject(informationList);
            secureStorageService.SetValue(PreferenceKey.ExposureSummary, summaryJson);
            secureStorageService.SetValue(PreferenceKey.ExposureInformation, informationListJson);
            loggerService.EndMethod();
        }

        public void RemoveExposureInformation()
        {
            loggerService.StartMethod();
            secureStorageService.RemoveValue(PreferenceKey.ExposureSummary);
            secureStorageService.RemoveValue(PreferenceKey.ExposureInformation);
            loggerService.EndMethod();
        }

        public async Task<string> UpdateStatusMessageAsync()
        {
            loggerService.StartMethod();
            ExposureNotificationStatus = await ExposureNotification.GetStatusAsync();
            loggerService.EndMethod();
            return await GetStatusMessageAsync();
        }

        public async Task<bool> StartExposureNotification()
        {
            loggerService.StartMethod();
            try
            {
                var enabled = await ExposureNotification.IsEnabledAsync();
                if (!enabled)
                {
                    await ExposureNotification.StartAsync();
                }

                loggerService.EndMethod();
                return true;
            }
            catch (Exception ex)
            {
                loggerService.Exception("Error enabling notifications.", ex);
                loggerService.EndMethod();
                return false;
            }
            finally
            {

            }
        }

        public async Task<bool> StopExposureNotification()
        {
            loggerService.StartMethod();
            try
            {
                var enabled = await ExposureNotification.IsEnabledAsync();
                if (enabled)
                {
                    await ExposureNotification.StopAsync();
                }

                loggerService.EndMethod();
                return true;
            }
            catch (Exception ex)
            {
                loggerService.Exception("Error disabling notifications.", ex);
                loggerService.EndMethod();
                return false;
            }
        }

        private async Task<string> GetStatusMessageAsync()
        {
            var message = "";

            switch (ExposureNotificationStatus)
            {
                case Status.Unknown:
                    await UserDialogs.Instance.AlertAsync(Resources.AppResources.ExposureNotificationStatusMessageUnknown, "", Resources.AppResources.ButtonOk);
                    message = Resources.AppResources.ExposureNotificationStatusMessageUnknown;
                    break;
                case Status.Disabled:
                    await UserDialogs.Instance.AlertAsync(Resources.AppResources.ExposureNotificationStatusMessageDisabled, "", Resources.AppResources.ButtonOk);
                    message = Resources.AppResources.ExposureNotificationStatusMessageDisabled;
                    break;
                case Status.Active:
                    message = Resources.AppResources.ExposureNotificationStatusMessageActive;
                    break;
                case Status.BluetoothOff:
                    // call out settings in each os
                    await UserDialogs.Instance.AlertAsync(Resources.AppResources.ExposureNotificationStatusMessageBluetoothOff, "", Resources.AppResources.ButtonOk);
                    message = Resources.AppResources.ExposureNotificationStatusMessageBluetoothOff;
                    break;
                case Status.Restricted:
                    // call out settings in each os
                    await UserDialogs.Instance.AlertAsync(Resources.AppResources.ExposureNotificationStatusMessageRestricted, "", Resources.AppResources.ButtonOk);
                    message = Resources.AppResources.ExposureNotificationStatusMessageRestricted;
                    break;
                default:
                    break;
            }

            CurrentStatusMessage = message;
            return message;
        }

        /* Processing number issued when positive */
        public string PositiveDiagnosis { get; set; }

        /* Date of diagnosis or onset (Local time) */
        public DateTime? DiagnosisDate { get; set; }

        public IEnumerable<TemporaryExposureKey> FliterTemporaryExposureKeys(IEnumerable<TemporaryExposureKey> temporaryExposureKeys)
        {
            loggerService.StartMethod();

            IEnumerable<TemporaryExposureKey> newTemporaryExposureKeys = null;

            try
            {
                if (DiagnosisDate is DateTime diagnosisDate)
                {
                    var fromDateTime = diagnosisDate.AddDays(AppConstants.DaysToSendTek);
                    var fromDateTimeOffset = new DateTimeOffset(fromDateTime);
                    loggerService.Info($"Filter: After {fromDateTimeOffset}");
                    newTemporaryExposureKeys = temporaryExposureKeys.Where(x => x.RollingStart >= fromDateTimeOffset);
                    loggerService.Info($"Count: {newTemporaryExposureKeys.Count()}");
                }
                else
                {
                    throw new InvalidOperationException("No diagnosis date has been set");
                }
            }
            catch (Exception ex)
            {
                loggerService.Exception("Temporary exposure keys filtering failed", ex);
                throw ex;
            }
            finally
            {
                DiagnosisDate = null;
                loggerService.EndMethod();
            }

            return newTemporaryExposureKeys;
        }
    }
}
