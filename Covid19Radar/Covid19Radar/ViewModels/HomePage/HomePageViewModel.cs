using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HomePageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private string _AppVersion;

        public string AppVer
        {
            get { return _AppVersion; }
            set { SetProperty(ref _AppVersion, value); }
        }

        public HomePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.HomePageTitle;
            AppVer = AppConstants.AppVersion;
        }

        public bool EnableNotifications
        {
            get => LocalStateManager.Instance.EnableNotifications;
            set
            {
                LocalStateManager.Instance.EnableNotifications = value;
                LocalStateManager.Save();
            }
        }

        public Command OnResetClick => new Command(async () =>
        {
            var check = await UserDialogs.Instance.ConfirmAsync("Could you reset all data?", "Reset All Data", "OK", "Cancel");
            if (check)
            {
                UserDialogs.Instance.ShowLoading("Deleting data");

                // TODO Exposure notification reset all data

                UserDialogs.Instance.HideLoading();

                await UserDialogs.Instance.ConfirmAsync("The data has been reset.", "Reset", "OK");
            }

        });
    }
}
