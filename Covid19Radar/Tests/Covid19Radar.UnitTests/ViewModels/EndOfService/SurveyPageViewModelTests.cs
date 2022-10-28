// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Reflection;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.UnitTests.Mocks;
using Covid19Radar.ViewModels.EndOfService;
using Covid19Radar.Views.EndOfService;
using Moq;
using Prism.Navigation;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Covid19Radar.UnitTests.ViewModels.EndOfService
{
    public class SurveyPageViewModelTests : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly MockRepository _mockRepository;
        private readonly Mock<INavigationService> _mockNavigationService;
        private readonly Mock<ISurveyService> _mockSurveyService;
        private readonly Mock<IUserDataRepository> _mockUserDataRepository;

        public SurveyPageViewModelTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockNavigationService = _mockRepository.Create<INavigationService>();
            _mockSurveyService = _mockRepository.Create<ISurveyService>();
            _mockUserDataRepository = _mockRepository.Create<IUserDataRepository>();

            MockTimeZoneInfo.SetJstLocalTimeZone();
        }

        public void Dispose()
        {
            MockTimeZoneInfo.ClearLocalTimeZone();
        }

        private SurveyPageViewModel CreateViewModel()
        {
            return new SurveyPageViewModel(
                _mockNavigationService.Object,
                _mockSurveyService.Object,
                _mockUserDataRepository.Object
                );
        }

        [Fact]
        public void InitializeTests()
        {
            DateTime testNow = DateTime.UtcNow;
            _mockUserDataRepository.Setup(x => x.GetStartDate()).Returns(testNow);

            SurveyPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            _mockUserDataRepository.Verify(x => x.GetStartDate(), Times.Once());

            Assert.Equal(0, unitUnderTest.SelectedIndexQ1);
            Assert.Equal(0, unitUnderTest.SelectedIndexQ2);
            Assert.Equal(testNow.ToString("yyyy年MM月dd日"), unitUnderTest.AppStartDate);
        }

        [Theory]
        [InlineData(2021, 12, 31, 14, 59, 59, 2021, 12, 31)]
        [InlineData(2021, 12, 31, 15, 0, 0, 2022, 1, 1)]
        [InlineData(2022, 1, 1, 0, 0, 0, 2022, 1, 1)]
        public void InitializeTests_StartDateDefault(
            int testYear, int testMonth, int testDay, int testHour, int testMinute, int testSeconds,
            int expectedYear, int expectedMonth, int expectedDay
            )
        {
            DateTime testStartDate = new DateTimeOffset(testYear, testMonth, testDay, testHour, testMinute, testSeconds, TimeSpan.Zero).DateTime;

            _mockUserDataRepository.Setup(x => x.GetStartDate()).Returns(testStartDate);

            SurveyPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            _testOutputHelper.WriteLine($"TimeZoneInfo.Local: {TimeZoneInfo.Local}");

            if (MockTimeZoneInfo.IsJst())
            {
                var expectedText = string.Format("{0}年{1}月{2}日", expectedYear, expectedMonth.ToString("D2"), expectedDay.ToString("D2"));
                Assert.Equal(expectedText, unitUnderTest.AppStartDate);
            }
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(1, 0, false)]
        [InlineData(0, 1, false)]
        [InlineData(1, 1, true)]
        public void SurveyPageToTerminationOfUsePageButtonEnabledTests(
            int testQ1Index, int testQ2Index, bool expectedIsEnabled)
        {
            SurveyPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            unitUnderTest.SelectedIndexQ1 = testQ1Index;
            unitUnderTest.SelectedIndexQ2 = testQ2Index;

            Assert.Equal(expectedIsEnabled, unitUnderTest.IsTerminationOfUsePageButtonEnabled);
        }

        [Fact]
        public async Task OnToTerminationOfUsePageButtonTests()
        {
            var testSurveyContent = new SurveyContent { Q1 = 1, Q2 = 2, StartDate = 1640962800 };
            _mockSurveyService.Setup(x =>
                x.BuildSurveyContent(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>())
            )
            .ReturnsAsync(testSurveyContent);

            SurveyPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            unitUnderTest.SelectedItemQ1 = new SurveyAnswerPickerItem { Value = 1 };
            unitUnderTest.SelectedItemQ2 = new SurveyAnswerPickerItem { Value = 2 };
            unitUnderTest.IsAppStartDate = true;
            unitUnderTest.IsExposureDataProvision = true;

            await unitUnderTest.OnToTerminationOfUsePageButton.ExecuteAsync();

            _mockSurveyService.Verify(
                x => x.BuildSurveyContent(
                    1, 2, true, true
                ),
                Times.Once());
            _mockNavigationService.Verify(
                x => x.NavigateAsync(
                    "TerminationOfUsePage",
                    It.Is<INavigationParameters>(x => x[TerminationOfUsePage.NavigationParameterNameSurveyContent].Equals(testSurveyContent))),
                Times.Once()
            );
        }
    }
}

