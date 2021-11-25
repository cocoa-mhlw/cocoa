/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubmitConsentPage : ContentPage
    {
        public const string IsFromAppLinksKey = "fromAppLinks";
        public const string ProcessingNumberKey = "prosessingNumber";

        public SubmitConsentPage()
        {
            InitializeComponent();
        }

        public static NavigationParameters BuildNavigationParams(
            bool isFromAppLinks,
            string processingNumber,
            NavigationParameters param
            )
        {
            param.Add(IsFromAppLinksKey, isFromAppLinks);
            param.Add(ProcessingNumberKey, processingNumber);
            return param;
        }

    }
}
