﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Prism.Navigation;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class DebugPage : ContentPage, INavigationAware
    {
        public DebugPage()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
            DebugPageTitle.AutomationId = "DebugPageTitle";
            DebugPageScrollView.AutomationId = "DebugPageScrollView";
            ManageExposureDataPage.AutomationId = "ManageExposureDataPage";
            ManageUserDataPage.AutomationId = "ManageUserDataPage";
            ContactedNotifyPage.AutomationId = "ContactedNotifyPage";
            ReAgreePrivacyPolicyPage.AutomationId = "ReAgreePrivacyPolicyPage";
            ReAgreeTermsOfServicePage.AutomationId = "ReAgreeTermsOfServicePage";
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
