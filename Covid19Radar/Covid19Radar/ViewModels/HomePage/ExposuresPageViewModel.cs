/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Chino;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19Radar.ViewModels
{
    public class ExposuresPageViewModel : ViewModelBase
    {
        private readonly IExposureDataRepository _exposureDataRepository;
        private readonly IExposureRiskCalculationConfigurationRepository _exposureRiskCalculationConfigurationRepository;
        private readonly IExposureRiskCalculationService _exposureRiskCalculationService;
        private readonly ILoggerService _loggerService;

        public ObservableCollection<ExposureSummary> Exposures { get; set; }

        public ExposuresPageViewModel(
            INavigationService navigationService,
            IExposureDataRepository exposureDataRepository,
            IExposureRiskCalculationConfigurationRepository exposureRiskCalculationConfigurationRepository,
            IExposureRiskCalculationService exposureRiskCalculationService,
            ILoggerService loggerService
            ) : base(navigationService)
        {
            _exposureDataRepository = exposureDataRepository;
            _exposureRiskCalculationConfigurationRepository = exposureRiskCalculationConfigurationRepository;
            _exposureRiskCalculationService = exposureRiskCalculationService;
            _loggerService = loggerService;

            Title = AppResources.MainExposures;
            Exposures = new ObservableCollection<ExposureSummary>();
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            await InitExposures();
        }

        public async Task InitExposures()
        {
            var exposures = new ObservableCollection<ExposureSummary>();

            var exposureRiskCalculationConfiguration
                = await _exposureRiskCalculationConfigurationRepository.GetExposureRiskCalculationConfigurationAsync(preferCache: false);
            _loggerService.Info(exposureRiskCalculationConfiguration.ToString());

            var dailySummaryList
                = await _exposureDataRepository.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay);
            var dailySummaryMap = dailySummaryList.ToDictionary(ds => ds.GetDateTime());

            var exposureWindowList
                = await _exposureDataRepository.GetExposureWindowsAsync(AppConstants.DaysOfExposureInformationToDisplay);

            var userExposureInformationList
                = _exposureDataRepository.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay);

            if (dailySummaryList.Count() > 0)
            {
                foreach (var ew in exposureWindowList.GroupBy(exposureWindow => exposureWindow.GetDateTime()))
                {
                    if (!dailySummaryMap.ContainsKey(ew.Key))
                    {
                        _loggerService.Warning($"ExposureWindow: {ew.Key} found, but that is not contained the list of dailySummary.");
                        continue;
                    }

                    var dailySummary = dailySummaryMap[ew.Key];

                    RiskLevel riskLevel = _exposureRiskCalculationService.CalcRiskLevel(
                        dailySummary,
                        ew.ToList(),
                        exposureRiskCalculationConfiguration
                        );
                    if (riskLevel < RiskLevel.High)
                    {
                        continue;
                    }

                    var (start, end) = ConvertToTerm(dailySummary.GetDateTime());

                    var ens = new ExposureSummary()
                    {
                        Timestamp = ew.Key,
                        ExposureDateStart = start,
                        ExposureDateEnd = end,
                    };
                    var exposureDurationInSec = ew.Sum(e => e.ScanInstances.Sum(s => s.SecondsSinceLastScan));
                    ens.SetExposureTime(exposureDurationInSec);

                    exposures.Add(ens);
                }
            }

            if (userExposureInformationList.Count() > 0)
            {
                foreach (var ei in userExposureInformationList.GroupBy(userExposureInformation => userExposureInformation.Timestamp))
                {
                    var (start, end) = ConvertToTerm(ei.Key);

                    var ens = new ExposureSummary()
                    {
                        Timestamp = ei.Key,
                        ExposureDateStart = start,
                        ExposureDateEnd = end,
                    };
                    ens.SetExposureCount(ei.Count());
                    exposures.Add(ens);
                }
            }

            Exposures.Clear();
            foreach (var exposure in exposures.OrderByDescending(exposureSummary => exposureSummary.Timestamp))
            {
                Exposures.Add(exposure);
            }
        }

        private static (string, string) ConvertToTerm(DateTime utcDatetime)
        {
            var from = utcDatetime.Date.ToLocalTime();
            var to = from.AddDays(1).ToLocalTime();

            bool changeMonth = from.Month != to.Month;
            bool changeYear = from.Year != to.Year;

            string format = AppResources.ExposureDateFormatDate;
            if (changeMonth)
            {
                format = AppResources.ExposureDateFormatMonth;
            }
            if (changeYear)
            {
                format = AppResources.ExposureDateFormatYear;
            }

            string fromStr = string.Format(AppResources.ExposureDateFormatYear, from.Year, from.Month, from.Day, from.Hour);
            string toStr = string.Format(format, to.Year, to.Month, to.Day, to.Hour);

            return (fromStr, toStr);
        }

    }

    public class ExposureSummary
    {
        public DateTime Timestamp { get; set; }

        public string ExposureDateStart { get; set; }
        public string ExposureDateEnd { get; set; }

        private string _description;

        public string Description => _description;

        public void SetExposureCount(int value)
        {
            _description = PluralizeCount(value);
        }

        public void SetExposureTime(int exposureDurationInSec)
        {
            var timeSpan = TimeSpan.FromSeconds(exposureDurationInSec);
            var totalMinutes = Math.Ceiling(timeSpan.TotalMinutes);
            _description = string.Format(AppResources.ExposurePageExposureDuration, totalMinutes);
        }

        private static string PluralizeCount(int count)
        {
            return count switch
            {
                1 => AppResources.ExposuresPageExposureUnitPluralOnce,
                _ => string.Format(AppResources.ExposuresPageExposureUnitPlural, count),
            };
        }
    }
}
