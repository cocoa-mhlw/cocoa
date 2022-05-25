/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HelpPage3 : ContentPage
    {
        public HelpPage3()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            HelpPage3Title.AutomationId = "HelpPage3Title";
            HelpPage3ScrollView.AutomationId = "HelpPage3ScrollView";
            HelpPage3Btn.AutomationId = "HelpPage3Btn";
#endif
        }
    }
}
