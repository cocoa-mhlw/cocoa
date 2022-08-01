/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Chino;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ExposuresPageViewModel : ViewModelBase
    {
        private readonly IExposureDataRepository _exposureDataRepository;
        private readonly IExposureRiskCalculationService _exposureRiskCalculationService;
        private readonly ILoggerService _loggerService;

        private readonly ILocalPathService _localPathService;
        private readonly IExposureDataExportService _exposureDataExportService;

        private V1ExposureRiskCalculationConfiguration _exposureRiskCalculationConfiguration;

        public ObservableCollection<ExposureSummary> Exposures { get; set; }

        private string _utcDescription;
        public string UtcDescription
        {
            get { return _utcDescription; }
            set { SetProperty(ref _utcDescription, value); }
        }

        public ExposuresPageViewModel(
            INavigationService navigationService,
            IExposureDataRepository exposureDataRepository,
            IExposureRiskCalculationService exposureRiskCalculationService,
            ILocalPathService localPathService,
            IExposureDataExportService exposureDataExportService,
            ILoggerService loggerService
            ) : base(navigationService)
        {
            _exposureDataRepository = exposureDataRepository;
            _exposureRiskCalculationService = exposureRiskCalculationService;
            _localPathService = localPathService;
            _exposureDataExportService = exposureDataExportService;
            _loggerService = loggerService;

            Title = AppResources.MainExposures;
            Exposures = new ObservableCollection<ExposureSummary>();
            UtcDescription = string.Format(
                AppResources.ExposuresPageToUtcDescription,
                TimeZoneInfo.Local.StandardName
                );
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            _loggerService.StartMethod();

            try
            {
                _exposureRiskCalculationConfiguration =
                    parameters.GetValue<V1ExposureRiskCalculationConfiguration>(ExposuresPage.ExposureRiskCalculationConfigurationKey);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public async Task InitExposures()
        {
            var exposures = new ObservableCollection<ExposureSummary>();

            var dailySummaryList
                = await _exposureDataRepository.GetDailySummariesAsync(AppConstants.TermOfExposureRecordValidityInDays);
            var dailySummaryMap = dailySummaryList.ToDictionary(ds => ds.GetDateTime());

            var exposureWindowList
                = await _exposureDataRepository.GetExposureWindowsAsync(AppConstants.TermOfExposureRecordValidityInDays);

            var userExposureInformationList
                = _exposureDataRepository.GetExposureInformationList(AppConstants.TermOfExposureRecordValidityInDays);

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
                        _exposureRiskCalculationConfiguration
                        );
                    if (riskLevel < RiskLevel.High)
                    {
                        continue;
                    }

                    var ens = new ExposureSummary()
                    {
                        Timestamp = ew.Key,
                        ExposureDate = IExposureDataRepository.ConvertToTerm(dailySummary.GetDateTime()),
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
                    var ens = new ExposureSummary()
                    {
                        Timestamp = ei.Key,
                        ExposureDate = IExposureDataRepository.ConvertToTerm(ei.Key),
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

        public Command OnClickExportExposureData => new Command(async () =>
        {
            _loggerService.StartMethod();

            try
            {
                string exposureDataFilePath = _localPathService.ExposureDataPath;
                await _exposureDataExportService.ExportAsync(exposureDataFilePath);

                await Share.RequestAsync(new ShareFileRequest
                {
                    File = new ShareFile(exposureDataFilePath)
                });
            }
            catch (NotImplementedInReferenceAssemblyException exception)
            {
                _loggerService.Exception("NotImplementedInReferenceAssemblyException", exception);
            }
            finally
            {
                _loggerService.EndMethod();
            }

        });

        public override async void OnAppearing()
        {
            base.OnAppearing();

            await InitExposures();
        }

        public override async void OnResume()
        {
            base.OnResume();

            await InitExposures();
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
