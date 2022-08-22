/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Prism.Navigation;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class SettingsPage : ContentPage, INavigationAware
    {
        public SettingsPage()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            SettingsPageTitle.AutomationId = "SettingsPageTitle";
            SettingsPageTitleScrollView.AutomationId = "SettingsPageTitleScrollView";
            OpenLicenseAgreementPageTitle.AutomationId = "OpenLicenseAgreementPageTitle";
            OpenTermsofservicePageTitle.AutomationId = "OpenTermsofservicePageTitle";
            OpenPrivacyPolicyPageTitle.AutomationId = "OpenPrivacyPolicyPageTitle";
            OpenWebAccessibilityPolicyPageTitle.AutomationId = "OpenWebAccessibilityPolicyPageTitle";
            OpenGitHub.AutomationId = "OpenGitHub";
#endif
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SemanticExtensions.SetSemanticFocus(this);
                });
            }
        }
    }
}
