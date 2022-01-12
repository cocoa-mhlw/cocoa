/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Chino;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19Radar.ViewModels
{
    public class ExposuresPageViewModel : ViewModelBase
    {
        private readonly IExposureDataRepository _exposureDataRepository;
        private readonly IExposureRiskCalculationService _exposureRiskCalculationService;

        public ObservableCollection<ExposureSummary> _exposures;

        public ObservableCollection<ExposureSummary> Exposures
        {
            get { return _exposures; }
            set { SetProperty(ref _exposures, value); }
        }

        public ExposuresPageViewModel(
            INavigationService navigationService,
            IExposureDataRepository exposureDataRepository,
            IExposureRiskCalculationService exposureRiskCalculationService
            ) : base(navigationService)
        {
            _exposureDataRepository = exposureDataRepository;
            _exposureRiskCalculationService = exposureRiskCalculationService;

            Title = AppResources.MainExposures;
            _exposures = new ObservableCollection<ExposureSummary>();

        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            await InitExposures();
        }

        public async Task InitExposures()
        {
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
                    var dailySummary = dailySummaryMap[ew.Key];

                    RiskLevel riskLevel = _exposureRiskCalculationService.CalcRiskLevel(dailySummary, ew.ToList());
                    if (riskLevel < RiskLevel.High)
                    {
                        continue;
                    }

                    var ens = new ExposureSummary()
                    {
                        Timestamp = ew.Key,
                        ExposureDate = dailySummary.GetDateTime().ToLocalTime().ToString("D", CultureInfo.CurrentCulture),
                    };
                    var exposureDurationInSec = ew.Sum(e => e.ScanInstances.Sum(s => s.SecondsSinceLastScan));
                    ens.SetExposureTime(exposureDurationInSec);

                    _exposures.Add(ens);
                }
            }

            if (userExposureInformationList.Count() > 0)
            {
                foreach (var ei in userExposureInformationList.GroupBy(userExposureInformation => userExposureInformation.Timestamp))
                {
                    var ens = new ExposureSummary()
                    {
                        Timestamp = ei.Key,
                        ExposureDate = ei.Key.ToLocalTime().ToString("D", CultureInfo.CurrentCulture),
                    };
                    ens.SetExposureCount(ei.Count());
                    _exposures.Add(ens);
                }
            }

            Exposures = new ObservableCollection<ExposureSummary>(
                _exposures.OrderByDescending(exposureSummary => exposureSummary.Timestamp)
                );
        }
    }

    public class ExposureSummary
    {
        public DateTime Timestamp { get; set; }

        public string ExposureDate { get; set; }

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
