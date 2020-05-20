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
    public class SettingsPageViewModel : ViewModelBase, INotifyPropertyChanged
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

        public SettingsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.HomePageTitle;
            AppVer = AppConstants.AppVersion;
        }


        Task Disabled()
        {
            LocalStateManager.Instance.LastIsEnabled = false;
            LocalStateManager.Instance.IsWelcomed = false;
            LocalStateManager.Save();
            return NavigationService.NavigateAsync(nameof(HomePage));
        }


        // Switch Behevior
        public ICommand OnChangeEnableExposureNotification => new Command(async () =>
        {
            
        });

        public ICommand OnChangeEnableNotification => new Command(async () =>
        {

        });

        public ICommand OnChangeResetData => new Command(async () =>
        {

        });


        public Command OnSaveClick => new Command(async () =>
        {
            if (ResetData)
            {
                var check = await UserDialogs.Instance.ConfirmAsync("Could you reset all data?", "Reset All Data", "OK", "Cancel");
                if (check)
                {
                    UserDialogs.Instance.ShowLoading("Deleting data");

                    // TODO Exposure notification reset all data

                    UserDialogs.Instance.HideLoading();

                    await UserDialogs.Instance.ConfirmAsync("The data has been reset.", "Reset", "OK");
                }
            }
        });


    }
}
