// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TroubleshootingPage : ContentPage
    {
        public TroubleshootingPage()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            TroubleshootingPageTitle.AutomationId = "TroubleshootingPageTitle";
#endif
        }
    }
}
