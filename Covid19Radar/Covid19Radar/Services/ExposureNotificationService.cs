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
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Xamarin.ExposureNotifications;

namespace Covid19Radar.Services
{
    public class ExposureNotificationService
    {
        private readonly HttpDataService httpDataService;
        private readonly UserDataService userDataService;
        private readonly INavigationService navigationService;
        //private UserDataModel userData;
        public string CurrentStatusMessage { get; set; } = "初期状態";
        public Status ExposureNotificationStatus { get; set; }

        private UserDataModel userData;

        public ExposureNotificationService(INavigationService navigationService, UserDataService userDataService, HttpDataService httpDataService)
        {
            this.httpDataService = httpDataService;
            this.navigationService = navigationService;
            this.userDataService = userDataService;
            userData = userDataService.Get();
            userDataService.UserDataChanged += OnUserDataChanged;
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

            Status status = await ExposureNotification.GetStatusAsync();
            GetStatusMessage(status);
        }

        public int GetExposureCount()
        {
            return userData.ExposureInformation.Count();
        }

        /*
        public async Task TestDownloadBatch()
        {
            long sinceEpochSeconds = new DateTimeOffset(DateTime.UtcNow.AddDays(-14)).ToUnixTimeSeconds();
            TemporaryExposureKeysResult tekResult = await httpDataService.GetTemporaryExposureKeys(sinceEpochSeconds);
            Console.WriteLine("Fetch Exposure Key");

            foreach (var keys in tekResult.Keys)
            {
                Console.WriteLine(keys.Url);
            }

        }

        public bool GetOptInStatus()
        {
            return userData.IsOptined;
        }
                */


        /*
        public async Task SetOptinStatusAsync(bool flg)
        {
            userData.IsOptined = flg;
            await userDataService.SetAsync(userData);
        }
        public bool GetOptInStatus()
        {
            return userData.IsOptined;
        }


        public async Task SetExposureNotificationStatusAsync(bool flg)
        {
            userData.IsExposureNotificationEnabled = flg;
            await userDataService.SetAsync(userData);
        }
        public bool GetExposureNotificationStatus()
        {
            return userData.IsExposureNotificationEnabled;
        }

        public async Task SetNotificationStatusAsync(bool flg)
        {
            userData.IsNotificationEnabled = flg;
            await userDataService.SetAsync(userData);
        }
        public bool GetNotificationStatus()
        {
            return userData.IsNotificationEnabled;
        }
        */
        public async Task<bool> StartExposureNotification()
        {
            if (!userData.IsOptined)
            {
                await UserDialogs.Instance.AlertAsync("利用規約に同意する必要があります。同意ページへ遷移します。");
                await navigationService.NavigateAsync(nameof(PrivacyPolicyPage));
            }

            Status status = await ExposureNotification.GetStatusAsync();
            if (status == Status.BluetoothOff
            //            || status == Status.Restricted
            || status == Status.NotAuthorized)
            {
                await UserDialogs.Instance.AlertAsync(GetStatusMessage(status));
                userData.IsExposureNotificationEnabled = false;
                await userDataService.SetAsync(userData);
                return false;
            }

            //            bool IsEnabled = await ExposureNotification.IsEnabledAsync();

            if (userData.IsOptined && userData.IsExposureNotificationEnabled && (status == Status.Unknown || status == Status.Active || status == Status.Disabled))
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
        }

        public async Task<bool> StopExposureNotification()
        {
            await ExposureNotification.StopAsync();
            return true;
        }

        public string GetStatusMessage(Status status)
        {
            var message = "";

            switch (status)
            {
                case Status.Unknown:
                    message = "Exposure Notification機能は非対応の状態です。";
                    break;
                case Status.Disabled:
                    message = "Exposure Notification機能は無効の状態です。";
                    break;
                case Status.Active:
                    message = "Exposure Notification機能は許諾の状態です。";
                    break;
                case Status.BluetoothOff:
                    message = "BluetoothがOffになっています。Bluetoothを有効にしてください。";
                    break;
                case Status.Restricted:
                    message = "Exposure Notification機能が制限されています。制限を解除してください。";
                    break;
                case Status.NotAuthorized:
                    message = "Exposure Notification機能が承認されていません。承認してください。";
                    break;
                default:
                    break;
            }

            if (!userData.IsOptined)
            {
                message.Append("/利用規約に同意する必要があります。");
            }

            this.CurrentStatusMessage = message;
            Console.WriteLine(message);
            return message;
        }


    }
}
