using System.Collections.Generic;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
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
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                userData.IsExposureNotificationEnabled = true;
                await userDataService.SetAsync(userData).ConfigureAwait(true);
                await exposureNotificationService.StartExposureNotification().ConfigureAwait(true);
            });
            await NavigationService.NavigateAsync(nameof(TutorialPage5));

        });
        public Command OnClickDisable => new Command(async () =>
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                userData.IsExposureNotificationEnabled = false;
                await userDataService.SetAsync(userData);
            });
            await NavigationService.NavigateAsync(nameof(TutorialPage5));
        });
    }
}