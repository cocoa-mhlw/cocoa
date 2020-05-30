using System;
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

        private bool _IsExposureNotificationEnabled;

        public bool IsExposureNotificationEnabled
        {
            get { return exposureNotificationService.GetExposureNotificationStatus(); }
            set {
                exposureNotificationService.SetExposureNotificationStatusAsync(value);
                SetProperty(ref _IsExposureNotificationEnabled, value);
            }
        }

        private string _IsNotificationEnabledText;

        public string OnEnableNotificationText
        {
            get { return _IsNotificationEnabledText; }
            set
            {
                SetProperty(ref _IsNotificationEnabledText, value);
            }
        }

        private string _IsExposureNotificationEnabledText;

        public string OnEnableExposureNotificationText
        {
            get { return _IsExposureNotificationEnabledText; }
            set
            {
                SetProperty(ref _IsExposureNotificationEnabledText, value);
            }
        }


        private readonly ExposureNotificationService exposureNotificationService;
        /*
        private readonly UserDataService userDataService;
        public UserDataModel userData;
        */
        public SettingsPageViewModel(INavigationService navigationService, UserDataService userDataService, ExposureNotificationService exposureNotificationService) : base(navigationService, userDataService, exposureNotificationService)
        {
            Title = AppResources.SettingsPageTitle;
            AppVer = AppConstants.AppVersion;
            /*
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
            */
            this.exposureNotificationService = exposureNotificationService;
            OnEnableNotificationText = exposureNotificationService.GetNotificationStatus() ? "Off" : "On";
        }

        public ICommand OnChangeExposureNotificationState => new Command(() =>
        {
            var status = exposureNotificationService.GetExposureNotificationStatus();
            if (status)
            {
                exposureNotificationService.SetExposureNotificationStatusAsync(!status);
                OnEnableNotificationText = "On";
            }
            else
            {
                exposureNotificationService.SetExposureNotificationStatusAsync(!status);
                OnEnableNotificationText = "Off";
            }
        });


        public ICommand OnChangeNotificationState => new Command(() =>
        {
            var status = exposureNotificationService.GetNotificationStatus();
            if (status)
            {
                exposureNotificationService.SetNotificationStatus(!status);
                OnEnableNotificationText = "On";
            }
            else
            {
                exposureNotificationService.SetNotificationStatus(!status);
                OnEnableNotificationText = "Off";
            }
        });

        public ICommand OnChangeResetData => new Command(async () =>
        {
            var check = await UserDialogs.Instance.ConfirmAsync(
                Resources.AppResources.SettingsPageDialogResetText,
                Resources.AppResources.SettingsPageDialogResetTitle,
                Resources.AppResources.ButtonOk,
                Resources.AppResources.ButtonCancel
            );
            /*
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
            */
        });
    }
}
