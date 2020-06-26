using System;
using System.Threading;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage4ViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private readonly ExposureNotificationService exposureNotificationService;
        private UserDataModel userData;

        public TutorialPage4ViewModel(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            this.userDataService = userDataService;
            this.exposureNotificationService = exposureNotificationService;
            userData = this.userDataService.Get();
        }

        public Command OnClickEnable => new Command(async () =>
        {
            await ExposureNotificationService.StartExposureNotification();
            await NavigationService.NavigateAsync(nameof(TutorialPage6));
        });
        public Command OnClickDisable => new Command(async () =>
        {
            userData.IsExposureNotificationEnabled = false;
            await userDataService.SetAsync(userData);
            await NavigationService.NavigateAsync(nameof(TutorialPage6));
        });
    }
}