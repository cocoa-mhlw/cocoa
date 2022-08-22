/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendLogCompletePage : ContentPage
    {
        public SendLogCompletePage()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            SendLogCompletePageTitle.AutomationId = "SendLogCompletePageTitle";
#endif
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = BindingContext as SendLogCompletePageViewModel;
            vm.OnBackButtonPressedCommand.Execute(null);
            return true;
        }
    }
}
