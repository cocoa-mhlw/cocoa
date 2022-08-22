/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.ObjectModel;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Views;
using Prism.Commands;
using Prism.Navigation;

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
            Title = AppResources.HelpMenuPageTitle;
            MenuItems = new ObservableCollection<MainMenuModel>();
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf105",
                PageName = nameof(HelpPage1),
                Title = AppResources.HelpMenuPageLabel1,
                ReadText = $"{AppResources.HelpMenuPageLabel1} {AppResources.Button}"
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf105",
                PageName = nameof(HelpPage2),
                Title = AppResources.HelpMenuPageLabel2,
                ReadText = $"{AppResources.HelpMenuPageLabel2} {AppResources.Button}"
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf105",
                PageName = nameof(HelpPage3),
                Title = AppResources.HelpMenuPageLabel3,
                ReadText = $"{AppResources.HelpMenuPageLabel3} {AppResources.Button}"
            });
            MenuItems.Add(new MainMenuModel()
            {
                Icon = "\uf105",
                PageName = nameof(HelpPage4),
                Title = AppResources.HelpMenuPageLabel4,
                ReadText = $"{AppResources.HelpMenuPageLabel4} {AppResources.Button}"
            });

            NavigateCommand = new DelegateCommand(Navigate);
        }

        async void Navigate()
        {
            await NavigationService.NavigateAsync(SelectedMenuItem.PageName);
            SelectedMenuItem = null;
            return;
        }
    }
}
