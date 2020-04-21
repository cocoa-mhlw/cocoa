using Covid19Radar.Resx;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class StatusUpdateCompletePageViewModel : ViewModelBase
    {
        public StatusUpdateCompletePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.TitleStatusUpdateComplete;
        }

        public Command OnClickHome => new Command(() => NavigationService.NavigateAsync("/NavigationPage/HomePage"));
    }
}
