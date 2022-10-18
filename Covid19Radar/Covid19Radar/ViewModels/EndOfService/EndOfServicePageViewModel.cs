// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using Covid19Radar.Common;
using Covid19Radar.Resources;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels.EndOfService
{
    public class EndOfServicePageViewModel : ViewModelBase
    {
        public string CheckDetailsLinkReadText => $"{AppResources.EndOfServicePageTextLink} {AppResources.Button}";

        public EndOfServicePageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public Command CheckDetailsCommand => new Command(async () =>
        {
            await Browser.OpenAsync(AppConstants.EndOfServiceCheckDetailsUrl, BrowserLaunchMode.SystemPreferred);
        });
    }
}

