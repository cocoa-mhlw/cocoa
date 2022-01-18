// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Covid19Radar.Services.Logs;
using Covid19Radar.Repository;
using Covid19Radar.Model;
using Prism.Navigation;
using System.Linq;
using Chino;
using Covid19Radar.Common;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using Xamarin.Forms;
using Covid19Radar.Views;
using Covid19Radar.Resources;

using Threshold = Covid19Radar.Model.V1ExposureRiskCalculationConfiguration.Threshold;
using Covid19Radar.Services;
using System.Threading.Tasks;
using System.Globalization;

namespace Covid19Radar.ViewModels
{
    public class ExposureCheckPageViewModel : ViewModelBase
    {
        private readonly ILoggerService _loggerService;
        private readonly IExposureDataRepository _exposureDataRepository;
        private readonly IExposureRiskCalculationConfigurationRepository _exposureRiskCalculationConfigurationRepository;
        private readonly IExposureRiskCalculationService _exposureRiskCalculationService;

        public ObservableCollection<ExposureCheckScoreModel> ExposureCheckScores { get; set; }

        private bool _isVisibleNoRiskContact;
        public bool IsVisibleNoRiskContact
        {
            get { return _isVisibleNoRiskContact; }
            set { SetProperty(ref _isVisibleNoRiskContact, value); }
        }

        private bool _isVisibleLowRiskContact;
        public bool IsVisibleLowRiskContact
        {
            get { return _isVisibleLowRiskContact; }
            set { SetProperty(ref _isVisibleLowRiskContact, value); }
        }

        private string _lowRiskContactPageAnnotationDecription;
        public string LowRiskContactPageAnnotationDecription
        {
            get { return _lowRiskContactPageAnnotationDecription; }
            set { SetProperty(ref _lowRiskContactPageAnnotationDecription, value); }
        }

        private string _lowRiskContactPageHeaderTextSuffix;
        public string LowRiskContactPageHeaderTextSuffix
        {
            get { return _lowRiskContactPageHeaderTextSuffix; }
            set { SetProperty(ref _lowRiskContactPageHeaderTextSuffix, value); }
        }

        private V1ExposureRiskCalculationConfiguration _exposureRiskCalculationConfiguration;

        public ExposureCheckPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IExposureDataRepository exposureDataRepository,
            IExposureRiskCalculationConfigurationRepository exposureRiskCalculationConfigurationRepository,
            IExposureRiskCalculationService exposureRiskCalculationService

            ) : base(navigationService)
        {
            _loggerService = loggerService;
            _exposureDataRepository = exposureDataRepository;
            _exposureRiskCalculationConfigurationRepository = exposureRiskCalculationConfigurationRepository;
            _exposureRiskCalculationService = exposureRiskCalculationService;

            ExposureCheckScores = new ObservableCollection<ExposureCheckScoreModel>();
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            _loggerService.StartMethod();

            _exposureRiskCalculationConfiguration
                = parameters.GetValue<V1ExposureRiskCalculationConfiguration>(ExposureCheckPage.ExposureRiskCalculationConfigurationKey);

            ShowExposureRiskCalculationConfiguration();

            try
            {
                var summaries = await _exposureDataRepository
                    .GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay);
                if (0 < summaries.Count())
                {
                    IsVisibleLowRiskContact = true;
                    IsVisibleNoRiskContact = false;

                    _ = SetupExposureCheckScoresAsync(summaries);
                }
                else
                {
                    IsVisibleLowRiskContact = false;
                    IsVisibleNoRiskContact = true;
                }
            }
            catch (Exception exception)
            {
                _loggerService.Exception("Exception occurred", exception);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        private void ShowExposureRiskCalculationConfiguration()
        {
            if (_exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Op
                    == Threshold.OPERATION_NOP)
            {
                _loggerService.Info("_exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Op = NOP");
                return;
            }

            var exposureDurationThresholdTimeSpan = TimeSpan.FromSeconds(_exposureRiskCalculationConfiguration.ExposureWindow_ScanInstance_SecondsSinceLastScanSum.Value);
            var exposureDurationThresholdInMinute = Math.Ceiling(exposureDurationThresholdTimeSpan.TotalMinutes);

            LowRiskContactPageHeaderTextSuffix = "";
                //= string.Format(
                //    AppResources.LowRiskContactPageHeaderTextSuffix,
                //    _exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Value,
                //    _exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Value,
                //    OperatorToString(_exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Op),
                //    exposureDurationThresholdInMinute,
                //    OperatorToString(_exposureRiskCalculationConfiguration.ExposureWindow_ScanInstance_SecondsSinceLastScanSum.Op)
                //);

            LowRiskContactPageAnnotationDecription
                = string.Format(
                    AppResources.LowRiskContactPageAnnotationDecription,
                    _exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Value,
                    OperatorToString(_exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Op)
                );
        }

        private static string OperatorToString(string op)
        {
            return op switch
            {
                Threshold.OPERATION_GREATER_EQUAL => AppResources.ThresholdTextOperatorGte,
                Threshold.OPERATION_GREATER => AppResources.ThresholdTextOperatorGt,
                Threshold.OPERATION_EQUAL => AppResources.ThresholdTextOperatorEqual,
                Threshold.OPERATION_LESS => AppResources.ThresholdTextOperatorLt,
                Threshold.OPERATION_LESS_EQUAL => AppResources.ThresholdTextOperatorLte,
                Threshold.OPERATION_NOP => string.Empty,
                _ => string.Empty
            };
        }

        private async Task SetupExposureCheckScoresAsync(List<DailySummary> summaries)
        {
            var dailySummaryList
                = await _exposureDataRepository.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay);

            if (dailySummaryList.Count() == 0)
            {
                return;
            }

            _loggerService.Info(_exposureRiskCalculationConfiguration.ToString());

            var dailySummaryMap = dailySummaryList.ToDictionary(ds => ds.GetDateTime());

            var exposureWindowList
                = await _exposureDataRepository.GetExposureWindowsAsync(AppConstants.DaysOfExposureInformationToDisplay);

            var userExposureInformationList
                = _exposureDataRepository.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay);

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
                if (riskLevel > RiskLevel.Low)
                {
                    continue;
                }

