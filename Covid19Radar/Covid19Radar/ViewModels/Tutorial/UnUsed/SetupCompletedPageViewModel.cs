/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using System.Windows.Input;
using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SetupCompletedPageViewModel : ViewModelBase
    {
        public SetupCompletedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.TitleSetupCompleted;
        }

        public Command OnClickToMain => new Command(async () =>
        {
            
            await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage), useModalNavigation: true);
        });

    }
}
