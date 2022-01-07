// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Covid19Radar.Services.Logs;
using Covid19Radar.Resources;
using Covid19Radar.Repository;
using Prism.Navigation;
using System.Linq;
using System.Threading.Tasks;
using System;
using Chino;
using System.IO;
using Covid19Radar.Services;
using Xamarin.Forms;
using Covid19Radar.Common;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Covid19Radar.ViewModels
{
    public class ExposureCheckPageViewModel : ViewModelBase
    {
        private const int EXPOSURE_NOT_FOUND_VALUE_IN_MINUTES = 0;

        private readonly ILoggerService _loggerService;
        private readonly IUserDataRepository _userDataRepository;
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

        public ExposureCheckPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            IExposureRiskCalculationService exposureRiskCalculationService) : base(navigationService)
        {
            Title = AppResources.TitileUserStatusSettings;
            this._loggerService = loggerService;
            this._userDataRepository = userDataRepository;
            this._exposureRiskCalculationService = exposureRiskCalculationService;

            ExposureCheckScores = new ObservableCollection<ExposureCheckScoreModel>();
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            _loggerService.StartMethod();

            if (await _exposureRiskCalculationService.HasContact())
            {
                IsVisibleLowRiskContact = true;
                IsVisibleNoRiskContact = false;

                SetupExposureCheckScores();
            }
            else
            {
                IsVisibleLowRiskContact = false;
                IsVisibleNoRiskContact = true;
            }

            _loggerService.EndMethod();
        }

        private async void SetupExposureCheckScores()
        {
            var summaries = await _userDataRepository.GetDailySummariesAsync();

            foreach (var summary in summaries)
            {
                ExposureCheckScores.Add(
                        new ExposureCheckScoreModel()
                        {
                            Sum = summary.DaySummary.ScoreSum.ToString("0.00"),
                            Date = summary.GetDateTime().ToLocalTime().ToString("D", CultureInfo.CurrentCulture)
                        });
            }
        }
    }

    public class ExposureCheckScoreModel
    {

        public string Sum { get; set; }
        public string Date { get; set; }
    }
}