using Covid19Radar.Common;
using Covid19Radar.Renderers;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class InitSettingPageViewModel : ViewModelBase
    {
        public InitSettingPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.TitleDeviceAccess;
        }

        public Command OnClickNotNow => new Command(() => NavigationService.NavigateAsync("SetupCompletedPage"));
        public Command OnClickEnable => new Command(async () =>
        {
            // TODO Enable Exposure Notification
            /*
            var enabled = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();
            if (!enabled)
            {
                await Xamarin.ExposureNotifications.ExposureNotification.StartAsync();
            }
            */
            await NavigationService.NavigateAsync("SetupCompletedPage");

        });
    }
}
