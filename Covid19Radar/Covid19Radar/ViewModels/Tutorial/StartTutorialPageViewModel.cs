using Covid19Radar.Renderers;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class StartTutorialPageViewModel : ViewModelBase
    {
        public StartTutorialPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public Command OnClickStart => new Command(async () =>
        {
            await NavigationService.NavigateAsync(nameof(PrivacyPolicyPage));
        });

    }
}
