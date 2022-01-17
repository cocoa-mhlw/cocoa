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

namespace Covid19Radar.ViewModels
{
    public class ExposureCheckPageViewModel : ViewModelBase
    {
        private readonly ILoggerService _loggerService;
        private readonly IExposureDataRepository _exposureDataRepository;

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
            IExposureDataRepository exposureDataRepository
            ) : base(navigationService)
        {
            _loggerService = loggerService;
            _exposureDataRepository = exposureDataRepository;

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

                    SetupExposureCheckScores(summaries);
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
                    == Threshold.OPERATION_NOP) {
                _loggerService.Info("_exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Op = NOP");
                return;
            }

            LowRiskContactPageHeaderTextSuffix
                = string.Format(
                    AppResources.LowRiskContactPageHeaderTextSuffix,
                    _exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Value,
                    OperatorToString(_exposureRiskCalculationConfiguration.DailySummary_DaySummary_ScoreSum.Op)
                );
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

        private void SetupExposureCheckScores(List<DailySummary> summaries)
        {
            summaries.Sort((a, b) => b.GetDateTime().CompareTo(a.GetDateTime()));

            foreach (var summary in summaries)
            {
                ExposureCheckScores.Add(
                        new ExposureCheckScoreModel()
                        {
                            DailySummaryScoreSum = summary.DaySummary.ScoreSum,
                            DateMillisSinceEpoch = summary.DateMillisSinceEpoch
                        });
            }
        }

        public Command OnClickShareApp => new Command(() =>
        {
            _loggerService.StartMethod();

            AppUtils.PopUpShare();

            _loggerService.EndMethod();
        });
    }
}