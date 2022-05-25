/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TutorialPage3 : ContentPage
    {
        public TutorialPage3()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            TutorialPage3Title.AutomationId = "TutorialPage3Title";
#endif
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await activity_indicator.ProgressTo(1.0, 900, Easing.SpringIn);

        }

        public void OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            activity_indicator.IsVisible = true;


        }

        public void OnNavigated(object sender, WebNavigatedEventArgs e)
        {

            activity_indicator.IsVisible = false;

        }
    }
}
