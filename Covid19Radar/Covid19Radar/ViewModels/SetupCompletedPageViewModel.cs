using System.Windows.Input;
using Covid19Radar.Resx;
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

        public Command OnClickHome => new Command(() => NavigationService.NavigateAsync("/NavigationPage/HomePage"));

    }
}
