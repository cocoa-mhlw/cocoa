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


    }
}
