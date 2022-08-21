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
using Covid19Radar.Views;
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
        private readonly Mock<IExposureRiskCalculationService> mockExposureRiskCalculationService;
        private readonly Mock<IExposureDataRepository> mockExposureDataRepository;
        private readonly Mock<IExposureRiskCalculationConfigurationRepository> mockExposureRiskCalculationConfigurationRepository;
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
            mockExposureDataRepository = mockRepository.Create<IExposureDataRepository>();
            mockExposureRiskCalculationConfigurationRepository = mockRepository.Create<IExposureRiskCalculationConfigurationRepository>();
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
                .Setup(x => x.GetExposureInformationList(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(new List<UserExposureInfo>()
                {
                    new UserExposureInfo(),
                    new UserExposureInfo()
                });
            mockExposureDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(Task.FromResult(new List<DailySummary>()
                {
                    new DailySummary()
                    {
                        DateMillisSinceEpoch = 1000L * 60 * 60 * 24 * 365
                    }
                }));
            mockExposureDataRepository
                .Setup(x => x.GetExposureWindowsAsync(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(Task.FromResult(new List<ExposureWindow>()
                {
                    new ExposureWindow()
                    {
                        DateMillisSinceEpoch = 1000L * 60 * 60 * 24 * 365,
                        ScanInstances = new List<ScanInstance>() {
                            new ScanInstance()
                            {
                                SecondsSinceLastScan = 60
                            }
                        }
                    }
                }));
            mockExposureRiskCalculationService
                .Setup(x => x.CalcRiskLevel(It.IsAny<DailySummary>(), It.IsAny<List<ExposureWindow>>(), It.IsAny<V1ExposureRiskCalculationConfiguration>()))
                .Returns(RiskLevel.High);

            var contactedNotifyViewModel = CreateViewModel();
            contactedNotifyViewModel.Initialize(ContactedNotifyPage.BuildNavigationParams(new V1ExposureRiskCalculationConfiguration()));

            Assert.Equal("0001年1月1日 月曜日以前\n2 件", contactedNotifyViewModel.ExposureCount);
            Assert.Equal("1971年1月1日 金曜日以降\n1日間に合計1分間の接触", contactedNotifyViewModel.ExposureDurationInMinutes);
        }

        [Fact]
        public void OnClickExposuresTest_Initialize_NoExposureInformation_HighRisk()
        {
            mockExposureDataRepository
                .Setup(x => x.GetExposureInformationList(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(new List<UserExposureInfo>());
            mockExposureDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(Task.FromResult(new List<DailySummary>()
                {
                    new DailySummary()
                    {
                        DateMillisSinceEpoch = 1000L * 60 * 60 * 24 * 365
                    }
                }));
            mockExposureDataRepository
                .Setup(x => x.GetExposureWindowsAsync(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(Task.FromResult(new List<ExposureWindow>()
                {
                    new ExposureWindow()
                    {
                        DateMillisSinceEpoch = 1000L * 60 * 60 * 24 * 365,
                        ScanInstances = new List<ScanInstance>() {
                            new ScanInstance()
                            {
                                SecondsSinceLastScan = 60
                            }
                        }
                    }
                }));
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
                .Setup(x => x.GetExposureInformationList(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(new List<UserExposureInfo>()
                {
                    new UserExposureInfo(),
                    new UserExposureInfo()
                });
            mockExposureDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(Task.FromResult(new List<DailySummary>()
                {
                    new DailySummary()
                    {
                        DateMillisSinceEpoch = 1000L * 60 * 60 * 24 * 365
                    }
                }));
            mockExposureDataRepository
                .Setup(x => x.GetExposureWindowsAsync(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(Task.FromResult(new List<ExposureWindow>()
                {
                    new ExposureWindow()
                    {
                        DateMillisSinceEpoch = 1000L * 60 * 60 * 24 * 365,
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

            Assert.Empty(contactedNotifyViewModel.ExposureDurationInMinutes);
            Assert.Equal("2 件", contactedNotifyViewModel.ExposureCount);
        }

        [Fact]
        public void OnClickExposuresTest_Initialize_NavigationParameter_NotSet()
        {
            mockExposureDataRepository
                .Setup(x => x.GetExposureInformationList(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(new List<UserExposureInfo>()
                {
                    new UserExposureInfo(),
                    new UserExposureInfo()
                });
            mockExposureDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(Task.FromResult(new List<DailySummary>()
                {
                    new DailySummary()
                    {
                        DateMillisSinceEpoch = 1000L * 60 * 60 * 24 * 365
                    }
                }));
            mockExposureDataRepository
                .Setup(x => x.GetExposureWindowsAsync(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(Task.FromResult(new List<ExposureWindow>()
                {
                    new ExposureWindow()
                    {
                        DateMillisSinceEpoch = 1000L * 60 * 60 * 24 * 365,
                        ScanInstances = new List<ScanInstance>() {
                            new ScanInstance()
                            {
                                SecondsSinceLastScan = 60
                            }
                        }
                    }
                }));

            mockExposureRiskCalculationConfigurationRepository
                .Setup(x => x.GetExposureRiskCalculationConfigurationAsync(true))
                .ReturnsAsync(new V1ExposureRiskCalculationConfiguration());

            var contactedNotifyViewModel = CreateViewModel();
            contactedNotifyViewModel.Initialize(new NavigationParameters());

            mockExposureRiskCalculationConfigurationRepository.Verify(x => x.GetExposureRiskCalculationConfigurationAsync(true), Times.Once());
        }

        [Fact]
        public async Task OnExposureListTest()
        {
            mockExposureDataRepository
                .Setup(x => x.GetExposureInformationList(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(new List<UserExposureInfo>()
                {
                    new UserExposureInfo(),
                    new UserExposureInfo()
                });
            mockExposureDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(Task.FromResult(new List<DailySummary>()
                {
                    new DailySummary()
                    {
                        DateMillisSinceEpoch = 1000L * 60 * 60 * 24 * 365
                    }
                }));
            mockExposureDataRepository
                .Setup(x => x.GetExposureWindowsAsync(AppConstants.TermOfExposureRecordValidityInDays))
                .Returns(Task.FromResult(new List<ExposureWindow>()
                {
                    new ExposureWindow()
                    {
                        DateMillisSinceEpoch = 1000L * 60 * 60 * 24 * 365,
                        ScanInstances = new List<ScanInstance>() {
                            new ScanInstance()
                            {
                                SecondsSinceLastScan = 60
                            }
                        }
                    }
                }));
            mockExposureRiskCalculationService
                .Setup(x => x.CalcRiskLevel(It.IsAny<DailySummary>(), It.IsAny<List<ExposureWindow>>(), It.IsAny<V1ExposureRiskCalculationConfiguration>()))
                .Returns(RiskLevel.High);

            var exposureRiskCalculationConfiguration =
                new V1ExposureRiskCalculationConfiguration
                {
                    FormatVersion = 1,
                    DailySummary_DaySummary_ScoreSum = new V1ExposureRiskCalculationConfiguration.Threshold { Op = "Op1", Value = 1 },
                    DailySummary_WeightedDurationAverage = new V1ExposureRiskCalculationConfiguration.Threshold { Op = "Op2", Value = 2 },
                    ExposureWindow_ScanInstance_SecondsSinceLastScanSum = new V1ExposureRiskCalculationConfiguration.Threshold { Op = "Op3", Value = 3 },
                    ExposureWindow_ScanInstance_TypicalAttenuationDb_Max = new V1ExposureRiskCalculationConfiguration.Threshold { Op = "Op4", Value = 4 },
                    ExposureWindow_ScanInstance_TypicalAttenuationDb_Min = new V1ExposureRiskCalculationConfiguration.Threshold { Op = "Op5", Value = 5 }
                };

            ContactedNotifyPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(ContactedNotifyPage.BuildNavigationParams(exposureRiskCalculationConfiguration));
            await unitUnderTest.OnExposureList.ExecuteAsync();

            mockNavigationService.Verify(x => x.NavigateAsync(
                nameof(ExposuresPage),
                It.Is<INavigationParameters>(x =>
                    x.ContainsKey(ExposuresPage.ExposureRiskCalculationConfigurationKey) &&
                    x.GetValue<V1ExposureRiskCalculationConfiguration>(ExposuresPage.ExposureRiskCalculationConfigurationKey).FormatVersion == 1 &&
                    x.GetValue<V1ExposureRiskCalculationConfiguration>(ExposuresPage.ExposureRiskCalculationConfigurationKey).DailySummary_DaySummary_ScoreSum.Op == "Op1" &&
                    x.GetValue<V1ExposureRiskCalculationConfiguration>(ExposuresPage.ExposureRiskCalculationConfigurationKey).DailySummary_DaySummary_ScoreSum.Value == 1 &&
                    x.GetValue<V1ExposureRiskCalculationConfiguration>(ExposuresPage.ExposureRiskCalculationConfigurationKey).DailySummary_WeightedDurationAverage.Op == "Op2" &&
                    x.GetValue<V1ExposureRiskCalculationConfiguration>(ExposuresPage.ExposureRiskCalculationConfigurationKey).DailySummary_WeightedDurationAverage.Value == 2 &&
                    x.GetValue<V1ExposureRiskCalculationConfiguration>(ExposuresPage.ExposureRiskCalculationConfigurationKey).ExposureWindow_ScanInstance_SecondsSinceLastScanSum.Op == "Op3" &&
                    x.GetValue<V1ExposureRiskCalculationConfiguration>(ExposuresPage.ExposureRiskCalculationConfigurationKey).ExposureWindow_ScanInstance_SecondsSinceLastScanSum.Value == 3 &&
                    x.GetValue<V1ExposureRiskCalculationConfiguration>(ExposuresPage.ExposureRiskCalculationConfigurationKey).ExposureWindow_ScanInstance_TypicalAttenuationDb_Max.Op == "Op4" &&
                    x.GetValue<V1ExposureRiskCalculationConfiguration>(ExposuresPage.ExposureRiskCalculationConfigurationKey).ExposureWindow_ScanInstance_TypicalAttenuationDb_Max.Value == 4 &&
                    x.GetValue<V1ExposureRiskCalculationConfiguration>(ExposuresPage.ExposureRiskCalculationConfigurationKey).ExposureWindow_ScanInstance_TypicalAttenuationDb_Min.Op == "Op5" &&
                    x.GetValue<V1ExposureRiskCalculationConfiguration>(ExposuresPage.ExposureRiskCalculationConfigurationKey).ExposureWindow_ScanInstance_TypicalAttenuationDb_Min.Value == 5
                )
            ));
        }
    }
}
