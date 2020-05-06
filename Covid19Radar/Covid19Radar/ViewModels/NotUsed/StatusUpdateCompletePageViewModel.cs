using Covid19Radar.Resources;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class StatusUpdateCompletePageViewModel : ViewModelBase
    {
        public StatusUpdateCompletePageViewModel() : base()
        {
            Title = AppResources.TitleStatusUpdateComplete;
        }

        public Command OnClickHome => new Command(() => NavigationService.NavigateAsync("/NavigationPage/HomePage"));
    }
}
