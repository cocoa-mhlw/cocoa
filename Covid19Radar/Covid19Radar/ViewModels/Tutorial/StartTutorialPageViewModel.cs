using Covid19Radar.Resources;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class StartTutorialPageViewModel : ViewModelBase
    {
        public StartTutorialPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            // TODO: Add Title so we can test it :)
        }

        public Command OnClickStart => new Command(async () =>
        {
            await NavigationService.NavigateAsync("DescriptionPage");
        });

    }
}
