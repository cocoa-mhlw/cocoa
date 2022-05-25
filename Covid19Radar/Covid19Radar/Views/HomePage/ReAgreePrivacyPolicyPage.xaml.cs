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
        public const string UpdatePrivacyPolicyInfoKey = "updatePrivacyPolicyInfo";
        public const string DestinationKey = "destination_reagree_privacy_policy";

        public static INavigationParameters BuildNavigationParams(
            TermsUpdateInfoModel.Detail privacyPolicyInfo,
            Destination destination,
            INavigationParameters? baseParam = null
            )
        {
            var param = new NavigationParameters();
            param.CopyFrom(baseParam);

            param.Add(UpdatePrivacyPolicyInfoKey, privacyPolicyInfo);
            param.Add(DestinationKey, destination);

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
