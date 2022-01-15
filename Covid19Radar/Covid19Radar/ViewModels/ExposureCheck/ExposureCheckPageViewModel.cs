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

namespace Covid19Radar.ViewModels
{
    public class ExposureCheckPageViewModel : ViewModelBase
    {
        private readonly ILoggerService _loggerService;
        private readonly IUserDataRepository _userDataRepository;
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

        public ExposureCheckPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            IExposureDataRepository exposureDataRepository
            ) : base(navigationService)
        {
            this._loggerService = loggerService;
            this._userDataRepository = userDataRepository;
            _exposureDataRepository = exposureDataRepository;

            ExposureCheckScores = new ObservableCollection<ExposureCheckScoreModel>();
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            _loggerService.StartMethod();

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