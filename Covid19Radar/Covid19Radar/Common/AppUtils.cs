using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.Common
{
    static class AppUtils
    {
        public static async void CheckPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (status != PermissionStatus.Granted)
            {
                //if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.LocationAlways))
                //{
                //    await Application.Current.MainPage.DisplayAlert("Need location", "This Application need BLE location", "OK");
                //}

                status = await Permissions.RequestAsync<Permissions.LocationAlways>();
            }
        }
    }
}
