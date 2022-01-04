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

namespace Covid19Radar.ViewModels
{
    public class ExposureCheckPageViewModel : ViewModelBase
    {
        private const int EXPOSURE_NOT_FOUND_VALUE_IN_MINUTES = 0;

        private readonly ILoggerService _loggerService;
        private readonly IUserDataRepository _userDataRepository;
        private readonly IExposureRiskCalculationService _exposureRiskCalculationService;

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
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            _loggerService.StartMethod();


            if (await _exposureRiskCalculationService.HasLowRiskContact())
            {
                IsVisibleLowRiskContact = true;
                IsVisibleNoRiskContact = false;

                SetupTotalContactTime();
            }
            else
            {
                IsVisibleLowRiskContact = false;
                IsVisibleNoRiskContact = true;

                TotalContactTime = $"{EXPOSURE_NOT_FOUND_VALUE_IN_MINUTES}{AppResources.LowRiskContactPageCountSuffixMinutesText}";
            }

            _loggerService.EndMethod();
        }

        private async void SetupTotalContactTime()
        {
            try
            {
                var exposureSeconds = await GetTotalNumberOfExposureSeconds();
                TotalContactTime = MakeTotalContactTimeString(exposureSeconds);
            }
            catch (Exception exception)
            {
                TotalContactTime = $"{EXPOSURE_NOT_FOUND_VALUE_IN_MINUTES}{AppResources.LowRiskContactPageCountSuffixMinutesText}";
                _loggerService.Exception("failed to get TotalContactTime", exception);
            }
        }

        private string MakeTotalContactTimeString(int exposureSeconds)
        {
            _loggerService.StartMethod();

            if (exposureSeconds < 0)
            {
                throw new InvalidDataException();
            }

            var totalNumberOfExposureMinutes = exposureSeconds / 60;
            var exposureHours = totalNumberOfExposureMinutes / 60;
            var exposureMinutes = totalNumberOfExposureMinutes % 60;

            var sb = new System.Text.StringBuilder();
            if (exposureHours == 0 && exposureMinutes == 0)
            {
                sb.Append($"{EXPOSURE_NOT_FOUND_VALUE_IN_MINUTES}{AppResources.LowRiskContactPageCountSuffixMinutesText}");
            }
            else
            {
                if (0 < exposureHours)
                {
                    sb.Append($"{exposureHours}{AppResources.LowRiskContactPageCountSuffixHoursText}");
                }
                if (0 < exposureMinutes)
                {
                    sb.Append($"{exposureMinutes}{AppResources.LowRiskContactPageCountSuffixMinutesText}");
                }
            }

            return sb.ToString();
        }

        private async Task<int> GetTotalNumberOfExposureSeconds()
        {
            var windows = await _userDataRepository.GetExposureWindowsAsync();
            var numberOfExposureSecondsList = windows
                .Select(AggregateSecondsSinceLastScans);
            var totalNumberOfExposureSeconds = numberOfExposureSecondsList
                .Aggregate(0, (sum, x) => sum + x);
            return totalNumberOfExposureSeconds;
        }

        private int AggregateSecondsSinceLastScans(ExposureWindow window) =>
            window.ScanInstances.Select(x => x.SecondsSinceLastScan).Aggregate(0, (sum, x) => sum + x);

        private string _totalContactTime;
        public string TotalContactTime
        {
            get => _totalContactTime;
            set => SetProperty(ref _totalContactTime, value);
        }

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
    }
}