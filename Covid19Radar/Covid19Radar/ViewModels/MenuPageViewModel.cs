/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Prism.Commands;
using Prism.Navigation;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Covid19Radar.Model;
using Covid19Radar.Views;
using Xamarin.Forms.Internals;

namespace Covid19Radar.ViewModels
{
    public class MenuPageViewModel : ViewModelBase
    {
        private const string MenuIconColorDefault = "#066AB9";
        private const string MenuIconColorSelected = "#FFF";
        private const string MenuTextColorDefault = "#000";
        private const string MenuTextColorSelected = "#FFF";

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
                Icon = "\uf965",
                PageName = nameof(HomePage),
                Title = Resources.AppResources.HomePageTitle,
                IconColor = MenuIconColorDefault,
                TextColor = MenuTextColorDefault
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf013",
                PageName = nameof(SettingsPage),
                Title = Resources.AppResources.SettingsPageTitle,
                IconColor = MenuIconColorDefault,
                TextColor = MenuTextColorDefault
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0e0",
                PageName = nameof(InqueryPage),
                Title = Resources.AppResources.InqueryPageTitle_Menu,
                IconColor = MenuIconColorDefault,
                TextColor = MenuTextColorDefault
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0eb",
                PageName = nameof(HelpMenuPage),
                Title = Resources.AppResources.HelpMenuPageMenu,
                IconColor = MenuIconColorDefault,
                TextColor = MenuTextColorDefault
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf70e",
                PageName = nameof(TermsofservicePage),
                Title = Resources.AppResources.TermsofservicePageTitle,
                IconColor = MenuIconColorDefault,
                TextColor = MenuTextColorDefault
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf70e",
                PageName = nameof(PrivacyPolicyPage2),
                Title = Resources.AppResources.PrivacyPolicyPageTitle,
                IconColor = MenuIconColorDefault,
                TextColor = MenuTextColorDefault
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf70e",
                PageName = nameof(WebAccessibilityPolicyPage),
                Title = Resources.AppResources.WebAccessibilityPolicyPageTitle,
                IconColor = MenuIconColorDefault,
                TextColor = MenuTextColorDefault
            });

            NavigateCommand = new DelegateCommand(Navigate);
        }

        async void Navigate()
        {
            ClearSelectedItem();
            SelectedMenuItem.IconColor = MenuIconColorSelected;
            SelectedMenuItem.TextColor = MenuTextColorSelected;
            await NavigationService.NavigateAsync(nameof(NavigationPage) + "/" + SelectedMenuItem.PageName);
            return;
        }

        private void ClearSelectedItem()
        {
            MenuItems.ForEach(item =>
            {
                item.IconColor = MenuIconColorDefault;
                item.TextColor = MenuTextColorDefault;
            });
        }

        public Command OnCloseButton => new Command(async () =>
        {
            var currentMenuItem = SelectedMenuItem ?? MenuItems[0];
            await NavigationService.NavigateAsync(nameof(NavigationPage) + "/" + currentMenuItem.PageName);
        });
    }
}