                List<string> reason = CreateReason(dailySummary, ew.ToList());

                ExposureCheckScores.Add(
                        new ExposureCheckScoreModel()
                        {
                            DateTimeString = DateTimeOffset.UnixEpoch
                                    .AddMilliseconds(dailySummary.DateMillisSinceEpoch).UtcDateTime
                                    .ToLocalTime().ToString("D", CultureInfo.CurrentCulture),
                            Reason = string.Join("\n", reason),
                        });
            }
        }

        private List<string> CreateReason(DailySummary dailySummary, List<ExposureWindow> exposureWindowList)
        {
            var descriptionList = new List<string>();

            if (!_exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Cond(dailySummary.DaySummary.ScoreSum))
            {
                var description =  string.Format(
                    AppResources.LowRiskContactPage_DailySummary_ScoreSum_Descritpion_Unsatisfied,
                    dailySummary.DaySummary.ScoreSum,
                    _exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Value,
                    OperatorToString(_exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Op)
                    );
                descriptionList.Add(description);
            }
            else
            {
                var description = string.Format(
                    AppResources.LowRiskContactPage_DailySummary_ScoreSum_Descritpion_Satisfied,
                    dailySummary.DaySummary.ScoreSum,
                    _exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Value,
                    OperatorToString(_exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Op)
                    );
                descriptionList.Add(description);
            }

            var exposureDurationInSec = exposureWindowList.Sum(e => e.ScanInstances.Sum(s => s.SecondsSinceLastScan));

            if (!_exposureRiskCalculationConfiguration.ExposureWindow_ScanInstance_SecondsSinceLastScanSum.Cond(exposureDurationInSec))
            {
                var exposureDurationTimeSpan = TimeSpan.FromSeconds(exposureDurationInSec);
                var exposureDurationInMinute = Math.Ceiling(exposureDurationTimeSpan.TotalMinutes);

                var exposureDurationThresholdTimeSpan = TimeSpan.FromSeconds(_exposureRiskCalculationConfiguration.ExposureWindow_ScanInstance_SecondsSinceLastScanSum.Value);
                var exposureDurationThresholdInMinute = Math.Ceiling(exposureDurationThresholdTimeSpan.TotalMinutes);

                var description = string.Format(
                    AppResources.LowRiskContactPage_ExposureDuration_Description_Unsatisfied,
                    exposureDurationInMinute,
                    exposureDurationThresholdInMinute,
                    OperatorToString(_exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Op)
                    );
                descriptionList.Add(description);
            }

            return descriptionList;
        }

        public Command OnClickShareApp => new Command(() =>
        {
            _loggerService.StartMethod();

            AppUtils.PopUpShare();

            _loggerService.EndMethod();
        });
    }

    public class ExposureCheckScoreModel
    {
        public string DateTimeString { get; set; }
        public string Reason { get; set; }
    }
}