/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using Prism.Commands;
using Prism.Navigation;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Covid19Radar.Model;
using Covid19Radar.Views;
using System.Diagnostics;
using Xamarin.Forms.Internals;

namespace Covid19Radar.ViewModels
{
    public class MenuPageViewModel : ViewModelBase
    {
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
                IconColor = "#019AE8",
                TextColor = "#000"
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf013",
                PageName = nameof(SettingsPage),
                Title = Resources.AppResources.SettingsPageTitle,
                IconColor = "#019AE8",
                TextColor = "#000"
            });

            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf0e0",
                PageName = nameof(InqueryPage),
                Title = Resources.AppResources.InqueryPageTitle,
                IconColor = "#019AE8",
                TextColor = "#000"
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf70e",
                PageName = nameof(TermsofservicePage),
                Title = Resources.AppResources.TermsofservicePageTitle,
                IconColor = "#019AE8",
                TextColor = "#000"
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf70e",
                PageName = nameof(PrivacyPolicyPage2),
                Title = Resources.AppResources.PrivacyPolicyPageTitle,
                IconColor = "#019AE8",
                TextColor = "#000"
            });

            NavigateCommand = new DelegateCommand(Navigate);
        }

        async void Navigate()
        {
            ClearSelectedItem();
            SelectedMenuItem.IconColor = "#FFF";
            SelectedMenuItem.TextColor = "#FFF";
            await NavigationService.NavigateAsync(nameof(NavigationPage) + "/" + SelectedMenuItem.PageName);
            return;
        }

        private void ClearSelectedItem()
        {
            MenuItems.ForEach(item =>
            {
                item.IconColor = "#019AE8";
                item.TextColor = "#000";
            });
        }

    }
}
