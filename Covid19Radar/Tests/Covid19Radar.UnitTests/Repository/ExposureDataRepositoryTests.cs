// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Covid19Radar.UnitTests.Repository
{
    public class ExposureDataRepositoryTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IPreferencesService> mockPreferencesService;
        private readonly Mock<IDateTimeUtility> mockDateTimeUtility;

        public ExposureDataRepositoryTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockPreferencesService = mockRepository.Create<IPreferencesService>();
            mockDateTimeUtility = mockRepository.Create<IDateTimeUtility>();
        }

        private IExposureDataRepository CreateRepository()
            => new ExposureDataRepository(
                mockPreferencesService.Object,
                mockDateTimeUtility.Object,
                mockLoggerService.Object
                );

        [Fact]
        public async void SetExposureDataTests_Once()
        {
            var existDailySummaries = new List<DailySummary>() {
            };
            var existExposureWindows = new List<ExposureWindow>()
            {
            };

            var addDailySummaries = new List<DailySummary>() {
                new DailySummary()
                {
                    DateMillisSinceEpoch = 10,
                    DaySummary = new ExposureSummaryData(),
                    ConfirmedClinicalDiagnosisSummary = new ExposureSummaryData(),
                    ConfirmedTestSummary = new ExposureSummaryData(),
                    RecursiveSummary = new ExposureSummaryData(),
                    SelfReportedSummary = new ExposureSummaryData()
                }
            };
            var addExposureWindows = new List<ExposureWindow>()
            {
                new ExposureWindow()
                {
                    CalibrationConfidence = CalibrationConfidence.High,
                    DateMillisSinceEpoch = 0,
                    Infectiousness = Infectiousness.High,
                    ReportType = ReportType.Unknown,
                    ScanInstances = new List<ScanInstance>()
                },
                new ExposureWindow()
                {
                    CalibrationConfidence = CalibrationConfidence.Medium,
                    DateMillisSinceEpoch = 0,
                    Infectiousness = Infectiousness.High,
                    ReportType = ReportType.ConfirmedTest,
                    ScanInstances = new List<ScanInstance>()
                }
            };

            // Mock Setup
            mockPreferencesService
                .Setup(x => x.GetValue(It.Is<string>(x => x == "DailySummaries"), It.IsAny<string>()))
                .Returns(JsonConvert.SerializeObject(existDailySummaries));
            mockPreferencesService
                .Setup(x => x.GetValue(It.Is<string>(x => x == "ExposureWindows"), It.IsAny<string>()))
                .Returns(JsonConvert.SerializeObject(existExposureWindows));

            var unitUnderTest = CreateRepository();

            var (newDailySummaryList, newExposureWindowList) = await unitUnderTest.SetExposureDataAsync(
                addDailySummaries,
                addExposureWindows
                );

            var expectedDailySummariesJson = JsonConvert.SerializeObject(addDailySummaries);
            var expectedExposureWindowsJson = JsonConvert.SerializeObject(addExposureWindows);

            // Assert
            Assert.Equal(addDailySummaries, newDailySummaryList);
            Assert.Equal(addExposureWindows, newExposureWindowList);
            mockPreferencesService.Verify(x => x.SetValue("DailySummaries", expectedDailySummariesJson), Times.Once);
            mockPreferencesService.Verify(x => x.SetValue("ExposureWindows", expectedExposureWindowsJson), Times.Once);

        }

        [Fact]
        public async void SetExposureDataTests_Append()
        {
            var existDailySummaries = new List<DailySummary>() {
                new DailySummary()
                {
                    DateMillisSinceEpoch = 0,
                    DaySummary = new ExposureSummaryData(),
                    ConfirmedClinicalDiagnosisSummary = new ExposureSummaryData(),
                    ConfirmedTestSummary = new ExposureSummaryData(),
                    RecursiveSummary = new ExposureSummaryData(),
                    SelfReportedSummary = new ExposureSummaryData()
                }
            };
            var existExposureWindows = new List<ExposureWindow>()
            {
                new ExposureWindow()
                {
                    CalibrationConfidence = CalibrationConfidence.High,
                    DateMillisSinceEpoch = 0,
                    Infectiousness = Infectiousness.High,
                    ReportType = ReportType.Unknown,
                    ScanInstances = new List<ScanInstance>()
                }
            };

            var addDailySummaries = new List<DailySummary>() {
                new DailySummary()
                {
                    DateMillisSinceEpoch = 10,
                    DaySummary = new ExposureSummaryData(),
                    ConfirmedClinicalDiagnosisSummary = new ExposureSummaryData(),
                    ConfirmedTestSummary = new ExposureSummaryData(),
                    RecursiveSummary = new ExposureSummaryData(),
                    SelfReportedSummary = new ExposureSummaryData()
                }
            };
            var addExposureWindows = new List<ExposureWindow>()
            {
                new ExposureWindow()
                {
                    CalibrationConfidence = CalibrationConfidence.High,
                    DateMillisSinceEpoch = 0,
                    Infectiousness = Infectiousness.High,
                    ReportType = ReportType.Unknown,
                    ScanInstances = new List<ScanInstance>()
                },
                new ExposureWindow()
                {
                    CalibrationConfidence = CalibrationConfidence.Medium,
                    DateMillisSinceEpoch = 0,
                    Infectiousness = Infectiousness.High,
                    ReportType = ReportType.ConfirmedTest,
                    ScanInstances = new List<ScanInstance>()
                }
            };

            var expectedDailySummaries = new List<DailySummary>() {
                existDailySummaries[0],
                addDailySummaries[0]
            };
            var expectedExposureWindows = new List<ExposureWindow>()
            {
                existExposureWindows[0],
                addExposureWindows[1]
            };

            var expectedNewDailySummaries = new List<DailySummary>() {
                addDailySummaries[0]
            };
            var expectedNewExposureWindows = new List<ExposureWindow>()
            {
                addExposureWindows[1]
            };

            // Mock Setup
            mockPreferencesService
                .Setup(x => x.GetValue(It.Is<string>(x => x == "DailySummaries"), It.IsAny<string>()))
                .Returns(JsonConvert.SerializeObject(existDailySummaries));
            mockPreferencesService
                .Setup(x => x.GetValue(It.Is<string>(x => x == "ExposureWindows"), It.IsAny<string>()))
                .Returns(JsonConvert.SerializeObject(existExposureWindows));

            var unitUnderTest = CreateRepository();

            var (newDailySummaryList, newExposureWindowList) = await unitUnderTest.SetExposureDataAsync(
                addDailySummaries,
                addExposureWindows
                );

            var expectedDailySummariesJson = JsonConvert.SerializeObject(expectedDailySummaries);
            var expectedExposureWindowsJson = JsonConvert.SerializeObject(expectedExposureWindows);

            // Assert
            Assert.Equal(expectedNewDailySummaries, newDailySummaryList);
            Assert.Equal(expectedNewExposureWindows, newExposureWindowList);
            mockPreferencesService.Verify(x => x.SetValue("DailySummaries", expectedDailySummariesJson), Times.Once);
            mockPreferencesService.Verify(x => x.SetValue("ExposureWindows", expectedExposureWindowsJson), Times.Once);

        }

        [Fact]
        public void GetExposureInformationListTests_Success()
        {
            var unitUnderTest = CreateRepository();

            mockPreferencesService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns("[{\"Timestamp\":\"2020-12-21T10:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":2,\"TotalRiskScore\":19,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2020-12-21T11:00:00\",\"Duration\":\"00:15:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":20,\"TransmissionRiskLevel\":5}]");

            var result = unitUnderTest.GetExposureInformationList();

            Assert.Equal(2, result.Count);
            Assert.Equal(new DateTime(2020, 12, 21, 10, 00, 00), result[0].Timestamp);
            Assert.Equal(new TimeSpan(0, 5, 0), result[0].Duration);
            Assert.Equal(2, result[0].AttenuationValue);
            Assert.Equal(19, result[0].TotalRiskScore);
            Assert.Equal(RiskLevel.Medium, result[0].TransmissionRiskLevel);
            Assert.Equal(new DateTime(2020, 12, 21, 11, 00, 00), result[1].Timestamp);
            Assert.Equal(new TimeSpan(0, 15, 0), result[1].Duration);
            Assert.Equal(3, result[1].AttenuationValue);
            Assert.Equal(20, result[1].TotalRiskScore);
            Assert.Equal(RiskLevel.MediumHigh, result[1].TransmissionRiskLevel);
        }

        [Fact]
        public void GetExposureInformationListTests_NoData()
        {
            var unitUnderTest = CreateRepository();

            mockPreferencesService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns((string)(object)null);

            var result = unitUnderTest.GetExposureInformationList();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void SetExposureInformationTests_Success()
        {
            var unitUnderTest = CreateRepository();

            var actualExposureInformaionJson = "";
            mockPreferencesService.Setup(x => x.SetValue("ExposureInformation", It.IsAny<string>())).Callback<string, string>((k, v) =>
            {
                actualExposureInformaionJson = v;
            });

            var testExposureInformation = new List<UserExposureInfo> {
                new UserExposureInfo(new DateTime(2020,12,21), new TimeSpan(0, 10, 0), 20, 30, RiskLevel.Lowest),
                new UserExposureInfo(new DateTime(2020,12,22), new TimeSpan(0, 20, 0), 30, 40, RiskLevel.Low)
            };

            unitUnderTest.SetExposureInformation(testExposureInformation);

            mockPreferencesService.Verify(x => x.SetValue("ExposureInformation", It.IsAny<string>()), Times.Once());

            Assert.NotEmpty(actualExposureInformaionJson);

            var regex = new Regex("^\\[({.+}),({.+})\\]$");
            var matches = regex.Matches(actualExposureInformaionJson);
            Assert.Single(matches);
            Assert.Equal(3, matches[0].Groups.Count);

            var actualInfo1 = matches[0].Groups[1].Value;
            Assert.Contains("\"Timestamp\":\"2020-12-21T00:00:00\"", actualInfo1);
            Assert.Contains("\"Duration\":\"00:10:00\"", actualInfo1);
            Assert.Contains("\"AttenuationValue\":20", actualInfo1);
            Assert.Contains("\"TotalRiskScore\":30", actualInfo1);
            Assert.Contains("\"TransmissionRiskLevel\":1", actualInfo1);

            var actualInfo2 = matches[0].Groups[2].Value;
            Assert.Contains("\"Timestamp\":\"2020-12-22T00:00:00\"", actualInfo2);
            Assert.Contains("\"Duration\":\"00:20:00\"", actualInfo2);
            Assert.Contains("\"AttenuationValue\":30", actualInfo2);
            Assert.Contains("\"TotalRiskScore\":40", actualInfo2);
            Assert.Contains("\"TransmissionRiskLevel\":2", actualInfo2);
        }

        [Fact]
        public void RemoveExposureInformationTests()
        {
            var unitUnderTest = CreateRepository();

            mockPreferencesService.Reset();
            mockLoggerService.Reset();

            unitUnderTest.RemoveExposureInformation();

            mockPreferencesService.Verify(x => x.RemoveValue("ExposureInformation"), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod("RemoveExposureInformation", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod("RemoveExposureInformation", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Theory]
        [InlineData(15, 4, 1)]
        [InlineData(16, 4, 1)]
        [InlineData(17, 3, 2)]
        [InlineData(18, 2, 3)]
        [InlineData(19, 1, 4)]
        [InlineData(20, 0, 0)]
        public void GetExposureInformationListToDisplayTests_Success(int day, int expectedCount, int expectedStartDay)
        {
            var unitUnderTest = CreateRepository();

            mockPreferencesService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns("[{\"Timestamp\":\"2021-01-01T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-02T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-03T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-04T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4}]");
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(new DateTime(2021, 1, day, 0, 0, 0));

            var result = unitUnderTest.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay);

            Assert.Equal(expectedCount, result.Count);
            for (int idx = 0; idx < expectedCount; idx++)
            {
                Assert.Equal(new DateTime(2021, 1, expectedStartDay + idx, 0, 0, 0), result[idx].Timestamp);
            }
        }

        [Fact]
        public void GetExposureInformationListToDisplayTests_NoData()
        {
            var unitUnderTest = CreateRepository();

            mockPreferencesService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns((string)(object)null);
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(new DateTime(2021, 1, 1, 0, 0, 0));

            var result = unitUnderTest.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(17, 9, 0, 3, 2)]
        [InlineData(17, 9, 1, 2, 3)]
        public void GetExposureInformationListToDisplayTests_Jst(int day, int hour, int minute, int expectedCount, int expectedStartDay)
        {
            var unitUnderTest = CreateRepository();

            mockPreferencesService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns("[{\"Timestamp\":\"2021-01-01T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-02T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-03T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-04T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4}]");
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(new DateTime(2021, 1, day, hour, minute, 0).AddHours(-9));

            var result = unitUnderTest.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay);

            Assert.Equal(expectedCount, result.Count);
            for (int idx = 0; idx < expectedCount; idx++)
            {
                Assert.Equal(new DateTime(2021, 1, expectedStartDay + idx, 0, 0, 0), result[idx].Timestamp);
            }
        }

        [Theory]
        [InlineData(15, 4)]
        [InlineData(16, 4)]
        [InlineData(17, 3)]
        [InlineData(18, 2)]
        [InlineData(19, 1)]
        [InlineData(20, 0)]
        public void GetV1ExposureCountToDisplayTests_Success(int day, int expectedCount)
        {
            var unitUnderTest = CreateRepository();

            mockPreferencesService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns("[{\"Timestamp\":\"2021-01-01T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-02T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-03T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-04T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4}]");
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(new DateTime(2021, 1, day, 0, 0, 0));

            var result = unitUnderTest.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay).Count();

            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public void GetV1ExposureCountToDisplayTests_NoData()
        {
            var unitUnderTest = CreateRepository();

            mockPreferencesService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns((string)(object)null);
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(new DateTime(2021, 1, 1, 0, 0, 0));

            var result = unitUnderTest.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay).Count();

            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData(5, 1, 0, 0)]
        [InlineData(5, 0, 0, 0)]
        [InlineData(5, -1, 1, 4)]
        [InlineData(5, -2, 2, 3)]
        [InlineData(5, -3, 3, 2)]
        [InlineData(5, -4, 4, 1)]
        [InlineData(5, -5, 4, 1)]
        public void RemoveOutOfDateExposureInformationTests(int day, int offsetDays, int expectedCount, int expectedStartDay)
        {
            var unitUnderTest = CreateRepository();

            mockPreferencesService.Setup(x => x.SetValue("ExposureInformation", It.IsAny<string>())).Callback<string, string>((k, v) =>
            {
                mockPreferencesService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns(v);
            });

            mockPreferencesService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns("[{\"Timestamp\":\"2021-01-01T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-02T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-03T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-04T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4}]");
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(new DateTime(2021, 1, day, 0, 0, 0));

            unitUnderTest.RemoveOutOfDateExposureInformation(offsetDays);

            mockPreferencesService.Verify(x => x.SetValue("ExposureInformation", It.IsAny<string>()), Times.Once());

            var result = unitUnderTest.GetExposureInformationList();

            Assert.Equal(expectedCount, result.Count);
            for (int idx = 0; idx < expectedCount; idx++)
            {
                Assert.Equal(new DateTime(2021, 1, expectedStartDay + idx, 0, 0, 0), result[idx].Timestamp);
            }
        }
    }
}
