// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
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
        [InlineData(new int[]{ 60 }, "1分")]
        [InlineData(new int[] { 60, 60 }, "2分")]
        [InlineData(new int[] { 60 * 60 }, "1時間")]
        [InlineData(new int[] { 60 * 59, 60 * 5 }, "1時間4分")]
        [InlineData(new int[] { 60 * 60 * 10 }, "10時間")]
        [InlineData(new int[] { 60 * 50, 60 * 20, 60 * 100 }, "2時間50分")]
        public void Initialize_Time_ja(int[] seconds, string expected)
        {
            var originalCalture = AppResources.Culture;
            AppResources.Culture = new CultureInfo("ja-JP");

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
        [InlineData(new int[] { 60 }, "1min")]
        [InlineData(new int[] { 60, 60 }, "2min")]
        [InlineData(new int[] { 60 * 60 }, "1h")]
        [InlineData(new int[] { 60 * 59, 60 * 5 }, "1h4min")]
        [InlineData(new int[] { 60 * 60 * 10 }, "10h")]
        [InlineData(new int[] { 60 * 50, 60 * 20, 60 * 100 }, "2h50min")]
        public void Initialize_Time_en(int[] seconds, string expected)
        {
            var originalCalture = AppResources.Culture;
            AppResources.Culture = new CultureInfo("en-US");

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
        [InlineData(new int[] { 60 }, "1分钟")]
        [InlineData(new int[] { 60, 60 }, "2分钟")]
        [InlineData(new int[] { 60 * 60 }, "1小时")]
        [InlineData(new int[] { 60 * 59, 60 * 5 }, "1小时4分钟")]
        [InlineData(new int[] { 60 * 60 * 10 }, "10小时")]
        [InlineData(new int[] { 60 * 50, 60 * 20, 60 * 100 }, "2小时50分钟")]
        public void Initialize_Time_zh(int[] seconds, string expected)
        {
            var originalCalture = AppResources.Culture;
            AppResources.Culture = new CultureInfo("zh-Hans");

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
        [InlineData(new int[] { 5 }, "0分")]
        [InlineData(new int[] { -5 }, "0分")]
        [InlineData(new int[] { 59 }, "0分")]
        [InlineData(new int[] { 61 }, "1分")]
        [InlineData(new int[] { 3599 }, "59分")]
        [InlineData(new int[] { 3601 }, "1時間")]
        [InlineData(new int[] { 5432 }, "1時間30分")]
        public void Initialize_Time_Irregular_Time(int[] seconds, string expected)
        {
            var originalCalture = AppResources.Culture;
            AppResources.Culture = new CultureInfo("ja-JP");

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

        [Fact]
        public void Initialize_Time_Irregular_ParseError()
        {
            var originalCalture = AppResources.Culture;
            AppResources.Culture = new CultureInfo("ja-JP");

            mockUserDataRepository
                .Setup(x => x.GetExposureWindowsAsync())
                .Throws(new JsonSerializationException("parse error mock"));

            var lowRiskContactPageViewModel = CreateViewModel();
            var parameters = new NavigationParameters();
            lowRiskContactPageViewModel.Initialize(parameters);

            mockUserDataRepository
                .Verify(x => x.GetExposureWindowsAsync(), Times.Once);
            Assert.Equal("0分", lowRiskContactPageViewModel.TotalContactTime);

            AppResources.Culture = originalCalture;
        }
    }
}