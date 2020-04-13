using Covid19Radar.Common;
using Covid19Radar.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    public class UserDataService
    {
        public bool IsExistUserData()
        {
            return Application.Current.Properties.ContainsKey("UserData");
        }

        public UserDataModel Get()
        {
            if (Application.Current.Properties.ContainsKey("UserData"))
            {
                return Utils.DeserializeFromJson<UserDataModel>(Application.Current.Properties["UserData"].ToString());
            }
            else
            {
                return new UserDataModel();
            }
        }

        public void Set(UserDataModel userData)
        {
            Application.Current.Properties["UserData"] = Utils.SerializeToJson(userData);
            Application.Current.SavePropertiesAsync();
        }
    }
}
