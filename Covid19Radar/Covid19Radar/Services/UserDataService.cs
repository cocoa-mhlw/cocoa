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
    public class UserDataService
    {
        private readonly HttpDataService httpDataService;
        private readonly MinutesTimer _downloadTimer;
        private UserDataModel current;

        public UserDataService()
        {
            this.httpDataService = Xamarin.Forms.DependencyService.Resolve<HttpDataService>();

            if (IsExistUserData())
            {
                current = Get();
                _downloadTimer = new MinutesTimer(current.GetJumpHashTimeDifference());
                _downloadTimer.Start();
                _downloadTimer.TimeOutEvent += TimerDownload;
            }
        }

        private async void TimerDownload(EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString());

            var downloadModel = await httpDataService.PostUserAsync(current);
            if (downloadModel.UserStatus != current.UserStatus)
            {
                current = downloadModel;
                await SetAsync(downloadModel);
            }

        }

        public bool IsExistUserData()
        {
            if (Application.Current.Properties.ContainsKey("UserData"))
            {
                var userDataJson = Application.Current.Properties["UserData"].ToString();

                UserDataModel userData = Utils.DeserializeFromJson<UserDataModel>(userDataJson);
                return (userData != null);
            }
            return false;
        }


        public async Task<UserDataModel> RegistUserAsync()
        {
            UserDataModel userData = await httpDataService.PostRegisterUserAsync();
            Application.Current.Properties["UserData"] = Utils.SerializeToJson(userData);
            await Application.Current.SavePropertiesAsync();
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
            Application.Current.Properties["UserData"] = Utils.SerializeToJson(userData);
            await Application.Current.SavePropertiesAsync();
        }
    }
}
