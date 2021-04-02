/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HelpPage4ViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;

        public HelpPage4ViewModel(INavigationService navigationService, ILoggerService loggerService) : base(navigationService)
        {
            Title = Resources.AppResources.HelpPage4Title;
            this.loggerService = loggerService;
        }

        public Command OnClickSetting => new Command(async () =>
        {
            loggerService.StartMethod();

            await NavigationService.NavigateAsync(nameof(SettingsPage));

            loggerService.EndMethod();
        });
    }
}