// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SubmitDiagnosisKeysCompletePageViewModel : ViewModelBase
    {
        public SubmitDiagnosisKeysCompletePageViewModel(
            INavigationService navigationService
            ) : base(navigationService)
        {
        }

        public Command OnToHomeButton => new Command(async () =>
        {
            //_ = await NavigationService.NavigateAsync(Destination.HomePage.ToPath());
        });
    }
}
