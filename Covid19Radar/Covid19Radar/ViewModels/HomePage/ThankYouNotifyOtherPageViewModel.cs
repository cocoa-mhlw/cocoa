/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Common;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ThankYouNotifyOtherPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;

        public ThankYouNotifyOtherPageViewModel(INavigationService navigationService, ILoggerService loggerService) : base(navigationService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
            this.loggerService = loggerService;
        }
        public Command OnClickShareApp => new Command(() =>
        {
            loggerService.StartMethod();

            AppUtils.PopUpShare();

            loggerService.EndMethod();
        });

    }
}
