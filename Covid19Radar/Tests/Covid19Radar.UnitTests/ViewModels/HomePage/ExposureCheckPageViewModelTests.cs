// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using Covid19Radar.ViewModels;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Moq;
using Prism.Navigation;
using Xunit;
using Chino;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using Covid19Radar.Resources;
using Newtonsoft.Json;
using Covid19Radar.Services;
using Covid19Radar.Common;

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class ExposureCheckPageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IUserDataRepository> mockUserDataRepository;
        private readonly IExposureRiskCalculationService exposureRiskCalculationService;

        public ExposureCheckPageViewModelTests()
        {

            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockUserDataRepository = mockRepository.Create<IUserDataRepository>();
            exposureRiskCalculationService = new ExposureRiskCalculationService(
                userDataRepository: mockUserDataRepository.Object,
                loggerService: mockLoggerService.Object);
        }

        private ExposureCheckPageViewModel CreateViewModel()
        {
            return new ExposureCheckPageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                mockUserDataRepository.Object,
                exposureRiskCalculationService
                );
        }

        [Fact]
        public void LowRiskPage_Initialize_Display()
        {
            mockUserDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(Task.FromResult(new List<DailySummary>() { new DailySummary() }));
            mockUserDataRepository
                .Setup(x => x.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(new List<UserExposureInfo>() { new UserExposureInfo() });

            var exposureCheckPageViewModel = CreateViewModel();
            var parameters = new NavigationParameters();
            exposureCheckPageViewModel.Initialize(parameters);

            Assert.True(exposureCheckPageViewModel.IsVisibleLowRiskContact);
            Assert.False(exposureCheckPageViewModel.IsVisibleNoRiskContact);
        }

        [Fact]
        public void NoRiskPage_Initialize_Display()
        {
            mockUserDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(Task.FromResult(new List<DailySummary>()));
            mockUserDataRepository
                .Setup(x => x.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(new List<UserExposureInfo>());

            var exposureCheckPageViewModel = CreateViewModel();
            var parameters = new NavigationParameters();
            exposureCheckPageViewModel.Initialize(parameters);
            Assert.False(exposureCheckPageViewModel.IsVisibleLowRiskContact);
            Assert.True(exposureCheckPageViewModel.IsVisibleNoRiskContact);
        }

        [Theory]
        [InlineData("ja-JP", new int[] { 60 }, "1分")]
        [InlineData("ja-JP", new int[] { 60, 60 }, "2分")]
        [InlineData("ja-JP", new int[] { 60 * 60 }, "1時間")]
        [InlineData("ja-JP", new int[] { 60 * 59, 60 * 5 }, "1時間4分")]
        [InlineData("ja-JP", new int[] { 60 * 60 * 10 }, "10時間")]
        [InlineData("ja-JP", new int[] { 60 * 50, 60 * 20, 60 * 100 }, "2時間50分")]
        [InlineData("en-US", new int[] { 60 }, "1min")]
        [InlineData("en-US", new int[] { 60, 60 }, "2min")]
        [InlineData("en-US", new int[] { 60 * 60 }, "1h")]
        [InlineData("en-US", new int[] { 60 * 59, 60 * 5 }, "1h4min")]
        [InlineData("en-US", new int[] { 60 * 60 * 10 }, "10h")]
        [InlineData("en-US", new int[] { 60 * 50, 60 * 20, 60 * 100 }, "2h50min")]
        [InlineData("zh-Hans",new int[] { 60 }, "1分钟")]
        [InlineData("zh-Hans", new int[] { 60, 60 }, "2分钟")]
        [InlineData("zh-Hans", new int[] { 60 * 60 }, "1小时")]
        [InlineData("zh-Hans", new int[] { 60 * 59, 60 * 5 }, "1小时4分钟")]
        [InlineData("zh-Hans", new int[] { 60 * 60 * 10 }, "10小时")]
        [InlineData("zh-Hans", new int[] { 60 * 50, 60 * 20, 60 * 100 }, "2小时50分钟")]
        public void LowRiskPage_Initialize_Time(string language, int[] seconds, string expected)
        {
            var originalCalture = AppResources.Culture;
            AppResources.Culture = new CultureInfo(language);

            var dummyExposureWindowList = new List<ExposureWindow>();

            foreach (var second in seconds)
            {
                dummyExposureWindowList.Add(
                    new ExposureWindow()
                    {
                        ScanInstances = new List<ScanInstance>()
                        {
                            new ScanInstance()
                            {
                                SecondsSinceLastScan = second
                            }
                        }
                    }
                );
            }

            mockUserDataRepository
                .Setup(x => x.GetExposureWindowsAsync())
                .Returns(Task.FromResult(dummyExposureWindowList));
            mockUserDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(Task.FromResult(new List<DailySummary>() { new DailySummary() }));
            mockUserDataRepository
                .Setup(x => x.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(new List<UserExposureInfo>() { new UserExposureInfo() });

            var exposureCheckPageViewModel = CreateViewModel();
            var parameters = new NavigationParameters();
            exposureCheckPageViewModel.Initialize(parameters);

            mockUserDataRepository
                .Verify(x => x.GetExposureWindowsAsync(), Times.Once);
            Assert.Equal(expected, exposureCheckPageViewModel.TotalContactTime);

            AppResources.Culture = originalCalture;
        }

        [Theory]
        [InlineData("ja-JP", new int[] { 5 }, "0分")]
        [InlineData("ja-JP", new int[] { -5 }, "0分")]
        [InlineData("ja-JP", new int[] { 59 }, "0分")]
        [InlineData("ja-JP", new int[] { 61 }, "1分")]
        [InlineData("ja-JP", new int[] { -60 }, "0分")]
        [InlineData("ja-JP", new int[] { 3599 }, "59分")]
        [InlineData("ja-JP", new int[] { 3601 }, "1時間")]
        [InlineData("ja-JP", new int[] { 5432 }, "1時間30分")]
        [InlineData("en-US", new int[] { 5 }, "0min")]
        [InlineData("en-US", new int[] { -5 }, "0min")]
        [InlineData("en-US", new int[] { 59 }, "0min")]
        [InlineData("en-US", new int[] { 61 }, "1min")]
        [InlineData("en-US", new int[] { -60 }, "0min")]
        [InlineData("en-US", new int[] { 3599 }, "59min")]
        [InlineData("en-US", new int[] { 3601 }, "1h")]
        [InlineData("en-US", new int[] { 5432 }, "1h30min")]
        [InlineData("zh-Hans", new int[] { 5 }, "0分钟")]
        [InlineData("zh-Hans", new int[] { -5 }, "0分钟")]
        [InlineData("zh-Hans", new int[] { 59 }, "0分钟")]
        [InlineData("zh-Hans", new int[] { 61 }, "1分钟")]
        [InlineData("zh-Hans", new int[] { -60 }, "0分钟")]
        [InlineData("zh-Hans", new int[] { 3599 }, "59分钟")]
        [InlineData("zh-Hans", new int[] { 3601 }, "1小时")]
        [InlineData("zh-Hans", new int[] { 5432 }, "1小时30分钟")]
        public void LowRiskPage_Initialize_Time_Irregular_Time(string language, int[] seconds, string expected)
        {
            var originalCalture = AppResources.Culture;
            AppResources.Culture = new CultureInfo(language);

            var dummyExposureWindowList = new List<ExposureWindow>();

            foreach (var second in seconds)
            {
                dummyExposureWindowList.Add(
                    new ExposureWindow()
                    {
                        ScanInstances = new List<ScanInstance>()
                        {
                            new ScanInstance()
                            {
                                SecondsSinceLastScan = second
                            }
                        }
                    }
                );
            }

            mockUserDataRepository
                .Setup(x => x.GetExposureWindowsAsync())
                .Returns(Task.FromResult(dummyExposureWindowList));
            mockUserDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(Task.FromResult(new List<DailySummary>() { new DailySummary() }));
            mockUserDataRepository
                .Setup(x => x.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(new List<UserExposureInfo>() { new UserExposureInfo() });

            var exposureCheckPageViewModel = CreateViewModel();
            var parameters = new NavigationParameters();
            exposureCheckPageViewModel.Initialize(parameters);

            mockUserDataRepository
                .Verify(x => x.GetExposureWindowsAsync(), Times.Once);
            Assert.Equal(expected, exposureCheckPageViewModel.TotalContactTime);

            AppResources.Culture = originalCalture;
        }

        [Theory]
        [InlineData("ja-JP", "0分")]
        [InlineData("en-US", "0min")]
        [InlineData("zh-Hans", "0分钟")]
        public void LowRiskPage_Initialize_Time_Irregular_ParseError_ja(string cluture, string expected)
        {
            var originalCalture = AppResources.Culture;
            AppResources.Culture = new CultureInfo(cluture);

            mockUserDataRepository
                .Setup(x => x.GetExposureWindowsAsync())
                .Throws(new JsonSerializationException("parse error mock"));
            mockUserDataRepository
                .Setup(x => x.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(Task.FromResult(new List<DailySummary>() { new DailySummary() }));
            mockUserDataRepository
                .Setup(x => x.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay))
                .Returns(new List<UserExposureInfo>() { new UserExposureInfo() });

            var exposureCheckPageViewModel = CreateViewModel();
            var parameters = new NavigationParameters();
            exposureCheckPageViewModel.Initialize(parameters);

            mockUserDataRepository
                .Verify(x => x.GetExposureWindowsAsync(), Times.Once);
            Assert.Equal(expected, exposureCheckPageViewModel.TotalContactTime);

            AppResources.Culture = originalCalture;
        }
    }
}