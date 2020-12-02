using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HelpPage2ViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;

        public HelpPage2ViewModel(INavigationService navigationService, ILoggerService loggerService) : base(navigationService)
        {
            Title = Resources.AppResources.HelpPage2Title;
            this.loggerService = loggerService;
        }

        public Command OnClickNext => new Command(async () =>
        {
            loggerService.StartMethod();

            await NavigationService.NavigateAsync(nameof(HelpPage4));

            loggerService.EndMethod();
        });
    }
}