using System.Collections.Generic;
using System.ComponentModel;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HomePageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public List<HomeMenuModel> HomeMenus { get; private set; }
        private IBeaconService _beaconService;

        public HomePageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = AppResources.HomePageTitle;
            _beaconService = Xamarin.Forms.DependencyService.Resolve<IBeaconService>();
            // Only Call InitializeService! Start automagically!
            _beaconService.InitializeService();

            SetData();
        }

        private void SetData()
        {
            HomeMenus = new List<HomeMenuModel>
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



        public Command OnClickUserSetting => new Command(() =>
        {
            NavigationService.NavigateAsync("UserSettingPage");
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
    }
}
