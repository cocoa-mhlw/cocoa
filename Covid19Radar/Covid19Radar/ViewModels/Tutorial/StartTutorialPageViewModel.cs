using Covid19Radar.Renderers;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class StartTutorialPageViewModel : ViewModelBase
    {
        public StartTutorialPageViewModel(INavigationService navigationService, IStatusBarPlatformSpecific statusBarPlatformSpecific) : base(navigationService, statusBarPlatformSpecific)
        {
        }

        public Command OnClickStart => new Command(async () =>
        {
            await NavigationService.NavigateAsync("DescriptionPage1");
        });

    }
}
