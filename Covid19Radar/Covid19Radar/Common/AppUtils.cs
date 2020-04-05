using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Covid19Radar.Common
{
    static class AppUtils
    {
        public static async void CheckPermission()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationAlwaysPermission>();
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.LocationAlways))
                {
                    await Application.Current.MainPage.DisplayAlert("Need location", "This Application need BLE location", "OK");
                }

                status = await CrossPermissions.Current.RequestPermissionAsync<LocationAlwaysPermission>();
            }
        }
    }
}
