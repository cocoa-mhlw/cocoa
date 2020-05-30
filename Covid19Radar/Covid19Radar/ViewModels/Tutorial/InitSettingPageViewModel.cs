using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class InitSettingPageViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private readonly ExposureNotificationService exposureNotificationService;
        private UserDataModel userData;

        public InitSettingPageViewModel(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            Title = Resources.AppResources.TitleDeviceAccess;
            this.userDataService = userDataService;
            this.exposureNotificationService = exposureNotificationService;
            userData = this.userDataService.Get();
        }

        public Command OnClickNotNow => new Command(async () =>
       {
           userData.IsExposureNotificationEnabled = false;
           await userDataService.SetAsync(userData);
           await NavigationService.NavigateAsync(nameof(SetupCompletedPage));
       });

        public Command OnClickEnable => new Command(async () =>
        {
            if (await ExposureNotificationService.StartExposureNotification())
            {
                await NavigationService.NavigateAsync(nameof(SetupCompletedPage));
            }
        });
    }
}
