using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HelpPage4ViewModel : ViewModelBase
    {
        public HelpPage4ViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.HelpPage4Title;
        }

        public Command OnClickSetting => new Command(async () =>
        {
            await NavigationService.NavigateAsync(nameof(SettingsPage));
        });
    }
}