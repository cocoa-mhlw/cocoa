/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TutorialPage6 : ContentPage
    {
        public TutorialPage6()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            TutorialPage6Title.AutomationId = "TutorialPage6Title";
#endif
        }
    }
}
