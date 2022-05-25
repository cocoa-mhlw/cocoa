/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TutorialPage1 : ContentPage
    {
        public TutorialPage1()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            TutorialPage1Title.AutomationId = "TutorialPage1Title";
            TutorialPage1ScrollView.AutomationId = "TutorialPage1ScrollView";
            TutorialPage2Btn.AutomationId = "TutorialPage2Btn";
#endif
        }
    }
}
