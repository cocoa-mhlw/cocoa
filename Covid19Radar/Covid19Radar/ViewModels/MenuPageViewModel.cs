using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using Covid19Radar.Model;
using Covid19Radar.Views;

namespace Covid19Radar.ViewModels
{
    public class MenuPageViewModel : ViewModelBase
    {
        //private INavigationService _navigationService;

        public ObservableCollection<MainMenuModel> MenuItems { get; set; }

        private MainMenuModel selectedMenuItem;
        public MainMenuModel SelectedMenuItem
        {
            get => selectedMenuItem;
            set => SetProperty(ref selectedMenuItem, value);
        }

        public DelegateCommand NavigateCommand { get; private set; }

        public MenuPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            MenuItems = new ObservableCollection<MainMenuModel>();

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0f3",
                PageName = nameof(StartTutorialPage),
                Title = "アプリの使い方"
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0f3",
                PageName = nameof(SettingsPage),
                Title = "追跡情報の設定"
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf2f1",
                PageName = nameof(UpdateInfomationPage),
                Title = "更新情報"
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf56c",
                PageName = nameof(LicenseAgreementPage),
                Title = "ライセンス"
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0c0",
                PageName = nameof(ContributorsPage),
                Title = "貢献者一覧"
            });

            NavigateCommand = new DelegateCommand(Navigate);
        }

        async void Navigate()
        {
            await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + SelectedMenuItem.PageName);
            //await NavigationService.NavigateAsync(nameof(NavigationPage) + "/" + SelectedMenuItem.PageName);
        }

    }
}
