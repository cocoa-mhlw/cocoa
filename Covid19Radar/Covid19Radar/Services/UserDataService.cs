using Covid19Radar.Common;
using Covid19Radar.Model;
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
        private MinutesTimer _downloadTimer;
        private UserDataModel current;
        public event EventHandler<UserDataModel> UserDataChanged;

        public UserDataService()
        {
            this.httpDataService = Xamarin.Forms.DependencyService.Resolve<HttpDataService>();

            current = Get();
            if (current != null)
            {
                StartTimer();
            }
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

            var downloadModel = await httpDataService.PostUserAsync(current);
            if (downloadModel.UserStatus != current.UserStatus)
            {
                var newModel = new UserDataModel()
                {
                    UserUuid = current.UserUuid,
                    Major = current.Major,
                    Minor = current.Minor,
                    UserStatus = current.UserStatus
                };
                await SetAsync(newModel);
            }

        }

        public bool IsExistUserData { get => current != null; }


        public async Task<UserDataModel> RegistUserAsync()
        {
            UserDataModel userData = await httpDataService.PostRegisterUserAsync();
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
            Application.Current.Properties["UserData"] = Utils.SerializeToJson(userData);
            await Application.Current.SavePropertiesAsync();
            var isNull = current == null;
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
