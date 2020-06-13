using System.Collections.Generic;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Plugin.LocalNotification;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage5ViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public TutorialPage5ViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            this.userDataService = userDataService;
            userData = this.userDataService.Get();

        }

        public Command OnClickEnable => new Command(async () =>
        {
            userData.IsNotificationEnabled = true;
            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = AppResources.LocalNotificationPermittedTitle,
                Description = AppResources.LocalNotificationPermittedDescription
            };
            NotificationCenter.Current.Show(notification);
            await userDataService.SetAsync(userData);
            await NavigationService.NavigateAsync(nameof(TutorialPage6));
        });
        public Command OnClickDisable => new Command(async () =>
        {
            userData.IsNotificationEnabled = false;
            await userDataService.SetAsync(userData);
            await NavigationService.NavigateAsync(nameof(TutorialPage6));
        });

    }
}