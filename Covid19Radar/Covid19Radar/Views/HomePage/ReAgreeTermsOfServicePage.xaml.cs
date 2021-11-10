/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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

        public static void PrepareNavigationParams(
            Model.TermsUpdateInfoModel updateInfo,
            Destination destination,
            INavigationParameters param
            )
        {
            param.Add(UpdateInfoKey, updateInfo);
            param.Add(DestinationKey, destination);
        }

        public ReAgreeTermsOfServicePage()
        {
            InitializeComponent();
        }
    }
}
