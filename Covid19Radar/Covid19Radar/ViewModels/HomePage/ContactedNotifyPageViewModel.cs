using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Essentials;
using Covid19Radar.Resources;

namespace Covid19Radar.ViewModels
{
    public class ContactedNotifyPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private string _exposureCount;
        public string ExposureCount
        {
            get { return _exposureCount; }
            set { SetProperty(ref _exposureCount, value); }
        }

        public ContactedNotifyPageViewModel(INavigationService navigationService, ILoggerService loggerService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            Title = AppResources.TitileUserStatusSettings;
            this.loggerService = loggerService;
            ExposureCount = exposureNotificationService.GetExposureCount().ToString();
        }

        public Command OnClickByForm => new Command(async () =>
        {
            loggerService.StartMethod();

            var uri = AppResources.UrlContactedForm;
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

            loggerService.EndMethod();
        });
    }
}
