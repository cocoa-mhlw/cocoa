using System.Collections.Generic;
using System.ComponentModel;
using Covid19Radar.Model;
using Covid19Radar.Resx;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HomePageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public List<HomeMenuModel> HomeMenus { get; private set; }

        public HomePageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = AppResources.HomeTitle;
            SetData();
        }

        private void SetData()
        {
            HomeMenus = new List<HomeMenuModel>
            {
                new HomeMenuModel
                {
                    Title=AppResources.StatusSettingsMenu,
                    Command=OnClickUserSetting,
                },
                new HomeMenuModel
                {
                    Title=AppResources.ListOfContributorsMenu,
                    Command=OnClickAcknowledgments
                },
                new HomeMenuModel
                {
                    Title=AppResources.UpdateInformationMenu,
                    Command=OnClickUpateInfo
                },
                new HomeMenuModel
                {
                    Title=AppResources.DetectedBeaconListMenu,
                    Command=OnClickDetectedBeacon
                },
                new HomeMenuModel
                {
                    Title=AppResources.LicenseAgreementMenu,
                    Command=OnClickLicenseAgreement
                }
            };
        }

        

        public Command OnClickUserSetting => new Command(() =>
        {
            NavigationService.NavigateAsync("UserSettingPage");
        });
        public Command OnClickAcknowledgments => new Command(() =>
        {
            NavigationService.NavigateAsync("ContributersPage");
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
    }
}
