/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Common;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.ExposureNotifications;

namespace Covid19Radar.Services
{
    public interface IExposureNotificationService
    {
        Configuration GetConfiguration();
        void RemoveConfiguration();

        Task FetchExposureKeyAsync();

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
        private readonly IPreferencesService preferencesService;

        public string CurrentStatusMessage { get; set; } = "初期状態";
        public Status ExposureNotificationStatus { get; set; }

        public ExposureNotificationService(
            ILoggerService loggerService,
            IHttpClientService httpClientService,
            IPreferencesService preferencesService
            )
        {
            this.loggerService = loggerService;
            this.httpClientService = httpClientService;
            this.preferencesService = preferencesService;

            _ = GetExposureNotificationConfig();
        }

        private async Task GetExposureNotificationConfig()
        {
            loggerService.StartMethod();
            try
            {
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
            }
            catch (Exception ex)
            {
                loggerService.Exception("Failed download of exposure notification configuration.", ex);
            }
            finally
            {
                loggerService.EndMethod();
            }
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
