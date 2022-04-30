// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HowToEnableExposureNotificationsPageViewModel : ViewModelBase
    {
        private readonly IExternalNavigationService _externalNavigationService;

        public HowToEnableExposureNotificationsPageViewModel(
            INavigationService navigationService,
            IExternalNavigationService externalNavigationService
            ) : base(navigationService)
        {
            _externalNavigationService = externalNavigationService;
        }

        public Command OnExposureNotificationSettingButton => new Command(() =>
        {
            _externalNavigationService.NavigateAppSettings();
        });
    }
}
