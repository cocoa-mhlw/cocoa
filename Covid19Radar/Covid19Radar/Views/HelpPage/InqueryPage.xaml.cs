/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Prism.Navigation;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class InqueryPage : ContentPage, INavigationAware
    {
        public InqueryPage()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            InqueryPageTitle.AutomationId = "InqueryPageTitle";
            InqueryPageTitleOpenLink.AutomationId = "InqueryPageTitleOpenLink";
#endif
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SemanticExtensions.SetSemanticFocus(this);
                });
            }
        }
    }
}
