using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class UserSettingPageViewModel : ViewModelBase
    {
        public UserSettingPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resx.AppResources.TitleStatusSettings;
        }

        public Command OnChangeStatusOverInfection => (new Command(() =>
        {
            NavigationService.NavigateAsync("SmsVerificationPage");
        }));
    }
}
