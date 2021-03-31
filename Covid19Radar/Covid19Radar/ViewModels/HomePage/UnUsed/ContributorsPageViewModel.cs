/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class ContributorsPageViewModel : ViewModelBase
    {
        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }
        public ContributorsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.TitleContributorsPage;
            Url = Resources.AppResources.UrlContributor;
        }
    }
}
