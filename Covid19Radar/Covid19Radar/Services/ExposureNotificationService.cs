using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using ImTools;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    public class ExposureNotificationService
    {
        private readonly IHttpDataService httpDataService;
        private readonly ILoggerService loggerService;
        private readonly UserDataService userDataService;
        private readonly INavigationService navigationService;
        public string CurrentStatusMessage { get; set; } = "初期状態";
        public Status ExposureNotificationStatus { get; set; }

        private SecondsTimer _downloadTimer;
        private UserDataModel userData;

        public ExposureNotificationService(INavigationService navigationService, ILoggerService loggerService, UserDataService userDataService, IHttpDataService httpDataService)
        {
            this.httpDataService = httpDataService;
            this.navigationService = navigationService;
            this.loggerService = loggerService;
            this.userDataService = userDataService;
            _ = this.GetExposureNotificationConfig();
            userData = userDataService.Get();
            userDataService.UserDataChanged += OnUserDataChanged;
            StartTimer();
        }
        private void StartTimer()
        {
            _downloadTimer = new SecondsTimer(userData.GetJumpHashTime());
            _downloadTimer.Start();
            _downloadTimer.TimeOutEvent += OnTimerInvoked;
        }

        private void OnTimerInvoked(EventArgs e)
        {
            Debug.WriteLine(DateTime.Now.ToString(new CultureInfo("en-US")));
            //await FetchExposureKeyAsync();
        }

        public async Task GetExposureNotificationConfig()
        {
            loggerService.StartMethod();

            string container = AppSettings.Instance.BlobStorageContainerName;
            string url = AppSettings.Instance.CdnUrlBase + $"{container}/Configration.json";
            HttpClient httpClient = new HttpClient();
            Task<HttpResponseMessage> response = httpClient.GetAsync(url);
            HttpResponseMessage result = await response;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                loggerService.Info("Success to download configuration");
                Application.Current.Properties["ExposureNotificationConfigration"] = await result.Content.ReadAsStringAsync();
                await Application.Current.SavePropertiesAsync();
            }
            else
            {
                loggerService.Error("Fail to download configuration");
            }

            loggerService.EndMethod();
        }


        private async void OnUserDataChanged(object sender, UserDataModel userData)
        {
            Debug.WriteLine("User Data has Changed!!!");
            this.userData = userDataService.Get();
            Debug.WriteLine(Utils.SerializeToJson(userData));
            await UpdateStatusMessageAsync();
        }

        public async Task FetchExposureKeyAsync()
        {
            loggerService.StartMethod();

            await Xamarin.ExposureNotifications.ExposureNotification.UpdateKeysFromServer();

            loggerService.EndMethod();
        }

        public int GetExposureCount()
        {
            loggerService.StartMethod();
            loggerService.EndMethod();
            return userData.ExposureInformation.Count();
        }

        public async Task<string> UpdateStatusMessageAsync()
        {
            loggerService.StartMethod();

            this.ExposureNotificationStatus = await ExposureNotification.GetStatusAsync();

            loggerService.EndMethod();
            return await GetStatusMessageAsync();
        }

        private async Task DisabledAsync()
        {
            userData.IsExposureNotificationEnabled = false;
            await userDataService.SetAsync(userData);
        }

        private async Task EnabledAsync()
        {
            userData.IsExposureNotificationEnabled = true;
            await userDataService.SetAsync(userData);
        }


        public async Task<bool> StartExposureNotification()
        {
            loggerService.StartMethod();
            try
            {
                var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();
                if (!enabled)
                {
                    await Xamarin.ExposureNotifications.ExposureNotification.StartAsync();
                }
                await EnabledAsync();

                loggerService.EndMethod();
                return true;
            }
            catch (Exception ex)
            {
                await DisabledAsync();

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
                var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();
                if (enabled)
                {
                    await Xamarin.ExposureNotifications.ExposureNotification.StopAsync();
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
            finally
            {
                await DisabledAsync();
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

            if (!userData.IsOptined)
            {
                message.Append(Resources.AppResources.ExposureNotificationStatusMessageIsOptined);
            }

            this.CurrentStatusMessage = message;
            return message;
        }


    }
}
