/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Model;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReAgreePrivacyPolicyPage : ContentPage
    {
        public const string PrivacyPolicyDetailKey = "privacy_policy_detail";

        public static INavigationParameters BuildNavigationParams(
            TermsUpdateInfoModel.Detail privatcyPolicyDetail
            )
        {
            var param = new NavigationParameters();
            param.Add(PrivacyPolicyDetailKey, privatcyPolicyDetail);
            return param;
        }

        public ReAgreePrivacyPolicyPage()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            ReAgreePrivacyPolicyPageTitle.AutomationId = "ReAgreePrivacyPolicyPageTitle";
#endif
        }
    }
}
