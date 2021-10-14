/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Views;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class SubmitConsentPageViewModel : ViewModelBase
    {
        private bool _isNextButtonVisible = true;
        public bool IsNextButtonVisible
        {
            get { return _isNextButtonVisible; }
            set { SetProperty(ref _isNextButtonVisible, value); }
        }

        public SubmitConsentPageViewModel() : base()
        {
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters != null && parameters.ContainsKey(SubmitConsentPage.IsFromAppLinksKey))
            {
                IsNextButtonVisible = !parameters.GetValue<bool>(SubmitConsentPage.IsFromAppLinksKey);
            }
        }
    }
}