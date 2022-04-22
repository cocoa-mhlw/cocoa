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
    public partial class HelpMenuPage : ContentPage, INavigationAware
    {
        public HelpMenuPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            list_view.SelectedItem = null;
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
