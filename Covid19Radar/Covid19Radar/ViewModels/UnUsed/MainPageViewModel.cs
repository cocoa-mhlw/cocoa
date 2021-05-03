/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.ComponentModel;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class MainPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public List<HomeMenuModel> MainMenus { get; private set; }

        public MainPageViewModel(INavigationService navigationService, IStatusBarPlatformSpecific statusBarPlatformSpecific)
            : base(navigationService, statusBarPlatformSpecific)
        {
            Title = "main page";
        }
    }
}
