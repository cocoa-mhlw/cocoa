/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Diagnostics;
using Covid19Radar.Common;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly IUserDataService userDataService;
        private readonly IExposureNotificationService exposureNotificationService;

        private string _startDate;
        private string _pastDate;

        public string StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); }
        }
        public string PastDate
        {
            get { return _pastDate; }
            set { SetProperty(ref _pastDate, value); }
        }

        public HomePageViewModel(INavigationService navigationService, ILoggerService loggerService, IUserDataService userDataService, IExposureNotificationService exposureNotificationService) : base(navigationService)
        {
            Title = AppResources.HomePageTitle;
            this.loggerService = loggerService;
            this.userDataService = userDataService;
            this.exposureNotificationService = exposureNotificationService;
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            loggerService.StartMethod();

            var startDate = userDataService.GetStartDate();
            StartDate = startDate.ToLocalTime().ToString("D");

            var daysOfUse = userDataService.GetDaysOfUse();
            PastDate = daysOfUse.ToString();


            // Check Version
            AppUtils.CheckVersion(loggerService);
            try
            {
                await exposureNotificationService.StartExposureNotification();
                await exposureNotificationService.FetchExposureKeyAsync();

                var statusMessage = await exposureNotificationService.UpdateStatusMessageAsync();
                loggerService.Info($"Exposure notification status: {statusMessage}");

                base.Initialize(parameters);

                loggerService.EndMethod();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                loggerService.Exception("Failed to exposure notification status.", ex);
                loggerService.EndMethod();
            }
        }

        public Command OnClickExposures => new Command(async () =>
        {
            loggerService.StartMethod();

            try // almost the same as try-catch in Initialize()
            {
                await exposureNotificationService.StartExposureNotification();
                await exposureNotificationService.FetchExposureKeyAsync();

                var statusMessage = await exposureNotificationService.UpdateStatusMessageAsync();
                loggerService.Info($"Exposure notification status: {statusMessage}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                loggerService.Exception("Failed to exposure notification status.", ex);
            }

            var count = exposureNotificationService.GetExposureCount();
            loggerService.Info($"Exposure count: {count}");
            if (count > 0)
            {
                await NavigationService.NavigateAsync(nameof(ContactedNotifyPage));
                loggerService.EndMethod();
                return;
            }
            else
            {
                await NavigationService.NavigateAsync(nameof(NotContactPage));
                loggerService.EndMethod();
                return;
            }
        });

        public Command OnClickShareApp => new Command(() =>
       {
           loggerService.StartMethod();

           AppUtils.PopUpShare();

           loggerService.EndMethod();
       });
    }
}
