﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Renderers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HeadsupPageViewModel : ViewModelBase
    {
        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }
        public HeadsupPageViewModel(INavigationService navigationService, IStatusBarPlatformSpecific statusBarPlatformSpecific) : base(navigationService, statusBarPlatformSpecific)
        {
            Title = Resources.AppResources.TitleHeadsup;
            Url = Resources.AppResources.UrlHeadsup;
        }

        public Command OnClickPrev => (new Command(() =>
        {
            NavigationService.NavigateAsync("HomePage");
        }));


    }
}
