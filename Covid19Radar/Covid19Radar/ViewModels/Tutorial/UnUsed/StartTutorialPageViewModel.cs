/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Resources;
using Covid19Radar.Renderers;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class StartTutorialPageViewModel : ViewModelBase
    {
        public StartTutorialPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            // TODO: Add Title so we can test it :)
        }

        public Command OnClickStart => new Command(async () =>
        {
            await NavigationService.NavigateAsync(nameof(PrivacyPolicyPage));
        });

    }
}
