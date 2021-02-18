using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage5ViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;

        public TutorialPage5ViewModel(INavigationService navigationService, ILoggerService loggerService) : base(navigationService)
        {
            this.loggerService = loggerService;

        }

        public Command OnClickEnable => new Command(async () =>
        {
            loggerService.StartMethod();
            await NavigationService.NavigateAsync(nameof(TutorialPage6));
            loggerService.EndMethod();
        });

        public Command OnClickDisable => new Command(async () =>
        {
            loggerService.StartMethod();
            await NavigationService.NavigateAsync(nameof(TutorialPage6));
            loggerService.EndMethod();
        });
    }
}