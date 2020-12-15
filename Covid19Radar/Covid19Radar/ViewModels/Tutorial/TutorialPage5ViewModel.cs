using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
//using Plugin.LocalNotification;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage5ViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly IUserDataService userDataService;
        private UserDataModel userData;

        public TutorialPage5ViewModel(INavigationService navigationService, ILoggerService loggerService, IUserDataService userDataService) : base(navigationService)
        {
            this.loggerService = loggerService;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();

        }

        public Command OnClickEnable => new Command(async () =>
        {
            loggerService.StartMethod();

            //var notification = new NotificationRequest
            //{
            //    NotificationId = 100,
            //    Title = AppResources.LocalNotificationPermittedTitle,
            //    Description = AppResources.LocalNotificationPermittedDescription
            //};
            //NotificationCenter.Current.Show(notification);
            userData.IsNotificationEnabled = true;
            await userDataService.SetAsync(userData);
            await NavigationService.NavigateAsync(nameof(TutorialPage6));

            loggerService.EndMethod();
        });
        public Command OnClickDisable => new Command(async () =>
        {
            loggerService.StartMethod();

            userData.IsNotificationEnabled = false;
            await userDataService.SetAsync(userData);
            await NavigationService.NavigateAsync(nameof(TutorialPage6));

            loggerService.EndMethod();
        });

    }
}