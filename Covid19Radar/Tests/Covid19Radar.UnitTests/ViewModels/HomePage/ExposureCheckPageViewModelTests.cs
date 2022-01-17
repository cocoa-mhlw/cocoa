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

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class ExposureCheckPageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IExposureDataRepository> mockExposureDataRepository;

        public ExposureCheckPageViewModelTests()
        {

            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockExposureDataRepository = mockRepository.Create<IExposureDataRepository>();
        }

        private ExposureCheckPageViewModel CreateViewModel()
        {
            return new ExposureCheckPageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                mockExposureDataRepository.Object
                );
        }

        [Fact]
        public void LowRiskPage_Initialize_Display()
        {
            mockExposureDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(Task.FromResult(new List<DailySummary>()
                {
                    new DailySummary()
                    {
                        DaySummary = new ExposureSummaryData()
                        {
                            ScoreSum = 1.23456
                        },
                        DateMillisSinceEpoch = (1000 * 60 * 60 * 24)
                    },
                    new DailySummary()
                    {
                        DaySummary = new ExposureSummaryData()
                        {
                            ScoreSum = 17.8
                        },
                        DateMillisSinceEpoch = (1000 * 60 * 60 * 24) * 2
                    }
                }));

            var exposureCheckPageViewModel = CreateViewModel();
            var parameters = ExposureCheckPage.BuildNavigationParams(new V1ExposureRiskCalculationConfiguration());
            exposureCheckPageViewModel.Initialize(parameters);

            

            Assert.True(exposureCheckPageViewModel.IsVisibleLowRiskContact);
            Assert.False(exposureCheckPageViewModel.IsVisibleNoRiskContact);
            Assert.Equal(2, exposureCheckPageViewModel.ExposureCheckScores.Count());
            Assert.Equal("17.80", exposureCheckPageViewModel.ExposureCheckScores[0].DailySummaryScoreSumString);
            Assert.Equal((1000 * 60 * 60 * 24) * 2, exposureCheckPageViewModel.ExposureCheckScores[0].DateMillisSinceEpoch);
            Assert.Equal("1.23", exposureCheckPageViewModel.ExposureCheckScores[1].DailySummaryScoreSumString);
            Assert.Equal(1000 * 60 * 60 * 24, exposureCheckPageViewModel.ExposureCheckScores[1].DateMillisSinceEpoch);
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