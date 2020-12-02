using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private string _AppVersion;

        public string AppVer
        {
            get { return _AppVersion; }
            set { SetProperty(ref _AppVersion, value); }
        }
        private UserDataModel _UserData;
        public UserDataModel UserData
        {
            get { return _UserData; }
            set { SetProperty(ref _UserData, value); }
        }

        private readonly ExposureNotificationService exposureNotificationService;

        private readonly UserDataService userDataService;
        private readonly ILogFileService logFileService;
        public SettingsPageViewModel(INavigationService navigationService, ILoggerService loggerService, UserDataService userDataService, ExposureNotificationService exposureNotificationService, ILogFileService logFileService) : base(navigationService, userDataService, exposureNotificationService)
        {
            Title = AppResources.SettingsPageTitle;
            AppVer = AppInfo.VersionString;// AppSettings.Instance.AppVersion;
            this.loggerService = loggerService;
            this.userDataService = userDataService;
            _UserData = this.userDataService.Get();
            this.exposureNotificationService = exposureNotificationService;
            this.logFileService = logFileService;
        }

        public ICommand OnChangeExposureNotificationState => new Command(async () =>
        {
            loggerService.StartMethod();

            if (UserData.IsExposureNotificationEnabled)
            {
                await exposureNotificationService.StartExposureNotification();
            }
            else
            {
                await exposureNotificationService.StopExposureNotification();
            }

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
                await userDataService.ResetAllDataAsync();

                _ = logFileService.DeleteLogsDir();

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
