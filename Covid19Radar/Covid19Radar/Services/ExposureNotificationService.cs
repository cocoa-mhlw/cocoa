using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Views;
using ImTools;
using Prism.Navigation;
using Prism.Navigation.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    public class ExposureNotificationService
    {
        private readonly HttpDataService httpDataService;
        private readonly UserDataService userDataService;
        private readonly INavigationService navigationService;
        public string CurrentStatusMessage { get; set; } = "初期状態";
        public Status ExposureNotificationStatus { get; set; }

        private SecondsTimer _downloadTimer;
        private UserDataModel userData;

        public ExposureNotificationService(INavigationService navigationService, UserDataService userDataService, HttpDataService httpDataService)
        {
            this.httpDataService = httpDataService;
            this.navigationService = navigationService;
            this.userDataService = userDataService;
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
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString());
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
            Console.WriteLine("User Data has Changed!!!");
            this.userData = userDataService.Get();
            Console.WriteLine(Utils.SerializeToJson(userData));

            if (userData.IsExposureNotificationEnabled)
            {
                await StartExposureNotification();
            }
            else
            {
                await StopExposureNotification();
            }

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

        public async Task<bool> StartExposureNotification()
        {
            /*
            if (!userData.IsOptined)
            {
                await UserDialogs.Instance.AlertAsync("利用規約に同意する必要があります。同意ページへ遷移します。");
                await navigationService.NavigateAsync(nameof(PrivacyPolicyPage));
            }
            */

            try
            {
                await ExposureNotification.StartAsync();
                var count = 0;
                while (true)
                {

                    Thread.Sleep(1000);
                    await ExposureNotification.StartAsync();

                    Status status = await ExposureNotification.GetStatusAsync();
                    if (status == Status.Active)
                    {
                        return true;
                    }
                    else if (status == Status.BluetoothOff)
                    {
                        await UserDialogs.Instance.AlertAsync(GetStatusMessage());
                        return true;
                    }
                    else
                    {
                        if (count > 2)
                        {
                            throw new Exception();
                        }
                        count++;
                    }
                }
            }
            catch (Exception)
            {
                userData.IsExposureNotificationEnabled = false;
                await userDataService.SetAsync(userData);
                return false;
            }

            /*
            ExposureNotificationStatus = await ExposureNotification.GetStatusAsync();
            if (ExposureNotificationStatus == Status.BluetoothOff
            //            || ExposureNotificationStatus == Status.Restricted
            || ExposureNotificationStatus == Status.NotAuthorized)
            {
                await UserDialogs.Instance.AlertAsync(GetStatusMessage());
                userData.IsExposureNotificationEnabled = false;
                await userDataService.SetAsync(userData);
                return false;
            }

            if (userData.IsOptined && userData.IsExposureNotificationEnabled && (ExposureNotificationStatus == Status.Unknown || ExposureNotificationStatus == Status.Active || ExposureNotificationStatus == Status.Disabled))
            {
                try
                {
                    await ExposureNotification.StartAsync();

                }
                catch (Exception)
                {
                    userData.IsExposureNotificationEnabled = false;
                    await userDataService.SetAsync(userData);
                    return false;
                }
            }
            return true;
            */
        }

        public async Task<bool> StopExposureNotification()
        {
            if (await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync())
            {
                await ExposureNotification.StopAsync();
            }
            return true;
        }

        public string GetStatusMessage()
        {
            var message = "";

            switch (ExposureNotificationStatus)
            {
                case Status.Unknown:
                    message = Resources.AppResources.ExposureNotificationStatusMessageUnknown;
                    break;
                case Status.Disabled:
                    message = Resources.AppResources.ExposureNotificationStatusMessageDisabled;
                    break;
                case Status.Active:
                    message = Resources.AppResources.ExposureNotificationStatusMessageActive;
                    break;
                case Status.BluetoothOff:
                    message = Resources.AppResources.ExposureNotificationStatusMessageBluetoothOff;
                    break;
                case Status.Restricted:
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
            Console.WriteLine(message);
            return message;
        }


    }
}
