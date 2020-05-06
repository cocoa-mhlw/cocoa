using System.Windows.Input;
using Covid19Radar.Resources;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SetupCompletedPageViewModel : ViewModelBase
    {

        public SetupCompletedPageViewModel() : base()
        {
            Title = AppResources.TitleSetupCompleted;
        }

        public Command OnClickHome => new Command(() => NavigationService.NavigateAsync("MainPage"));

    }
}
