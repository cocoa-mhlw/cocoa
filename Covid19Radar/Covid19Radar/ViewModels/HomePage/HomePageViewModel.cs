using System.Collections.Generic;
using System.ComponentModel;
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
    public class HomePageViewModel : ViewModelBase, INotifyPropertyChanged
    {
//        public List<HomeMenuModel> HomeMenus { get; private set; }
        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public HomePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.HomePageTitle;
            Url = Resources.AppResources.UrlUpdate;
            //SetData();
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
            var check = await UserDialogs.Instance.ConfirmAsync("Could you reset all data?", "Reset All Data", "OK","Cancel");
            if (check)
            {
                UserDialogs.Instance.ShowLoading("Deleting data");

                // TODO Exposure notification reset all data

                UserDialogs.Instance.HideLoading();

                await UserDialogs.Instance.ConfirmAsync("The data has been reset.", "Reset", "OK");
            }

        });


        /*
        private void SetData()
        {
            HomeMenus = new List<HomeMenuModel>
            {
                new HomeMenuModel
                {
                    Title=AppResources.HomePageViewStatusSettingsMenu,
                    Command=OnClickUserSetting
                },
                new HomeMenuModel
                {
                    Title=AppResources.HomePageViewListOfContributorsMenu,
                    Command=OnClickAcknowledgments
                },
                new HomeMenuModel
                {
                    Title=AppResources.TitleUpdateInformation,
                    Command=OnClickUpateInfo
                },
                new HomeMenuModel
                {
                    Title=AppResources.DetectedBeaconListMenu,
                    Command=OnClickDetectedBeacon
                },
                new HomeMenuModel
                {
                    Title=AppResources.TitleLicenseAgreement,
                    Command=OnClickLicenseAgreement
                }
            };
        }

        public Command OnClickUserSetting => new Command(() =>
        {
            NavigationService.NavigateAsync("NotifyOthersPage");
        });
        public Command OnClickAcknowledgments => new Command(() =>
        {
            NavigationService.NavigateAsync("ContributorsPage");
        });

        public Command OnClickUpateInfo => new Command(() =>
        {
            NavigationService.NavigateAsync("UpdateInfoPage");
        });
        public Command OnClickDetectedBeacon => new Command(() =>
        {
            NavigationService.NavigateAsync("DetectedBeaconPage");
        });
        public Command OnClickLicenseAgreement => new Command(() =>
        {
            NavigationService.NavigateAsync("LicenseAgreementPage");
        });
        */
    }
}
