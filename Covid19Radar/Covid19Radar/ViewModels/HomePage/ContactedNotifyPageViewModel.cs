/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Essentials;
using Covid19Radar.Resources;
using System;

namespace Covid19Radar.ViewModels
{
    public class ContactedNotifyPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private string _exposureCount;
        public string ExposureCount
        {
            get { return _exposureCount; }
            set { SetProperty(ref _exposureCount, value); }
        }
        public string NowDate
        {
            get { return DateTime.Now.ToLocalTime().ToString("D"); }
        }

        public ContactedNotifyPageViewModel(INavigationService navigationService, ILoggerService loggerService, IExposureNotificationService exposureNotificationService) : base(navigationService)
        {
            Title = AppResources.TitileUserStatusSettings;
            this.loggerService = loggerService;
            ExposureCount = exposureNotificationService.GetExposureCountToDisplay().ToString();
        }

        public Command OnClickByForm => new Command(async () =>
        {
            loggerService.StartMethod();

            var uri = AppResources.UrlContactedForm;
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

            loggerService.EndMethod();
        });
    }
}
