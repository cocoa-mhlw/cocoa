/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class HelpPage1ViewModel : ViewModelBase
    {
        public HelpPage1ViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.HelpPage1Title;
        }
    }
}
