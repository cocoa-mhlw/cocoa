using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class StatusUpdateCompletePageViewModel : ViewModelBase
    {
        public StatusUpdateCompletePageViewModel(INavigationService navigationService, IStatusBarPlatformSpecific statusBarPlatformSpecific) : base(navigationService, statusBarPlatformSpecific)
        {
            Title = AppResources.TitleStatusUpdateComplete;
        }

        public Command OnClickHome => new Command(() => NavigationService.NavigateAsync("/NavigationPage/HomePage"));
    }
}
