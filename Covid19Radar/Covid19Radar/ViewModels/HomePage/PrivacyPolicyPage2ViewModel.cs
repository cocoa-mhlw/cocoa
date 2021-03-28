/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using Covid19Radar.Resources;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class PrivacyPolicyPage2ViewModel : ViewModelBase
    {
        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public PrivacyPolicyPage2ViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.PrivacyPolicyPageTitle;
            Url = AppResources.UrlPrivacyPolicy;
        }
    }
}
