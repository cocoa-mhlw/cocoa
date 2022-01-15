/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Essentials;
using Covid19Radar.Resources;
using Covid19Radar.Repository;
using Covid19Radar.Common;
using System.Linq;
using Covid19Radar.Services;
using Chino;
using System;
using Acr.UserDialogs;

namespace Covid19Radar.ViewModels
{
    public class ContactedNotifyPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly IExposureDataRepository _exposureDataRepository;
        private readonly IExposureRiskCalculationService _exposureRiskCalculationService;
        private readonly IDialogService _dialogService;

        private readonly IExposureRiskCalculationConfigurationRepository _exposureRiskCalculationConfigurationRepository;

        private string _exposureDurationInMinutes;
        public string ExposureDurationInMinutes
        {
            get { return _exposureDurationInMinutes; }
            set { SetProperty(ref _exposureDurationInMinutes, value); }
        }

        private string _exposureCount;
        public string ExposureCount
        {
            get { return _exposureCount; }
            set { SetProperty(ref _exposureCount, value); }
        }

        public ContactedNotifyPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IExposureDataRepository exposureDataRepository,
            IExposureRiskCalculationService exposureRiskCalculationService,
            IExposureRiskCalculationConfigurationRepository exposureRiskCalculationConfigurationRepository,
            IDialogService dialogService
            ) : base(navigationService)
        {
            this.loggerService = loggerService;
            _exposureDataRepository = exposureDataRepository;
            _exposureRiskCalculationService = exposureRiskCalculationService;
            _exposureRiskCalculationConfigurationRepository = exposureRiskCalculationConfigurationRepository;
            _dialogService = dialogService;

            Title = AppResources.TitileUserStatusSettings;
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            try
            {
                loggerService.EndMethod();

                var exposureRiskCalculationConfiguration
                    = await _exposureRiskCalculationConfigurationRepository.GetExposureRiskCalculationConfigurationAsync(preferCache: false);

                var userExposureInformationList = _exposureDataRepository.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay);

                string contactedNotifyPageCountFormat = AppResources.ContactedNotifyPageCountOneText;
                if (userExposureInformationList.Count() > 1)
                {
                    contactedNotifyPageCountFormat = AppResources.ContactedNotifyPageCountText;
                }

                var dailySummaryList = await _exposureDataRepository.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay);
                var dailySummaryMap = dailySummaryList.ToDictionary(ds => ds.GetDateTime());
                var exposureWindowList = await _exposureDataRepository.GetExposureWindowsAsync(AppConstants.DaysOfExposureInformationToDisplay);

                int dayCount = 0;
                long exposureDurationInSec = 0;
                foreach (var ew in exposureWindowList.GroupBy(exposureWindow => exposureWindow.GetDateTime()))
                {
                    var dailySummary = dailySummaryMap[ew.Key];

                    RiskLevel riskLevel = _exposureRiskCalculationService.CalcRiskLevel(dailySummary, ew.ToList(), exposureRiskCalculationConfiguration);
                    if (riskLevel >= RiskLevel.High)
                    {
                        exposureDurationInSec += ew.Sum(e => e.ScanInstances.Sum(s => s.SecondsSinceLastScan));
                        dayCount += 1;
                    }
                }

                string contactedNotifyPageExposureDurationFormat = AppResources.ContactedNotifyPageExposureDurationOne;
                if (dayCount > 1)
                {
                    contactedNotifyPageExposureDurationFormat = AppResources.ContactedNotifyPageExposureDuration;
                }
                TimeSpan timeSpan = TimeSpan.FromSeconds(exposureDurationInSec);
                var totalMinutes = Math.Ceiling(timeSpan.TotalMinutes);

                if (userExposureInformationList.Count() > 0 && dayCount > 0)
                {
                    // Show Headers
                    var beforeDateMillisSinceEpoch = userExposureInformationList.Max(ei => ei.Timestamp.ToUnixEpochMillis());
                    var afterDateMillisSinceEpoch = dailySummaryList.Min(ds => ds.DateMillisSinceEpoch);

                    var beforeDate = DateTimeOffset.UnixEpoch.AddMilliseconds(beforeDateMillisSinceEpoch).UtcDateTime;
                    var afterDate = DateTimeOffset.UnixEpoch.AddMilliseconds(afterDateMillisSinceEpoch).UtcDateTime;

                    ExposureCount = string.Format(AppResources.ContactedNotifyPageCountHeader, beforeDate.ToString("D")) + "\n"
                        + string.Format(contactedNotifyPageCountFormat, userExposureInformationList.Count());
                    ExposureDurationInMinutes = string.Format(AppResources.ContactedNotifyPageExposureDurationHeader, afterDate.ToString("D")) + "\n"
                        + string.Format(contactedNotifyPageExposureDurationFormat, dayCount, totalMinutes);
                }
                else if (exposureDurationInSec > 0)
                {
                    ExposureDurationInMinutes = string.Format(contactedNotifyPageExposureDurationFormat, dayCount, totalMinutes);
                }
                else if (userExposureInformationList.Count() > 0)
                {
                    ExposureCount = string.Format(contactedNotifyPageCountFormat, userExposureInformationList.Count());
                }
            }
            catch(Exception exception)
            {
                loggerService.Exception("failed to risk calculation", exception);
                await _dialogService.ShowUnknownErrorWaringAsync();
            }
            finally
            {
                loggerService.EndMethod();
            }
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
