using Covid19Radar.Common;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class InitSettingPageViewModel : ViewModelBase
    {
        public InitSettingPageViewModel() : base()
        {
            Title = Resources.AppResources.TitleDeviceAccess;
        }

        public Command OnClickNotNow => new Command(() => NavigationService.NavigateAsync("SetupCompletedPage"));
        public Command OnClickEnable => new Command(async () =>
        {
            // TODO Allow popup?

            var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();
            if (!enabled)
            {
                await Xamarin.ExposureNotifications.ExposureNotification.StartAsync();
                await NavigationService.NavigateAsync("SetupCompletedPage");
            }
        });
    }
}
