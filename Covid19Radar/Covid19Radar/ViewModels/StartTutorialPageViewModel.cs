using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class StartTutorialPageViewModel : ViewModelBase
    {
        
        public StartTutorialPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        public Command OnClickNext => (new Command(() =>
        {
            NavigationService.NavigateAsync("DescriptionPage");
        }));

    }
}
