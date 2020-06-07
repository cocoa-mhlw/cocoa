using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Views;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HelpMenuPageViewModel : ViewModelBase
    {
        public ObservableCollection<MainMenuModel> MenuItems { get; set; }

        private MainMenuModel selectedMenuItem;
        public MainMenuModel SelectedMenuItem
        {
            get => selectedMenuItem;
            set => SetProperty(ref selectedMenuItem, value);
        }

        public DelegateCommand NavigateCommand { get; private set; }

        public HelpMenuPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "使い方";// Resources.AppResources.HelpMenuPageTitle;
            MenuItems = new ObservableCollection<MainMenuModel>();
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf105",
                PageName = nameof(HelpPage1),
                Title = Resources.AppResources.HelpMenuPageLabel1
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf105",
                PageName = nameof(HelpPage2),
                Title = "接触の有無はどのように知ることができますか？"//Resources.AppResources.HelpPage2Title
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf105",
                PageName = nameof(HelpPage3),
                Title = "新型コロナウィルスに感染していると判定されたら"//Resources.AppResources.HelpPage3Title
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf105",
                PageName = nameof(HelpPage4),
                Title = "接触の記録を停止／情報を削除するには"//Resources.AppResources.HelpPage4Title
            });

            NavigateCommand = new DelegateCommand(Navigate);
        }

        async void Navigate()
        {
            await NavigationService.NavigateAsync(SelectedMenuItem.PageName);
            return;
        }
    }
}