// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using Xamarin.Forms;

namespace Covid19Radar.Views
{
    public partial class ManageExposureDataPage : ContentPage
    {
        public ManageExposureDataPage()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            ManageExposureDataPageTitle.AutomationId = "ManageExposureDataPageTitle";
            ManageExposureDataPageClear.AutomationId = "ManageExposureDataPageClear";
            ManageExposureDataPageLow.AutomationId = "ManageExposureDataPageLow";
            ManageExposureDataPageHigh.AutomationId = "ManageExposureDataPageHigh";
#endif
        }
    }
}
