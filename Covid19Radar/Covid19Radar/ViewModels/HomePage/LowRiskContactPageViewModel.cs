// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Covid19Radar.Services.Logs;
using Covid19Radar.Resources;
using Covid19Radar.Repository;
using Prism.Navigation;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19Radar.ViewModels
{
    public class LowRiskContactPageViewModel: ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly IUserDataRepository userDataRepository;

        public LowRiskContactPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository) : base(navigationService)
        {
            Title = AppResources.TitileUserStatusSettings;
            this.loggerService = loggerService;
            this.userDataRepository = userDataRepository;
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            loggerService.StartMethod();

            var exposureSeconds = await getTotalNumberOfExposureSeconds();
            TotalContactTime = makeTotalContactTimeString(exposureSeconds);

            loggerService.EndMethod();
        }

        private string makeTotalContactTimeString(int exposureSeconds)
        {
            var totalNumberOfExposureMinutes = exposureSeconds / 60;
            var exposureHours = totalNumberOfExposureMinutes / 60;
            var exposureMinutes = totalNumberOfExposureMinutes % 60;

            var sb = new System.Text.StringBuilder();
            if (0 < exposureHours)
            {
                sb.Append($"{exposureHours}{AppResources.LowRiskContactPageCountSuffixHourText}");
            }
            sb.Append($"{exposureMinutes}{AppResources.LowRiskContactPageCountSuffixMinutesText}");
            return sb.ToString();
        }

        private async Task<int> getTotalNumberOfExposureSeconds()
        {
            var windows = await userDataRepository.GetExposureWindowsAsync();
            var numberOfExposureSecondsList = windows
                .ToArray()
                .Select(aggregateSecondsSinceLastScans);
            var totalNumberOfExposureSeconds = numberOfExposureSecondsList
                .Aggregate(0, (sum, x) => sum + x);
            return totalNumberOfExposureSeconds;
        }

        private int aggregateSecondsSinceLastScans(Chino.ExposureWindow window) =>
            window.ScanInstances.Select(x => x.SecondsSinceLastScan).Aggregate(0, (sum, x) => sum + x);

        private string _totalContactTime;
        public string TotalContactTime
        {
            get => _totalContactTime;
            set => SetProperty(ref _totalContactTime, value);
        }
    }
}
