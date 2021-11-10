/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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

        public static void PrepareNavigationParams(
            Model.TermsUpdateInfoModel.Detail privacyPolicyInfo,
            Destination destination,
            INavigationParameters param
            )
        {
            param.Add(UpdatePrivacyPolicyInfoKey, privacyPolicyInfo);
            param.Add(DestinationKey, destination);
        }

        public ReAgreePrivacyPolicyPage()
        {
            InitializeComponent();
        }
    }
}
