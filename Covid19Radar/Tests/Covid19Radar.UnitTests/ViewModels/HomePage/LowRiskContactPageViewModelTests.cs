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
using System.Globalization;
using Covid19Radar.Resources;
using Newtonsoft.Json;

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class LowRiskContactPageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IUserDataRepository> mockUserDataRepository;

        public LowRiskContactPageViewModelTests()
        {

            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockUserDataRepository = mockRepository.Create<IUserDataRepository>();
        }

        private LowRiskContactPageViewModel CreateViewModel()
        {
            return new LowRiskContactPageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                mockUserDataRepository.Object
                );
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
        public void Initialize_Time(string language, int[] seconds, string expected)
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

            var lowRiskContactPageViewModel = CreateViewModel();
            var parameters = new NavigationParameters();
            lowRiskContactPageViewModel.Initialize(parameters);

            mockUserDataRepository
                .Verify(x => x.GetExposureWindowsAsync(), Times.Once);
            Assert.Equal(expected, lowRiskContactPageViewModel.TotalContactTime);

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
        public void Initialize_Time_Irregular_Time(string language, int[] seconds, string expected)
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

            var lowRiskContactPageViewModel = CreateViewModel();
            var parameters = new NavigationParameters();
            lowRiskContactPageViewModel.Initialize(parameters);

            mockUserDataRepository
                .Verify(x => x.GetExposureWindowsAsync(), Times.Once);
            Assert.Equal(expected, lowRiskContactPageViewModel.TotalContactTime);

            AppResources.Culture = originalCalture;
        }

        [Theory]
        [InlineData("ja-JP", "0分")]
        [InlineData("en-US", "0min")]
        [InlineData("zh-Hans", "0分钟")]
        public void Initialize_Time_Irregular_ParseError_ja(string cluture, string expected)
        {
            var originalCalture = AppResources.Culture;
            AppResources.Culture = new CultureInfo(cluture);

            mockUserDataRepository
                .Setup(x => x.GetExposureWindowsAsync())
                .Throws(new JsonSerializationException("parse error mock"));

            var lowRiskContactPageViewModel = CreateViewModel();
            var parameters = new NavigationParameters();
            lowRiskContactPageViewModel.Initialize(parameters);

            mockUserDataRepository
                .Verify(x => x.GetExposureWindowsAsync(), Times.Once);
            Assert.Equal(expected, lowRiskContactPageViewModel.TotalContactTime);

            AppResources.Culture = originalCalture;
        }
    }
}