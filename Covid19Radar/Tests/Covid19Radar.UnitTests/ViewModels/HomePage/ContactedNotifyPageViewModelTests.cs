// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Moq;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class ContactedNotifyPageViewModelTests: IDisposable
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IExposureRiskCalculationConfigurationRepository> mockExposureRiskCalculationConfigurationRepository;
        private readonly Mock<IExposureRiskCalculationService> mockExposureRiskCalculationService;
        private readonly Mock<IDialogService> mockDialogService;
        private readonly Mock<IExposureDataRepository> mockExposureDataRepository;
        private readonly CultureInfo originalCalture;

        public ContactedNotifyPageViewModelTests()
        {
            originalCalture = AppResources.Culture;
            AppResources.Culture = new CultureInfo("ja-JP");

            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockExposureRiskCalculationService = mockRepository.Create<IExposureRiskCalculationService>();
            mockDialogService = mockRepository.Create<IDialogService>();
            mockExposureRiskCalculationConfigurationRepository = mockRepository.Create<IExposureRiskCalculationConfigurationRepository>();
            mockExposureDataRepository = mockRepository.Create<IExposureDataRepository>();
        }

        public void Dispose()
        {
            AppResources.Culture = originalCalture;
        }

        private ContactedNotifyPageViewModel CreateViewModel()
        {

            return new ContactedNotifyPageViewModel(
                    mockNavigationService.Object,
                    mockLoggerService.Object,
                    mockExposureDataRepository.Object,
                    mockExposureRiskCalculationService.Object,
                    mockExposureRiskCalculationConfigurationRepository.Object,
                    mockDialogService.Object
                );
        }

        [Fact]
        public void OnClickExposuresTest_()
        {

            mockExposureRiskCalculationConfigurationRepository
                .Setup(x => x.GetExposureRiskCalculationConfigurationAsync(true))
                .Returns(Task.FromResult(new V1ExposureRiskCalculationConfiguration()
                {
                    DailySummary_DaySummary_ScoreSum = new V1ExposureRiskCalculationConfiguration.Threshold() {
                        Op = "=",
                        Value = 1.0
                    }
                })); ;
            mockExposureDataRepository
                .Setup(x => x.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(new List<UserExposureInfo>()
                {
                    new UserExposureInfo(),
                    new UserExposureInfo()
                });
            mockExposureDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(Task.FromResult(new List<DailySummary>()
                {
                    new DailySummary()
                    {
                        DateMillisSinceEpoch = 1000l * 60 * 60 * 24 * 365,
                        DaySummary = new ExposureSummaryData()
                        {
                            ScoreSum = 1.0
                        }
                    }
                }));
            mockExposureDataRepository
                .Setup(x => x.GetExposureWindowsAsync(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(Task.FromResult(new List<ExposureWindow>()
                {
                    new ExposureWindow()
                    {
                        DateMillisSinceEpoch = 1000l * 60 * 60 * 24 * 365,
                        ScanInstances = new List<ScanInstance>() {
                            new ScanInstance()
                            {
                                SecondsSinceLastScan = 60
                            }
                        }
                    }
                }));


            var contactedNotifyViewModel = CreateViewModel();
            contactedNotifyViewModel.Initialize(new NavigationParameters());

            Assert.Equal("2 件", contactedNotifyViewModel.ExposureCount);
            Assert.Equal("", contactedNotifyViewModel.ExposureDurationInMinutes);
        }


        [Fact]
        public void OnClickExposuresTest_Exception()
        {

            mockExposureRiskCalculationConfigurationRepository
                .Setup(x => x.GetExposureRiskCalculationConfigurationAsync(true))
                .Throws(new HttpRequestException());

            var contactedNotifyViewModel = CreateViewModel();
            contactedNotifyViewModel.Initialize(new NavigationParameters());

            mockDialogService
                .Verify(x => x.ShowUnknownErrorWaringAsync(), Times.Once);
        }
    }
}
