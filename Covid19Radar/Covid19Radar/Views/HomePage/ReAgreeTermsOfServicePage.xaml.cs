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
        public const string TermsOfServiceDetailKey = "terms_of_service_detail";

        public static INavigationParameters BuildNavigationParams(
            TermsUpdateInfoModel.Detail termsOfServiceDetail)
        {
            var param = new NavigationParameters();
            param.Add(TermsOfServiceDetailKey, termsOfServiceDetail);
            return param;
        }

        public ReAgreeTermsOfServicePage()
        {
            InitializeComponent();
        }
    }
}
