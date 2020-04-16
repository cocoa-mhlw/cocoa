using Covid19Radar.Common;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class InitSettingPageViewModel : ViewModelBase
    {
        public InitSettingPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resx.AppResources.TitleDeviceAccess;
            AppUtils.CheckPermission();
        }

        public Command OnClickNext => new Command(() => NavigationService.NavigateAsync("/NavigationPage/HomePage"));
    }
}
