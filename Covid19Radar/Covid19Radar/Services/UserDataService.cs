using Covid19Radar.Common;
using Covid19Radar.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    /// <summary>
    /// This service registers, retrieves, stores, and automatically updates user data.
    /// </summary>
    public class UserDataService
    {
        private readonly IHttpDataService httpDataService;
        private UserDataModel current;
        public event EventHandler<UserDataModel> UserDataChanged;

        public UserDataService(IHttpDataService httpDataService)
        {
            this.httpDataService = httpDataService;
            current = Get();
        }

        public bool IsExistUserData { get => current != null; }

        public async Task<UserDataModel> RegisterUserAsync()
        {
            var userData = await httpDataService.PostRegisterUserAsync();
            if (userData == null)
            {
                return null;
            }
            userData.StartDateTime = DateTime.UtcNow;
            userData.IsExposureNotificationEnabled = false;
            userData.IsNotificationEnabled = false;
            userData.IsOptined = false;
            userData.IsPolicyAccepted = false;
            userData.IsPositived = false;
            await SetAsync(userData);
            return userData;
        }

        public UserDataModel Get()
        {
            var storedUserData = SecureStorage.GetAsync(AppConstants.StorageKey.UserData).Result;
            if (storedUserData != null)
            {
                return Utils.DeserializeFromJson<UserDataModel>(storedUserData);
            }
            return null;
        }

        public async Task SetAsync(UserDataModel userData)
        {
            var newdata = Utils.SerializeToJson(userData);
            var currentdata = Utils.SerializeToJson(current);
            if (currentdata.Equals(newdata))
            {
                return;
            }
            await SecureStorage.SetAsync(AppConstants.StorageKey.UserData, newdata);
            current = Get();

            UserDataChanged?.Invoke(this, current);
        }

        public async Task ResetAllDataAsync()
        {
            Application.Current.Properties.Remove("UserData");
            current = null;
            await Application.Current.SavePropertiesAsync();
        }
    }

}
