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

        public UserDataService()
        {
            this.httpDataService = Xamarin.Forms.DependencyService.Resolve<HttpDataService>();
        }

        public bool IsExistUserData()
        {
            if (Application.Current.Properties.ContainsKey("UserData"))
            {
                UserDataModel userData = Utils.DeserializeFromJson<UserDataModel>(Application.Current.Properties["UserData"].ToString());
                var state = httpDataService.PutUserAsync(userData);

                if (state != null)
                {
                    return true;
                }
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

        public void Set(UserDataModel userData)
        {
            Application.Current.Properties["UserData"] = Utils.SerializeToJson(userData);
            Application.Current.SavePropertiesAsync();
        }
    }
}
