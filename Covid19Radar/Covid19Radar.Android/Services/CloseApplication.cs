using Covid19Radar.Services;

namespace Covid19Radar.Droid.Services
{
    public class CloseApplication : ICloseApplication
    {
        public void closeApplication()
        {
            var activity = Xamarin.Essentials.Platform.CurrentActivity;
            activity.FinishAffinity();
        }
    }
}