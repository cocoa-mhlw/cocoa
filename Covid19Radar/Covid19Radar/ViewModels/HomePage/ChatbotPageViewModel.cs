using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ChatbotPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private UserDataModel _UserData;
        public UserDataModel UserData
        {
            get { return _UserData; }
            set { SetProperty(ref _UserData, value); }
        }

        private readonly ExposureNotificationService exposureNotificationService;

        private readonly IUserDataService userDataService;
        public ChatbotPageViewModel(INavigationService navigationService, ILoggerService loggerService, IUserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, exposureNotificationService)
        {
            Title = AppResources.SettingsPageTitle;
            this.loggerService = loggerService;
            this.userDataService = userDataService;
            _UserData = this.userDataService.Get();
            this.exposureNotificationService = exposureNotificationService;
        }


        private void _userDataChanged(object sender, UserDataModel e)
        {
            _UserData = this.userDataService.Get();
            RaisePropertyChanged();
        }


        public ICommand OnChangeExposureNotificationState => new Command(async () =>
        {
            loggerService.StartMethod();

            await userDataService.SetAsync(_UserData);

            loggerService.EndMethod();
        });


        public ICommand OnChangeNotificationState => new Command(async () =>
        {
            loggerService.StartMethod();

            await userDataService.SetAsync(_UserData);

            loggerService.EndMethod();
        });

        public ICommand OnChangeResetData => new Command(async () =>
        {
            loggerService.StartMethod();

            var check = await UserDialogs.Instance.ConfirmAsync(
                Resources.AppResources.SettingsPageDialogResetText,
                Resources.AppResources.SettingsPageDialogResetTitle,
                Resources.AppResources.ButtonOk,
                Resources.AppResources.ButtonCancel
            );
            if (check)
            {
                UserDialogs.Instance.ShowLoading(Resources.AppResources.LoadingTextDeleting);

                if (await ExposureNotification.IsEnabledAsync())
                {
                    await ExposureNotification.StopAsync();
                }

                // Reset All Data and Optout
                UserDataModel userData = new UserDataModel();
                await userDataService.SetAsync(userData);

                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync(Resources.AppResources.SettingsPageDialogResetCompletedText);
                Application.Current.Quit();

                // Application close
                Xamarin.Forms.DependencyService.Get<ICloseApplication>().closeApplication();

                loggerService.EndMethod();
                return;
            }

            loggerService.EndMethod();
        });
    }
}
