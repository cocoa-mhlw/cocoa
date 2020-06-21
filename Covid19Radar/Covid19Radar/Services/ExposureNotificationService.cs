using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
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
        private readonly UserDataService userDataService;
        private readonly INavigationService navigationService;
        public string CurrentStatusMessage { get; set; } = "初期状態";
        public Status ExposureNotificationStatus { get; set; }

        private SecondsTimer _downloadTimer;
        private UserDataModel userData;

        public ExposureNotificationService(INavigationService navigationService, UserDataService userDataService, IHttpDataService httpDataService)
        {
            this.httpDataService = httpDataService;
            this.navigationService = navigationService;
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

        private async void OnTimerInvoked(EventArgs e)
        {
            Debug.WriteLine(DateTime.Now.ToString(new CultureInfo("en-US")));
            //await FetchExposureKeyAsync();
        }

        public async Task GetExposureNotificationConfig()
        {
            string container = AppSettings.Instance.BlobStorageContainerName;
            string url = AppSettings.Instance.CdnUrlBase + $"{container}/Configration.json";
            HttpClient httpClient = new HttpClient();
            Task<HttpResponseMessage> response = httpClient.GetAsync(url);
            HttpResponseMessage result = await response;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Application.Current.Properties["ExposureNotificationConfigration"] = await result.Content.ReadAsStringAsync();
                await Application.Current.SavePropertiesAsync();
            }
        }


        private async void OnUserDataChanged(object sender, UserDataModel userData)
        {
            Debug.WriteLine("User Data has Changed!!!");
            this.userData = userDataService.Get();
            Debug.WriteLine(Utils.SerializeToJson(userData));
            await UpdateStatusMessage();
        }

        public async Task FetchExposureKeyAsync()
        {
            await Xamarin.ExposureNotifications.ExposureNotification.UpdateKeysFromServer();
        }

        public int GetExposureCount()
        {
            return userData.ExposureInformation.Count();
        }

        public async Task<string> UpdateStatusMessage()
        {
            this.ExposureNotificationStatus = await ExposureNotification.GetStatusAsync();
            return GetStatusMessage();
        }

        private async Task DisabledAsync()
        {
            userData.IsExposureNotificationEnabled = false;
            await userDataService.SetAsync(userData);
            await UpdateStatusMessage();
        }

        private async Task EnabledAsync()
        {
            userData.IsExposureNotificationEnabled = true;
            await userDataService.SetAsync(userData);
            await UpdateStatusMessage();
        }


        public async Task<bool> StartExposureNotification()
        {
            try
            {
                var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();
                if (!enabled)
                {
                    await Xamarin.ExposureNotifications.ExposureNotification.StartAsync();
                }
                await EnabledAsync();
                return true;
            }
            catch (Exception)
            {
                await DisabledAsync();
                return false;
            }
            finally
            {

            }
        }

        public async Task<bool> StopExposureNotification()
        {
            try
            {
                var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();
                if (enabled) {
                    await Xamarin.ExposureNotifications.ExposureNotification.StopAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disabling notifications: {ex}");
                return false;
            }
            finally
            {
                await DisabledAsync();
            }
        }

        public string GetStatusMessage()
        {
            var message = "";

            switch (ExposureNotificationStatus)
            {
                case Status.Unknown:
                    UserDialogs.Instance.AlertAsync(Resources.AppResources.ExposureNotificationStatusMessageUnknown, Resources.AppResources.DialogExposureNotificationStartupErrorTitle, Resources.AppResources.ButtonOk);
                    message = Resources.AppResources.ExposureNotificationStatusMessageUnknown;
                    break;
                case Status.Disabled:
                    UserDialogs.Instance.AlertAsync(Resources.AppResources.ExposureNotificationStatusMessageDisabled, Resources.AppResources.DialogExposureNotificationStartupErrorTitle, Resources.AppResources.ButtonOk);
                    message = Resources.AppResources.ExposureNotificationStatusMessageDisabled;
                    break;
                case Status.Active:
                    message = Resources.AppResources.ExposureNotificationStatusMessageActive;
                    break;
                case Status.BluetoothOff:
                    // call out settings in each os
                    UserDialogs.Instance.AlertAsync(Resources.AppResources.ExposureNotificationStatusMessageBluetoothOff, Resources.AppResources.DialogExposureNotificationStartupErrorTitle, Resources.AppResources.ButtonOk);
                    message = Resources.AppResources.ExposureNotificationStatusMessageBluetoothOff;
                    break;
                case Status.Restricted:
                    // call out settings in each os
                    UserDialogs.Instance.AlertAsync(Resources.AppResources.ExposureNotificationStatusMessageRestricted, Resources.AppResources.DialogExposureNotificationStartupErrorTitle, Resources.AppResources.ButtonOk);
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
            Debug.WriteLine(message);
            return message;
        }


    }
}
