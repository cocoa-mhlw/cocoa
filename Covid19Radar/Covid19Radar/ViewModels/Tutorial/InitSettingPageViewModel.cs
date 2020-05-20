using Covid19Radar.Common;
using Covid19Radar.Renderers;
using Covid19Radar.Services;
using Covid19Radar.Views;
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

        public Command OnClickNotNow => new Command(async () =>
        {
            LocalStateManager.Instance.LastIsEnabled = false;
            LocalStateManager.Save();
            await NavigationService.NavigateAsync("SetupCompletedPage");
        });
        public Command OnClickEnable => new Command(async () =>
        {
            LocalStateManager.Instance.LastIsEnabled = true;
            LocalStateManager.Save();
            await NavigationService.NavigateAsync(nameof(SetupCompletedPage));

        });
    }
}
