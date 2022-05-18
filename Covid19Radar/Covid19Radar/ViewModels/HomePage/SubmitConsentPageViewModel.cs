/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Resources;
using Covid19Radar.Views;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class SubmitConsentPageViewModel : ViewModelBase
    {
        private bool _isDeepLink = false;
        public bool IsDeepLink
        {
            get => _isDeepLink;
            set => SetProperty(ref _isDeepLink, value);
        }

        private bool _isProcessingNumberVisible = false;
        public bool IsProcessingNumberVisible
        {
            get => _isProcessingNumberVisible;
            set => SetProperty(ref _isProcessingNumberVisible, value);
        }

        private string _processingNumber = "";
        public string ProcessingNumber
        {
            get => _processingNumber;
            set => SetProperty(ref _processingNumber, value);
        }

        private bool _isNextButtonVisible = true;
        public bool IsNextButtonVisible
        {
            get => _isNextButtonVisible;
            set => SetProperty(ref _isNextButtonVisible, value);
        }

        public SubmitConsentPageViewModel() : base()
        {
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters != null)
            {
                if (parameters.ContainsKey(SubmitConsentPage.IsFromAppLinksKey))
                {
                    var isFromAppLinksKey = parameters.GetValue<bool>(SubmitConsentPage.IsFromAppLinksKey);
                    IsDeepLink = isFromAppLinksKey;
                    IsProcessingNumberVisible = isFromAppLinksKey;
                    IsNextButtonVisible = !isFromAppLinksKey;
                }
                if (parameters.ContainsKey(SubmitConsentPage.ProcessingNumberKey))
                {
                    ProcessingNumber = parameters.GetValue<string>(SubmitConsentPage.ProcessingNumberKey);
                }
            }
        }
    }
}
