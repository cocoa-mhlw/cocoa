/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Radar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendLogConfirmationPage : ContentPage
    {
        public const string LogIdKey = "logIdKey";
        public const string ZipFilePathKey = "zipFilePathKey";

        public SendLogConfirmationPage()
        {
            InitializeComponent();
        }

        public static INavigationParameters BuildNavigationParams(
            string logId,
            string zipFilePath,
            INavigationParameters? baseParam = null
            )
        {
            var param = new NavigationParameters();
            param.CopyFrom(baseParam);

            param.Add(LogIdKey, logId);
            param.Add(ZipFilePathKey, zipFilePath);

            return param;
        }

    }
}
