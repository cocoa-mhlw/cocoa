/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Windows.Input;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;

        public string TermsOfUseReadText => $"{AppResources.TermsofservicePageTitle} {AppResources.Button}";
        public string PrivacyPolicyReadText => $"{AppResources.PrivacyPolicyPageTitle} {AppResources.Button}";
        public string WebAccessibilityPolicyReadText => $"{AppResources.WebAccessibilityPolicyPageTitle} {AppResources.Button}";

        public SettingsPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService
            ) : base(navigationService)
        {
            Title = AppResources.SettingsPageTitle;
            this.loggerService = loggerService;
        }

        public ICommand OnClickObtainSourceCode => new Command<string>(async (uri) =>
        {
            loggerService.StartMethod();

            await Browser.OpenAsync(uri, BrowserLaunchMode.External);

            loggerService.EndMethod();
        });
    }
}
