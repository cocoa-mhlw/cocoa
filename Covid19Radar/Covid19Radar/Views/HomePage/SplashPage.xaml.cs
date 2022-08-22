/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashPage : ContentPage
    {
        public const string DestinationKey = "destination_splash";

        public static INavigationParameters BuildNavigationParams(
            Destination destination,
            INavigationParameters? baseParam = null
            )
        {
            var param = new NavigationParameters();
            param.CopyFrom(baseParam);

            param.Add(DestinationKey, destination);

            return param;
        }

        public SplashPage()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            SplashPageTitle.AutomationId = "SplashPageTitle";
#endif
        }
    }
}
