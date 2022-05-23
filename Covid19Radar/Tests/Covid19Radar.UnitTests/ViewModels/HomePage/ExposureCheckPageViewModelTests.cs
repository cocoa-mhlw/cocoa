// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using Covid19Radar.ViewModels;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Moq;
using Prism.Navigation;
using Xunit;
using Chino;
using System.Collections.Generic;
using System.Threading.Tasks;
using Covid19Radar.Common;
using System.Linq;
using Covid19Radar.Views;
using Covid19Radar.Model;
using Covid19Radar.Services;

using Threshold = Covid19Radar.Model.V1ExposureRiskCalculationConfiguration.Threshold;
using System;

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class ExposureCheckPageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IExposureDataRepository> mockExposureDataRepository;
        private readonly Mock<IExposureRiskCalculationService> mockExposureRiskCalculationService;
        private readonly Mock<ILocalPathService> mockLocalPathService;
        private readonly Mock<IExposureDataExportService> mockExposureDataExportService;
        private readonly Mock<IUserDataRepository> mockUserDataRepository;
        private readonly Mock<IExposureRiskCalculationConfigurationRepository> mockExposureRiskCalculationConfigurationRepository;
        private readonly Mock<IDateTimeUtility> mockDateTimeUtility;

        public ExposureCheckPageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockExposureDataRepository = mockRepository.Create<IExposureDataRepository>();
            mockExposureRiskCalculationService = mockRepository.Create<IExposureRiskCalculationService>();
            mockLocalPathService = mockRepository.Create<ILocalPathService>();
            mockExposureDataExportService = mockRepository.Create<IExposureDataExportService>();
            mockUserDataRepository = mockRepository.Create<IUserDataRepository>();
            mockExposureRiskCalculationConfigurationRepository = mockRepository.Create<IExposureRiskCalculationConfigurationRepository>();
            mockDateTimeUtility = mockRepository.Create<IDateTimeUtility>();
        }


        private ExposureCheckPageViewModel CreateViewModel()
        {

            return new ExposureCheckPageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                mockExposureDataRepository.Object,
                mockExposureRiskCalculationService.Object,
                mockLocalPathService.Object,
                mockExposureDataExportService.Object,
                mockUserDataRepository.Object,
                mockExposureRiskCalculationConfigurationRepository.Object,
                mockDateTimeUtility.Object
                );
        }

        [Fact(Skip = "Temporarily do not display contacts below the threshold")]
        public void LowRiskPage_Initialize_Display()
        {
            var dummyDailySummaries = new List<DailySummary>()
                {
                    new DailySummary()
                    {
                        DateMillisSinceEpoch = (new DateTime().AddDays(0).Date).ToUnixEpochMillis(),
                        DaySummary = new ExposureSummaryData()
                        {
                            ScoreSum = 1700
                        },
                    },
                    new DailySummary()
                    {
                        DateMillisSinceEpoch = (new DateTime().AddDays(1).Date).ToUnixEpochMillis(),
                        DaySummary = new ExposureSummaryData()
                        {
                            ScoreSum = 1700
                        },
                    },
                    new DailySummary()
                    {
                        DateMillisSinceEpoch = (new DateTime().AddDays(2).Date).ToUnixEpochMillis(),
                        DaySummary = new ExposureSummaryData()
                        {
                            ScoreSum = 1000
                        },
                    },
                };
            var dummyExposureWindows = new List<ExposureWindow>()
                {
                    new ExposureWindow()
                    {
                        DateMillisSinceEpoch = (new DateTime().AddDays(0).Date).ToUnixEpochMillis(),
                        ScanInstances = new List<ScanInstance>()
                        {
                            new ScanInstance()
                            {
                                SecondsSinceLastScan = 840,
                            }
                        },
                    },
                    new ExposureWindow()
                    {
                        DateMillisSinceEpoch = (new DateTime().AddDays(1).Date).ToUnixEpochMillis(),
                        ScanInstances = new List<ScanInstance>()
                        {
                            new ScanInstance()
                            {
                                SecondsSinceLastScan = 840,
                            }
                        },
                    },
                    new ExposureWindow()
                    {
                        DateMillisSinceEpoch = (new DateTime().AddDays(2).Date).ToUnixEpochMillis(),
                        ScanInstances = new List<ScanInstance>()
                        {
                            new ScanInstance()
                            {
                                SecondsSinceLastScan = 900,
                            }
                        },
                    },
                };

            mockUserDataRepository
                .Setup(x => x.GetDaysOfUse())
                .Returns(14);

            mockDateTimeUtility
                .Setup(x => x.UtcNow)
                .Returns((new DateTime()).AddDays(14));

            mockExposureDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.TermOfExposureRecordValidityInDays))
                .ReturnsAsync(dummyDailySummaries);

            mockExposureDataRepository
                .Setup(x => x.GetExposureWindowsAsync(AppConstants.TermOfExposureRecordValidityInDays))
                .ReturnsAsync(dummyExposureWindows);

            var riskConfiguration = new V1ExposureRiskCalculationConfiguration()
            {
                DailySummary_DaySummary_ScoreSum = new Threshold()
                {
                    Op = Threshold.OPERATION_GREATER_EQUAL,
                    Value = 1170,
                },
                ExposureWindow_ScanInstance_SecondsSinceLastScanSum = new Threshold()
                {
                    Op = Threshold.OPERATION_GREATER_EQUAL,
                    Value = 900,
                },
            };

            var exposureCheckPageViewModel = CreateViewModel();
            var parameters = ExposureCheckPage.BuildNavigationParams(riskConfiguration);
            exposureCheckPageViewModel.Initialize(parameters);

            Assert.True(exposureCheckPageViewModel.IsExposureDetected);

            Assert.Equal(14, exposureCheckPageViewModel.ExposureCheckScores.Count());

            var dates = Enumerable.Range(0, 14)
                .Select(offset => new DateTime().AddDays(offset).Date.ToUnixEpochMillis())
                .ToList();
            dates.Sort((a, b) => b.CompareTo(a));

            // Sort DESC
            for (int i=0; i < 14; i++)
            {
                Assert.Equal(dates[i], exposureCheckPageViewModel.ExposureCheckScores[i].DateMillisSinceEpoch);
            }

            Assert.Equal(dummyDailySummaries[2].DateMillisSinceEpoch, exposureCheckPageViewModel.ExposureCheckScores[11].DateMillisSinceEpoch);
            Assert.True(exposureCheckPageViewModel.ExposureCheckScores[11].IsScoreVisible);
            Assert.False(exposureCheckPageViewModel.ExposureCheckScores[11].IsDurationTimeVisible);
            Assert.True(exposureCheckPageViewModel.ExposureCheckScores[11].IsReceived);

            Assert.Equal(dummyDailySummaries[1].DateMillisSinceEpoch, exposureCheckPageViewModel.ExposureCheckScores[12].DateMillisSinceEpoch);
            Assert.True(exposureCheckPageViewModel.ExposureCheckScores[12].IsScoreVisible);
            Assert.True(exposureCheckPageViewModel.ExposureCheckScores[12].IsDurationTimeVisible);
            Assert.True(exposureCheckPageViewModel.ExposureCheckScores[12].IsReceived);

            Assert.Equal(dummyDailySummaries[0].DateMillisSinceEpoch, exposureCheckPageViewModel.ExposureCheckScores[13].DateMillisSinceEpoch);
            Assert.True(exposureCheckPageViewModel.ExposureCheckScores[13].IsScoreVisible);
            Assert.True(exposureCheckPageViewModel.ExposureCheckScores[13].IsDurationTimeVisible);
            Assert.True(exposureCheckPageViewModel.ExposureCheckScores[13].IsReceived);
        }

        [Fact]
        public async void NoRiskPage_Initialize_Display()
        {
            mockExposureDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(Task.FromResult(new List<DailySummary>()));

            var exposureCheckPageViewModel = CreateViewModel();
            var parameters = ExposureCheckPage.BuildNavigationParams(new V1ExposureRiskCalculationConfiguration());
            exposureCheckPageViewModel.Initialize(parameters);
            await exposureCheckPageViewModel.Setup();

            Assert.False(exposureCheckPageViewModel.IsExposureDetected);
        }
    }
}
