using Covid19Radar.Common;
using Covid19Radar.Model;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.LocalNotification;
using Prism.Navigation;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    /// <summary>
    /// This service registers, retrieves, stores, and automatically updates user data.
    /// </summary>
    public class UserDataService
    {
        private readonly HttpDataService httpDataService;
//        private readonly INavigationService navigationService;
        private MinutesTimer _downloadTimer;
        private UserDataModel current;
        public event EventHandler<UserDataModel> UserDataChanged;

        public UserDataService(HttpDataService httpDataService, INavigationService navigationService)
        {
            this.httpDataService = httpDataService;
//            this.navigationService = navigationService;

            current = Get();
            /*
            if (current != null)
            {
                // User does't have secret
                if (!httpDataService.HasSecret())
                {
                    return;
                }
                StartTimer();
            }
*/
        }

        /*
        private void StartTimer()
        {
            _downloadTimer = new MinutesTimer(current.GetJumpHashTimeDifference());
            _downloadTimer.Start();
            _downloadTimer.TimeOutEvent += TimerDownload;
        }


        private async void TimerDownload(EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString());
            if (!IsExistUserData) { return; }
            UserDataModel downloadModel;
            try
            {
                downloadModel = await httpDataService.GetUserAsync(current);
                if (downloadModel == null) return;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return;
            }
            var hasNotification = downloadModel.LastNotificationTime != current.LastNotificationTime;

            if (hasNotification)
            {
                // Pull Notification.
                try
                {
                    var newModel = new UserDataModel()
                    {
                        UserUuid = current.UserUuid,
                        LastNotificationTime = downloadModel.LastNotificationTime
                    };
                    var result = await httpDataService.GetNotificationPullAsync(newModel);
                    foreach (var notify in result.Messages)
                    {

                        // TODO Positive Notify 
                        // notificationService.ReceiveNotification(notify.Title, notify.Message);
                    }
                    await SetAsync(newModel);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }

        }
        */

        public bool IsExistUserData { get => current != null; }

        public async Task<UserDataModel> RegisterUserAsync()
        {
            var userData = await httpDataService.PostRegisterUserAsync();
            if (userData == null)
            {
                return null;
            }
            await SetAsync(userData);
            return userData;
        }

        public UserDataModel Get()
        {
            if (Application.Current.Properties.ContainsKey("UserData"))
            {
                return Utils.DeserializeFromJson<UserDataModel>(Application.Current.Properties["UserData"].ToString());
            }
            return null;
        }

        public async Task SetAsync(UserDataModel userData)
        {
            if (Equals(userData, current))
            {
                return;
            }
            var isNull = current == null;
            if (!isNull && string.IsNullOrWhiteSpace(userData.Secret))
            {
                userData.Secret = current.Secret;
            }

            current = userData;
            Application.Current.Properties["UserData"] = Utils.SerializeToJson(current);
            await Application.Current.SavePropertiesAsync();

            UserDataChanged?.Invoke(this, current);
            // only first time.
            /*
            if (isNull && userData != null)
            {
                StartTimer();
            }
            */
        }
    }

}
