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
        private readonly IUserDataRepository _userDataRepository;
        private readonly IDateTimeUtility _dateTimeUtility;

        public ObservableCollection<ExposureCheckScoreModel> ExposureCheckScores { get; set; }

        private bool _isExposureDetected = false;
        public bool IsExposureDetected
        {
            get { return _isExposureDetected; }
            set { SetProperty(ref _isExposureDetected, value); }
        }

        private string _lowRiskContactPageHeaderTextSuffix;
        public string LowRiskContactPageHeaderTextSuffix
        {
            get { return _lowRiskContactPageHeaderTextSuffix; }
            set { SetProperty(ref _lowRiskContactPageHeaderTextSuffix, value); }
        }

        private string _lowRiskContactPageAnnotationDecription;
        public string LowRiskContactPageAnnotationDecription
        {
            get { return _lowRiskContactPageAnnotationDecription; }
            set { SetProperty(ref _lowRiskContactPageAnnotationDecription, value); }
        }

        private V1ExposureRiskCalculationConfiguration _exposureRiskCalculationConfiguration;

        public ExposureCheckPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IExposureDataRepository exposureDataRepository,
            IExposureRiskCalculationService exposureRiskCalculationService,
            IUserDataRepository userDataRepository,
            IExposureRiskCalculationConfigurationRepository exposureRiskCalculationConfigurationRepository,
            IDateTimeUtility dateTimeUtility
            ) : base(navigationService)
        {
            _loggerService = loggerService;
            _exposureDataRepository = exposureDataRepository;
            _exposureRiskCalculationService = exposureRiskCalculationService;
            _userDataRepository = userDataRepository;
            _exposureRiskCalculationConfigurationRepository = exposureRiskCalculationConfigurationRepository;
            _dateTimeUtility = dateTimeUtility;

            ExposureCheckScores = new ObservableCollection<ExposureCheckScoreModel>();
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            _loggerService.StartMethod();

            _exposureRiskCalculationConfiguration
                = parameters.GetValue<V1ExposureRiskCalculationConfiguration>(ExposureCheckPage.ExposureRiskCalculationConfigurationKey);

            if (_exposureRiskCalculationConfiguration is null)
            {
                _exposureRiskCalculationConfiguration
                    = await _exposureRiskCalculationConfigurationRepository.GetExposureRiskCalculationConfigurationAsync(preferCache: true);
            }

            _loggerService.Info(_exposureRiskCalculationConfiguration.ToString());

            ShowExposureRiskCalculationConfiguration();

            try
            {
                _ = Setup();
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

        private int GetLastOffsetDay(List<DailySummary> dailySummaryList)
        {
            var dayOfUse = _userDataRepository.GetDaysOfUse();

            var oldestExposureDateMillisSinceEpoch = dailySummaryList.Min(x => x.DateMillisSinceEpoch);
            var oldestExposureDate = DateTimeOffset.UnixEpoch.AddMilliseconds(oldestExposureDateMillisSinceEpoch).UtcDateTime;
            var oldestExposureDateOffset = (DateTime.UtcNow.Date - oldestExposureDate).Days;

            var daysOffset = Math.Max(
                dayOfUse,
                oldestExposureDateOffset
                );

            daysOffset = Math.Min(
                daysOffset,
                14 // 2 weeks
                );

            _loggerService.Debug($"daysOffset: {daysOffset}");

            return daysOffset;
        }

        public async Task Setup()
        {
            _loggerService.StartMethod();

            List<DailySummary> dailySummaryList = await _exposureDataRepository
                .GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay);

            if (dailySummaryList.Count() == 0)
            {
                _loggerService.EndMethod();
                return;
            }

            var dailySummaryMap = dailySummaryList.ToDictionary(ds => ds.DateMillisSinceEpoch);

            _loggerService.Debug($"dailySummaryMap {dailySummaryMap.Count}");

            var exposureWindowList
                = await _exposureDataRepository.GetExposureWindowsAsync(AppConstants.DaysOfExposureInformationToDisplay);

            _loggerService.Debug($"exposureWindowList {exposureWindowList.Count}");

            var daysOffset = GetLastOffsetDay(dailySummaryList);

            var dates = Enumerable.Range(-daysOffset, daysOffset)
                .Select(offset => _dateTimeUtility.UtcNow.Date.AddDays(offset).ToUnixEpochMillis())
                .ToList();
            dates.Sort((a, b) => b.CompareTo(a));

            foreach (var dateMillisSinceEpoch in dates)
            {
                _loggerService.Debug($"dateMillisSinceEpoch {dateMillisSinceEpoch}");

                var ewList = exposureWindowList.Where(ew => ew.DateMillisSinceEpoch == dateMillisSinceEpoch).ToList();

                bool IsBlank = false;

                if (!dailySummaryMap.ContainsKey(dateMillisSinceEpoch))
                {
                    _loggerService.Warning($"DailySummary: {dateMillisSinceEpoch} not found.");
                    IsBlank = true;
                }
                else if (ewList.Count == 0)
                {
                    _loggerService.Warning($"DailySummary: {dateMillisSinceEpoch} found, but that is not contained the list of ExposureWindow.");
                    IsBlank = true;
                }

                if (IsBlank)
                {
                    ExposureCheckScoreModel blank = CreateBlankModel(dateMillisSinceEpoch);
                    ExposureCheckScores.Add(blank);
                    continue;
                }

                IsExposureDetected = true;

                var dailySummary = dailySummaryMap[dateMillisSinceEpoch];

                RiskLevel riskLevel = _exposureRiskCalculationService.CalcRiskLevel(
                    dailySummary,
                    ewList,
                    _exposureRiskCalculationConfiguration
                    );
                if (riskLevel > RiskLevel.Low)
                {
                    continue;
                }

                ExposureCheckScoreModel exposureCheckModel = CreateExposureCheckScoreModel(dailySummary, ewList);
                ExposureCheckScores.Add(exposureCheckModel);
            }

            _loggerService.EndMethod();
        }

        private ExposureCheckScoreModel CreateBlankModel(long dateMillisSinceEpoch)
        {
            return new ExposureCheckScoreModel()
            {
                DateMillisSinceEpoch = dateMillisSinceEpoch,
                IsDurationTimeVisible = false,
                IsScoreVisible = false,
                IsReceived = false,
                Description = "受信した信号はありません"
            };
        }

        private ExposureCheckScoreModel CreateExposureCheckScoreModel(DailySummary dailySummary, List<ExposureWindow> exposureWindowList)
        {
            var exposureCheckModel = new ExposureCheckScoreModel()
            {
                DateMillisSinceEpoch = dailySummary.DateMillisSinceEpoch,
                IsReceived = true,
            };

            var descriptionList = new List<string>();

            var scoreSumValue = _exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Value;
            if (scoreSumValue <= 0)
            {
                scoreSumValue = ExposureRiskCalculationConfigurationRepository.CreateDefaultConfiguration().DailySummary_DaySummary_ScoreSum.Value;
                _loggerService.Error($"ExposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Value is invalid 0. Use default value {scoreSumValue}.");
            }

            var ratio = dailySummary.DaySummary.ScoreSum / scoreSumValue;

            if (1 > ratio)
            {
                var description = string.Format(
                    AppResources.LowRiskContactPage_DailySummary_ScoreSum_Descritpion_Satisfied,
                    Math.Ceiling(ratio * 100)
                    );
                descriptionList.Add(description);
            }
            else
            {
                exposureCheckModel.IsDurationTimeVisible = true;

                var description = string.Format(
                    AppResources.LowRiskContactPage_DailySummary_ScoreSum_Descritpion_Unsatisfied,
                    Math.Ceiling(ratio * 100)
                    );
                descriptionList.Add(description);
            }

            var exposureDurationInSec = exposureWindowList.Sum(e => e.ScanInstances.Sum(s => s.SecondsSinceLastScan));

            if (exposureCheckModel.IsDurationTimeVisible
                && !_exposureRiskCalculationConfiguration.ExposureWindow_ScanInstance_SecondsSinceLastScanSum.Cond(exposureDurationInSec))
            {
                var exposureDurationTimeSpan = TimeSpan.FromSeconds(exposureDurationInSec);
                var exposureDurationInMinute = Math.Ceiling(exposureDurationTimeSpan.TotalMinutes);

                var exposureDurationThresholdTimeSpan = TimeSpan.FromSeconds(_exposureRiskCalculationConfiguration.ExposureWindow_ScanInstance_SecondsSinceLastScanSum.Value);
                var exposureDurationThresholdInMinute = Math.Ceiling(exposureDurationThresholdTimeSpan.TotalMinutes);

                var description = string.Format(
                    AppResources.LowRiskContactPage_ExposureDuration_Description_Unsatisfied,
                    exposureDurationInMinute
                    );
                descriptionList.Add(description);
            }

            // Score is always visible
            exposureCheckModel.IsScoreVisible = true;
            exposureCheckModel.Description = string.Join("\n", descriptionList);

            return exposureCheckModel;
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
        public long DateMillisSinceEpoch { get; set; }

        public string DateTimeString => DateTimeOffset.UnixEpoch
                .AddMilliseconds(DateMillisSinceEpoch).UtcDateTime
                .ToLocalTime().ToString("D", CultureInfo.CurrentCulture);

        public bool IsScoreVisible { get; set; }

        public bool IsDurationTimeVisible { get; set; }

        public bool IsReceived { get; set; }

        public string Description { get; set; }
    }
}