// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Xamarin.Forms;

namespace Covid19Radar.Views
{
    public partial class ManageUserDataPage : ContentPage
    {
        public ManageUserDataPage()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            ManageUserDataPageTitle.AutomationId = "ManageUserDataPageTitle";
            ManageUserDataPageReset.AutomationId = "ManageUserDataPageReset";
            ManageUserDataPage1day.AutomationId = "ManageUserDataPage1day";
            ManageUserDataPage14day.AutomationId = "ManageUserDataPage14day";
            ManageUserDataPage15day.AutomationId = "ManageUserDataPage15day";
#endif
        }
    }
}
