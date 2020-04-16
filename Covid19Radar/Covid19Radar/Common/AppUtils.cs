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
                status = await Permissions.RequestAsync<Permissions.LocationAlways>();
            }
        }
    }
}
