/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.IO;
using Covid19Radar.Services;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class LicenseAgreementPageViewModel : ViewModelBase
    {
        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public LicenseAgreementPageViewModel(
            INavigationService navigationService,
            ILocalContentService localContentService
            ) : base(navigationService)
        {
            Title = Resources.AppResources.TitleLicenseAgreement;
            Url = Path.Combine(localContentService.GetLocalContentUrl(), AppSettings.Instance.LicenseFilename);
        }
    }
}
