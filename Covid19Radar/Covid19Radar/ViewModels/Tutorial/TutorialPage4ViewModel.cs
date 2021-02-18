using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage4ViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly IExposureNotificationService exposureNotificationService;

        public TutorialPage4ViewModel(INavigationService navigationService, ILoggerService loggerService, IExposureNotificationService exposureNotificationService) : base(navigationService)
        {
            this.loggerService = loggerService;
            this.exposureNotificationService = exposureNotificationService;
        }

        public Command OnClickEnable => new Command(async () =>
        {
            loggerService.StartMethod();

            await exposureNotificationService.StartExposureNotification();
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