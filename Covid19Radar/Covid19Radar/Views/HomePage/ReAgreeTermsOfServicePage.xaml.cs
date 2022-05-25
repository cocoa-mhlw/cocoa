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
    public partial class ReAgreeTermsOfServicePage : ContentPage
    {
        public const string UpdateInfoKey = "updateInfo";
        public const string DestinationKey = "destination_reagree_term_of_service";

        public static INavigationParameters BuildNavigationParams(
            TermsUpdateInfoModel updateInfo,
            Destination destination,
            INavigationParameters? baseParam = null
            )
        {
            var param = new NavigationParameters();
            param.CopyFrom(baseParam);

            param.Add(UpdateInfoKey, updateInfo);
            param.Add(DestinationKey, destination);

            return param;
        }

        public ReAgreeTermsOfServicePage()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            ReAgreeTermsOfServicePageTitle.AutomationId = "ReAgreeTermsOfServicePageTitle";
#endif
        }
    }
}
