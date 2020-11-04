using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Essentials;
using Covid19Radar.Resources;

namespace Covid19Radar.ViewModels
{
    public class ContactedNotifyPageViewModel : ViewModelBase
    {
        private string _exposureCount;
        public string ExposureCount
        {
            get { return _exposureCount; }
            set { SetProperty(ref _exposureCount, value); }
        }

        private readonly ExposureNotificationService exposureNotificationService;

        public ContactedNotifyPageViewModel(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            Title = AppResources.TitileUserStatusSettings;
            this.exposureNotificationService = exposureNotificationService;
            ExposureCount = exposureNotificationService.GetExposureCount().ToString();
        }

        public Command OnClickByForm => new Command(async () =>
        {
            var uri = AppResources.UrlContactedForm;
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        });
    }
}
