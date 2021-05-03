/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class StatusUpdateCompletePageViewModel : ViewModelBase
    {
        public StatusUpdateCompletePageViewModel(INavigationService navigationService, IStatusBarPlatformSpecific statusBarPlatformSpecific) : base(navigationService, statusBarPlatformSpecific)
        {
            Title = AppResources.TitleStatusUpdateComplete;
        }

        public Command OnClickHome => new Command(() => NavigationService.NavigateAsync("/NavigationPage/HomePage"));
    }
}
