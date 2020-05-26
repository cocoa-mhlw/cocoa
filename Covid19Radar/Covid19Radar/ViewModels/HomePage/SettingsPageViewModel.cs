using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private string _AppVersion;

        public string AppVer
        {
            get { return _AppVersion; }
            set { SetProperty(ref _AppVersion, value); }
        }

        private bool _EnableExposureNotification;

        public bool EnableExposureNotification
        {
            get { return _EnableExposureNotification; }
            set
            {
                SetProperty(ref _EnableExposureNotification, value);
                RaisePropertyChanged(nameof(EnableExposureNotification));
            }
        }

        private bool _EnableLocalNotification;

        public bool EnableLocalNotification
        {
            get { return _EnableLocalNotification; }
            set
            {
                SetProperty(ref _EnableLocalNotification, value);
                RaisePropertyChanged(nameof(EnableLocalNotification));
            }
        }

        private bool _ResetData;

        public bool ResetData
        {
            get { return _ResetData; }
            set
            {
                SetProperty(ref _ResetData, value);
                RaisePropertyChanged(nameof(ResetData));
            }
        }

        private readonly UserDataService userDataService;
        private UserDataModel userData;

        public SettingsPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = "設定"; //AppResources.SettingsPageTitle;
            AppVer = AppConstants.AppVersion;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
            this.userDataService.UserDataChanged += _userDataChanged;

            EnableExposureNotification = userData.IsExposureNotificationEnabled;
            EnableLocalNotification = userData.IsNotificationEnabled;
        }

        private void _userDataChanged(object sender, UserDataModel e)
        {
            userData = this.userDataService.Get();
        }


        // Switch Behevior
        public ICommand OnChangeEnableExposureNotification => new Command(async () =>
        {
            //await UserDialogs.Instance.AlertAsync("設定を保存するには、Saveをタップしてください");
        });

        public ICommand OnChangeEnableNotification => new Command(async () =>
        {
            //await UserDialogs.Instance.AlertAsync("設定を保存するには、Saveをタップしてください");
        });

        public ICommand OnChangeResetData => new Command(async () =>
        {
            /*
            var check = await UserDialogs.Instance.ConfirmAsync(
                "本当にすべてのデータをリセットしますか?",
                "データの全削除",
                Resources.AppResources.DialogButtonOk,
                Resources.AppResources.DialogButtonCancel
            );

            if (check)
            {
                UserDialogs.Instance.ShowLoading(Resources.AppResources.LoadingTextDeleting);

                if (await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync())
                {
                    await Xamarin.ExposureNotifications.ExposureNotification.StopAsync();
                }

                // Reset All Data and Optout
                UserDataModel userData = new UserDataModel();
                await userDataService.SetAsync(userData);

                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync("全設定とデータを削除しました。アプリの再起動をしてください。");
                Application.Current.Quit();

                // Application close
                Xamarin.Forms.DependencyService.Get<ICloseApplication>().closeApplication();
                return;

            }
            */
        });


        public Command OnSaveClick => new Command(async () =>
        {
            /*
            userData.LastIsEnabled = EnableExposureNotification;
            userData.EnableNotifications = EnableLocalNotification;
            await userDataService.SetAsync(userData);
            await UserDialogs.Instance.AlertAsync("設定を保存しました。");
            */
        });


    }
}
