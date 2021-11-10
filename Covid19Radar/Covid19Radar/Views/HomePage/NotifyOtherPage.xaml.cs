/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Common;
using Covid19Radar.ViewModels;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.RegularExpressions;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotifyOtherPage : ContentPage
    {
        public const string ProcessingNumberKey = "processingNumber";

        public NotifyOtherPage()
        {
            InitializeComponent();
        }

        public static NavigationParameters BuildNavigationParams(
            string processingNumber,
            NavigationParameters param
            )
        {
            param.Add(ProcessingNumberKey, processingNumber);
            return param;
        }

        void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var button = (RadioButton)sender;
            if (button.IsChecked)
            {
                (BindingContext as NotifyOtherPageViewModel).OnClickRadioButtonIsTrueCommand(button.Text);
            }
        }
    }
}
