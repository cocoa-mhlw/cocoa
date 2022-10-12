// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.UnitTests.Mocks;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Covid19Radar.UnitTests.Services
{
    public class SurveyServiceTests : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly MockRepository _mockRepository;
        private readonly Mock<IEventLogService> _mockEventLogService;
        private readonly Mock<IExposureDataRepository> _mockExposureDataRepository;

        public SurveyServiceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockEventLogService = _mockRepository.Create<IEventLogService>();
            _mockExposureDataRepository = _mockRepository.Create<IExposureDataRepository>();

            MockTimeZoneInfo.SetJstLocalTimeZone();
        }

        public void Dispose()
        {
            MockTimeZoneInfo.ClearLocalTimeZone();
        }

        private SurveyService CreateService()
        {
            return new SurveyService(
                _mockEventLogService.Object,
                _mockExposureDataRepository.Object
                );
        }

        [Fact]
        public async Task BuildSurveyContentTests_HasExposureDataProvision()
        {
            _mockExposureDataRepository.Setup(x => x.GetDailySummariesAsync()).ReturnsAsync(
                new List<DailySummary>
                {
                    new DailySummary
                    {
                        DateMillisSinceEpoch = 1640962800,
                        DaySummary = new ExposureSummaryData
                        {
                            MaximumScore = 1350.0
                        }
                    }
                });

            SurveyService unitUnderTest = CreateService();
            SurveyContent result = await unitUnderTest.BuildSurveyContent(1, 2, new DateTimeOffset(2022, 1, 1, 0, 0, 0, new TimeSpan(9, 0, 0)).DateTime, true);

            _testOutputHelper.WriteLine($"TimeZoneInfo.Local: {TimeZoneInfo.Local}");

            Assert.Equal(1, result.Q1);
            Assert.Equal(2, result.Q2);
            if (MockTimeZoneInfo.IsJst())
            {
                Assert.Equal(1640962800, result.Q3);
            }
            Assert.Single(result.ExposureData.DailySummaryList);

            _mockExposureDataRepository.Verify(x => x.GetDailySummariesAsync(), Times.Once());
        }

        [Fact]
        public async Task BuildSurveyContentTests_HasNotExposureDataProvision()
        {
            SurveyService unitUnderTest = CreateService();
            SurveyContent result = await unitUnderTest.BuildSurveyContent(1, 2, new DateTimeOffset(2022, 1, 1, 0, 0, 0, new TimeSpan(9, 0, 0)).DateTime, false);

            _testOutputHelper.WriteLine($"TimeZoneInfo.Local: {TimeZoneInfo.Local}");

            Assert.Equal(1, result.Q1);
            Assert.Equal(2, result.Q2);
            if (MockTimeZoneInfo.IsJst())
            {
                Assert.Equal(1640962800, result.Q3);
            }
            Assert.Null(result.ExposureData);

            _mockExposureDataRepository.Verify(x => x.GetDailySummariesAsync(), Times.Never());
        }

        [Fact]
        public async Task BuildSurveyContentTests_DailySummary()
        {
            _mockExposureDataRepository.Setup(x => x.GetDailySummariesAsync()).ReturnsAsync(
                new List<DailySummary>
                {
                    new DailySummary
                    {
                        DateMillisSinceEpoch = 1640962800,
                        DaySummary = new ExposureSummaryData
                        {
                            MaximumScore = 1350.0
                        }
                    },
                    new DailySummary
                    {
                        DateMillisSinceEpoch = 1640962801,
                        DaySummary = new ExposureSummaryData
                        {
                            MaximumScore = 1349.0
                        }
                    }
                });

            SurveyService unitUnderTest = CreateService();
            SurveyContent result = await unitUnderTest.BuildSurveyContent(1, 2, new DateTimeOffset(2022, 1, 1, 0, 0, 0, new TimeSpan(9, 0, 0)).DateTime, true);

            List<SurveyExposureData.DailySummary> dailySummaryList = result.ExposureData.DailySummaryList;
            Assert.Equal(2, dailySummaryList.Count);
            Assert.Equal(1640962800, dailySummaryList[0].DateMillisSinceEpoch);
            Assert.Equal(1, dailySummaryList[0].ExposureDetected);
            Assert.Equal(1640962801, dailySummaryList[1].DateMillisSinceEpoch);
            Assert.Equal(0, dailySummaryList[1].ExposureDetected);

            _mockExposureDataRepository.Verify(x => x.GetDailySummariesAsync(), Times.Once());
        }

        [Fact]
        public async Task SubmitSurveyTests()
        {
            SurveyContent testSurveyContent = new SurveyContent { Q1 = 1, Q2 = 2, Q3 = 1640962800, ExposureData = new SurveyExposureData { } };

            _mockEventLogService.Setup(x => x.SendAsync(It.IsAny<EventLog>())).ReturnsAsync(true);
            
            SurveyService unitUnderTest = CreateService();
            bool result = await unitUnderTest.SubmitSurvey(testSurveyContent);

            Assert.True(result);

            _mockEventLogService.Verify(x => x.SendAsync(
                It.Is<EventLog>(x =>
                    x.Type == "survey" &&
                    x.Subtype == "survey" &&
                    x.Content.Value<int>("q1") == 1 &&
                    x.Content.Value<int>("q2") == 2 &&
                    x.Content.Value<long>("q3") == 1640962800 &&
                    x.Content.Value<JObject>("exposure_data") != null
                )),
                Times.Once());
        }
    }
}

