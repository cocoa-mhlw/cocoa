using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ChatbotPageViewModel : ViewModelBase
    {
        private UserDataModel _UserData;
        public UserDataModel UserData
        {
            get { return _UserData; }
            set { SetProperty(ref _UserData, value); }
        }

        private readonly ExposureNotificationService exposureNotificationService;

        private readonly UserDataService userDataService;
        public ChatbotPageViewModel(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            Title = AppResources.SettingsPageTitle;
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
            await userDataService.SetAsync(_UserData);
        });


        public ICommand OnChangeNotificationState => new Command(async () =>
        {
            await userDataService.SetAsync(_UserData);
        });

        public ICommand OnChangeResetData => new Command(async () =>
        {
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
                return;
            }
        });
    }
}
