using Covid19Radar.Common;
using Covid19Radar.Model;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    /// <summary>
    /// This service registers, retrieves, stores, and automatically updates user data.
    /// </summary>
    public class UserDataService
    {
        private readonly HttpDataService httpDataService;
        private readonly INotificationService notificationService;
        private readonly INavigationService navigationService;
        private MinutesTimer _downloadTimer;
        private UserDataModel current;
        public event EventHandler<UserDataModel> UserDataChanged;

        public UserDataService(HttpDataService httpDataService, INavigationService navigationService, INotificationService notificationService)
        {
            this.httpDataService = httpDataService;
            this.navigationService = navigationService;
            this.notificationService = notificationService;
            this.notificationService.Initialize();
            this.notificationService.NotificationReceived += OnLocalNotificationTaped;
            current = Get();
            if (current != null)
            {
                // User does't have secret
                if (!httpDataService.HasSecret())
                {
                    return;
                }
                StartTimer();
            }
        }

        private async void OnLocalNotificationTaped(object sender, EventArgs e)
        {
            await navigationService.NavigateAsync("NavigationPage/HeadsupPage");
        }

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
            var hasStatusChange = downloadModel.UserStatus != current.UserStatus;
            if (hasStatusChange)
            {
                // Notification Contacted
                /*
                if (downloadModel.UserStatus == UserStatus.Contactd)
                {
                    // TOOD Change to Resouce String
                    notificationService.ScheduleNotification("TEST", "MESSAGE");
                }
                */
                var newModel = new UserDataModel()
                {
                    UserUuid = current.UserUuid,
                    Major = current.Major,
                    Minor = current.Minor,
                    UserStatus = downloadModel.UserStatus,
                    LastNotificationTime = current.LastNotificationTime
                };
                await SetAsync(newModel);
            }
            if (hasNotification)
            {
                // Pull Notification.
                try
                {
                    var newModel = new UserDataModel()
                    {
                        UserUuid = current.UserUuid,
                        Major = current.Major,
                        Minor = current.Minor,
                        UserStatus = current.UserStatus,
                        LastNotificationTime = downloadModel.LastNotificationTime
                    };
                    var result = await httpDataService.GetNotificationPullAsync(newModel);
                    foreach (var notify in result.Messages)
                    {
                        notificationService.ReceiveNotification(notify.Title, notify.Message);
                    }
                    await SetAsync(newModel);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }

        }

        public bool IsExistUserData { get => current != null; }


        public async Task<UserDataModel> RegistUserAsync()
        {
            UserDataModel userData = await httpDataService.PostRegisterUserAsync();
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
            if (userData.Equals(current))
            {
                return;
            }
            var isNull = current == null;
            if (!isNull && string.IsNullOrWhiteSpace(userData.Secret))
            {
                userData.Secret = current.Secret;
            }
            Application.Current.Properties["UserData"] = Utils.SerializeToJson(userData);
            await Application.Current.SavePropertiesAsync();
            current = userData;
            if (UserDataChanged != null)
            {
                UserDataChanged(this, current);
            }
            // only first time.
            if (isNull && userData != null)
            {
                StartTimer();
            }
        }
    }
}
