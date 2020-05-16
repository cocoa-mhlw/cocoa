using System.Windows.Input;
using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SetupCompletedPageViewModel : ViewModelBase
    {
        public SetupCompletedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.TitleSetupCompleted;
        }

        public Command OnClickToMain => new Command(async () =>
        {
            await NavigationService.NavigateAsync("/MainPage");
        });

    }
}
