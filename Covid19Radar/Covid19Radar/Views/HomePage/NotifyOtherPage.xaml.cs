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
        public const string ProcessNumberKey = "processNumber";

        public NotifyOtherPage()
        {
            InitializeComponent();
        }

        public static bool IsValidProcessNumber(string processNumber)
        {
            return Regex.IsMatch(processNumber, AppConstants.LinkQueryValueRegexProcessingNumber);
        }

        public static NavigationParameters BuildNavigationParams(
            string processNumber,
            NavigationParameters param
            )
        {
            param.Add(ProcessNumberKey, processNumber);
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
