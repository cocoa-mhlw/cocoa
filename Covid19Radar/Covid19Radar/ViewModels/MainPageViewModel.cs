using System.Collections.Generic;
using System.ComponentModel;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class MainPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public List<HomeMenuModel> MainMenus { get; private set; }

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "main page";

            //SetData();
        }
        /*
        private void SetData()
        {
            MainMenus = new List<HomeMenuModel>
            {
                new HomeMenuModel
                {
                    Title=AppResources.HomePageViewStatusSettingsMenu,
                    Command=OnClickUserSetting,
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
        */
        /*
                public Command OnClickUserSetting => new Command(() =>
                {
                    NavigationService.NavigateAsync("UserStatusPage");
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
