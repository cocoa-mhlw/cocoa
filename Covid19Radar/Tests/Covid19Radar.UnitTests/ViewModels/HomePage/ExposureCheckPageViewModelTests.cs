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

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class ExposureCheckPageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IExposureDataRepository> mockExposureDataRepository;
        private readonly Mock<IExposureRiskCalculationService> mockExposureRiskCalculationService;

        public ExposureCheckPageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockExposureDataRepository = mockRepository.Create<IExposureDataRepository>();
            mockExposureRiskCalculationService = mockRepository.Create<IExposureRiskCalculationService>();
        }


        private ExposureCheckPageViewModel CreateViewModel()
        {
            return new ExposureCheckPageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                mockExposureDataRepository.Object,
                mockExposureRiskCalculationService.Object
                );
        }

        [Fact]
        public void LowRiskPage_Initialize_Display()
        {
            mockExposureDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay))
                .ReturnsAsync(new List<DailySummary>()
                {
                    new DailySummary()
                    {
                        DateMillisSinceEpoch = (1000 * 60 * 60 * 24),
                        DaySummary = new ExposureSummaryData()
                        {
                            ScoreSum = 1700
                        },
                    },
                    new DailySummary()
                    {
                        DateMillisSinceEpoch = (1000 * 60 * 60 * 24) * 2,
                        DaySummary = new ExposureSummaryData()
                        {
                            ScoreSum = 1700
                        },
                    },
                    new DailySummary()
                    {
                        DateMillisSinceEpoch = (1000 * 60 * 60 * 24) * 3,
                        DaySummary = new ExposureSummaryData()
                        {
                            ScoreSum = 1000
                        },
                    },
                });

            mockExposureDataRepository
                .Setup(x => x.GetExposureWindowsAsync(AppConstants.DaysOfExposureInformationToDisplay))
                .ReturnsAsync(new List<ExposureWindow>()
                {
                    new ExposureWindow()
                    {
                        DateMillisSinceEpoch = (1000 * 60 * 60 * 24),
                        ScanInstances = new List<ScanInstance>()
                        {
                            new ScanInstance()
                            {
                                SecondsSinceLastScan = 900,
                            }
                        },
                    },
                    new ExposureWindow()
                    {
                        DateMillisSinceEpoch = (1000 * 60 * 60 * 24) * 2,
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
                        DateMillisSinceEpoch = (1000 * 60 * 60 * 24) * 3,
                        ScanInstances = new List<ScanInstance>()
                        {
                            new ScanInstance()
                            {
                                SecondsSinceLastScan = 840,
                            }
                        },
                    },
                });

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

            Assert.True(exposureCheckPageViewModel.IsVisibleLowRiskContact);
            Assert.False(exposureCheckPageViewModel.IsVisibleNoRiskContact);

            Assert.Equal(3, exposureCheckPageViewModel.ExposureCheckScores.Count());

            // Sort DESC
            Assert.Equal((1000 * 60 * 60 * 24) * 3, exposureCheckPageViewModel.ExposureCheckScores[0].DateMillisSinceEpoch);
            Assert.Equal((1000 * 60 * 60 * 24) * 2, exposureCheckPageViewModel.ExposureCheckScores[1].DateMillisSinceEpoch);
            Assert.Equal(1000 * 60 * 60 * 24, exposureCheckPageViewModel.ExposureCheckScores[2].DateMillisSinceEpoch);

            Assert.True(exposureCheckPageViewModel.ExposureCheckScores[0].IsScoreVisible);
            Assert.False(exposureCheckPageViewModel.ExposureCheckScores[0].IsDurationTimeVisible);

            Assert.True(exposureCheckPageViewModel.ExposureCheckScores[1].IsScoreVisible);
            Assert.True(exposureCheckPageViewModel.ExposureCheckScores[1].IsDurationTimeVisible);

            Assert.True(exposureCheckPageViewModel.ExposureCheckScores[2].IsScoreVisible);
            Assert.False(exposureCheckPageViewModel.ExposureCheckScores[2].IsDurationTimeVisible);
        }

        [Fact]
        public void NoRiskPage_Initialize_Display()
        {
            mockExposureDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(Task.FromResult(new List<DailySummary>()));

            var exposureCheckPageViewModel = CreateViewModel();
            var parameters = ExposureCheckPage.BuildNavigationParams(new V1ExposureRiskCalculationConfiguration());
            exposureCheckPageViewModel.Initialize(parameters);
            Assert.False(exposureCheckPageViewModel.IsVisibleLowRiskContact);
            Assert.True(exposureCheckPageViewModel.IsVisibleNoRiskContact);
        }
    }
}