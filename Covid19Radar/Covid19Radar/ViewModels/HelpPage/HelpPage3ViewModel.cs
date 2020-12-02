using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HelpPage3ViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;

        public HelpPage3ViewModel(INavigationService navigationService, ILoggerService loggerService) : base(navigationService)
        {
            Title = Resources.AppResources.HelpPage3Title;
            this.loggerService = loggerService;
        }

        public Command OnClickNotifyOtherPage => new Command(async () =>
        {
            loggerService.StartMethod();

            await NavigationService.NavigateAsync(nameof(SubmitConsentPage));

            loggerService.EndMethod();
        });

    }
}