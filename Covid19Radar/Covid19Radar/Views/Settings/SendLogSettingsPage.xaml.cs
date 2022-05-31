// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.Views
{
    public partial class SendLogSettingsPage : ContentPage
    {
        public const string DestinationKey = "destination_send_log_settings";

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

        public SendLogSettingsPage()
        {
            InitializeComponent();
        }
    }
}
