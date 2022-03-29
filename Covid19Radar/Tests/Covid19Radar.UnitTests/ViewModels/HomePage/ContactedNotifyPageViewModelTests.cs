// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
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
    public class ContactedNotifyPageViewModelTests : IDisposable
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IExposureRiskCalculationConfigurationRepository> mockExposureRiskCalculationConfigurationRepository;
        private readonly Mock<IExposureRiskCalculationService> mockExposureRiskCalculationService;
        private readonly Mock<IExposureDataRepository> mockExposureDataRepository;
        private readonly CultureInfo originalAppResourceCalture;
        private readonly CultureInfo originalThreadCalture;
        private readonly CultureInfo originalThreadUICalture;

        public ContactedNotifyPageViewModelTests()
        {
            originalAppResourceCalture = AppResources.Culture;
            originalThreadCalture = Thread.CurrentThread.CurrentCulture;
            originalThreadUICalture = Thread.CurrentThread.CurrentUICulture;
            AppResources.Culture = new CultureInfo("ja-JP");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ja-JP");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja-JP");

            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockExposureRiskCalculationService = mockRepository.Create<IExposureRiskCalculationService>();
            mockExposureRiskCalculationConfigurationRepository = mockRepository.Create<IExposureRiskCalculationConfigurationRepository>();
            mockExposureDataRepository = mockRepository.Create<IExposureDataRepository>();
        }

        public void Dispose()
        {
            AppResources.Culture = originalAppResourceCalture;
            Thread.CurrentThread.CurrentCulture = originalThreadCalture;
            Thread.CurrentThread.CurrentUICulture = originalThreadUICalture;
        }

        private ContactedNotifyPageViewModel CreateViewModel()
        {

            return new ContactedNotifyPageViewModel(
                    mockNavigationService.Object,
                    mockLoggerService.Object,
                    mockExposureDataRepository.Object,
                    mockExposureRiskCalculationService.Object,
                    mockExposureRiskCalculationConfigurationRepository.Object
                );
        }

        [Fact(Skip = "Skipped due to test failure due to execution environment")]
        public void OnClickExposuresTest_Initialize()
        {
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
                        DateMillisSinceEpoch = 1000l * 60 * 60 * 24 * 365
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
            mockExposureRiskCalculationConfigurationRepository
                .Setup(x => x.GetExposureRiskCalculationConfigurationAsync(It.IsAny<bool>()))
                .ReturnsAsync(new V1ExposureRiskCalculationConfiguration());
            mockExposureRiskCalculationService
                .Setup(x => x.CalcRiskLevel(It.IsAny<DailySummary>(), It.IsAny<List<ExposureWindow>>(), It.IsAny<V1ExposureRiskCalculationConfiguration>()))
                .Returns(RiskLevel.High);

            var contactedNotifyViewModel = CreateViewModel();
            contactedNotifyViewModel.Initialize(new NavigationParameters());

            Assert.Equal("0001年1月1日 月曜日以前\n2 件", contactedNotifyViewModel.ExposureCount);
            Assert.Equal("1971年1月1日 金曜日以降\n1日間に合計1分間の接触", contactedNotifyViewModel.ExposureDurationInMinutes);
        }

        [Fact]
        public void OnClickExposuresTest_Initialize_NoExposureInformation_HighRisk()
        {
            mockExposureDataRepository
                .Setup(x => x.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(new List<UserExposureInfo>());
            mockExposureDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(Task.FromResult(new List<DailySummary>()
                {
                    new DailySummary()
                    {
                        DateMillisSinceEpoch = 1000l * 60 * 60 * 24 * 365
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
            mockExposureRiskCalculationConfigurationRepository
                .Setup(x => x.GetExposureRiskCalculationConfigurationAsync(It.IsAny<bool>()))
                .ReturnsAsync(new V1ExposureRiskCalculationConfiguration());
            mockExposureRiskCalculationService
                .Setup(x => x.CalcRiskLevel(It.IsAny<DailySummary>(), It.IsAny<List<ExposureWindow>>(), It.IsAny<V1ExposureRiskCalculationConfiguration>()))
                .Returns(RiskLevel.High);

            var contactedNotifyViewModel = CreateViewModel();
            contactedNotifyViewModel.Initialize(new NavigationParameters());

            Assert.Empty(contactedNotifyViewModel.ExposureCount);
            Assert.Equal("1日間に合計1分間の接触", contactedNotifyViewModel.ExposureDurationInMinutes);
        }

        [Fact]
        public void OnClickExposuresTest_Initialize_NoExposureInformation_NoHighRisk()
        {
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
                        DateMillisSinceEpoch = 1000l * 60 * 60 * 24 * 365
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
            mockExposureRiskCalculationConfigurationRepository
                .Setup(x => x.GetExposureRiskCalculationConfigurationAsync(It.IsAny<bool>()))
                .ReturnsAsync(new V1ExposureRiskCalculationConfiguration());

            var contactedNotifyViewModel = CreateViewModel();
            contactedNotifyViewModel.Initialize(new NavigationParameters());

            Assert.Empty(contactedNotifyViewModel.ExposureDurationInMinutes);
            Assert.Equal("2 件", contactedNotifyViewModel.ExposureCount);
        }

        [Fact]
        public void OnClickExposuresTest_Initialize_Exception()
        {

            mockExposureRiskCalculationConfigurationRepository
                .Setup(x => x.GetExposureRiskCalculationConfigurationAsync(true))
                .Throws(new HttpRequestException());

            var contactedNotifyViewModel = CreateViewModel();
            contactedNotifyViewModel.Initialize(new NavigationParameters());

            mockLoggerService
                .Verify(x => x.Exception(
                    "Failed to Initialize",
                    It.IsAny<Exception>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()), Times.Once);
        }
    }
}
