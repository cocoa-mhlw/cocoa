/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Common;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Repository
{
    public class UserDataRepositoryTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IPreferencesService> mockPreferencesService;
        private readonly Mock<ISecureStorageService> mockSecureStorageService;
        private readonly Mock<IDateTimeUtility> mockDateTimeUtility;

        public UserDataRepositoryTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockPreferencesService = mockRepository.Create<IPreferencesService>();
            mockSecureStorageService = mockRepository.Create<ISecureStorageService>();
            mockDateTimeUtility = mockRepository.Create<IDateTimeUtility>();
            DateTimeUtility.Instance = mockDateTimeUtility.Object;
        }

        private IUserDataRepository CreateRepository()
            => new UserDataRepository(
                mockPreferencesService.Object,
                mockSecureStorageService.Object,
                mockLoggerService.Object
                );

        #region LastConfirmedUtcDateTime

        [Fact]
        public void LastConfirmedUtcDateTimeTest_NotExists()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.LastConfirmedDateTimeEpoch)).Returns(false);

            var userDataRepository = CreateRepository();

            var lastConfirmedUtcDateTime = userDataRepository.GetLastConfirmedDate();

            Assert.Null(lastConfirmedUtcDateTime);
        }

        [Fact]
        public void LastConfirmedUtcDateTimeTest_Exists()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.LastConfirmedDateTimeEpoch)).Returns(true);
            mockPreferencesService.Setup(s => s.GetValue(PreferenceKey.LastConfirmedDateTimeEpoch, 0L)).Returns(800);

            var userDataRepository = CreateRepository();

            var lastConfirmedUtcDateTime = userDataRepository.GetLastConfirmedDate();

            var expoectedDateTime = DateTimeOffset.FromUnixTimeSeconds(800).DateTime;

            Assert.NotNull(lastConfirmedUtcDateTime);
            Assert.Equal(expoectedDateTime, lastConfirmedUtcDateTime);
        }

        #endregion

        #region RemoveAllExposureNotificationStatus()

        [Fact]
        public void RemoveAllExposureNotificationStatusTest()
        {
            var userDataRepository = CreateRepository();

            userDataRepository.RemoveAllExposureNotificationStatus();

            mockPreferencesService.Verify(s => s.RemoveValue(PreferenceKey.LastConfirmedDateTimeEpoch), Times.Once());
            mockPreferencesService.Verify(s => s.RemoveValue(PreferenceKey.CanConfirmExposure), Times.Once());
        }

        #endregion

        [Fact]
        public void GetExposureInformationListTests_Success()
        {
            var unitUnderTest = CreateRepository();

            mockSecureStorageService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns("[{\"Timestamp\":\"2020-12-21T10:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":2,\"TotalRiskScore\":19,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2020-12-21T11:00:00\",\"Duration\":\"00:15:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":20,\"TransmissionRiskLevel\":5}]");

            var result = unitUnderTest.GetExposureInformationList();

            Assert.Equal(2, result.Count);
            Assert.Equal(new DateTime(2020, 12, 21, 10, 00, 00), result[0].Timestamp);
            Assert.Equal(new TimeSpan(0, 5, 0), result[0].Duration);
            Assert.Equal(2, result[0].AttenuationValue);
            Assert.Equal(19, result[0].TotalRiskScore);
            Assert.Equal(UserRiskLevel.Medium, result[0].TransmissionRiskLevel);
            Assert.Equal(new DateTime(2020, 12, 21, 11, 00, 00), result[1].Timestamp);
            Assert.Equal(new TimeSpan(0, 15, 0), result[1].Duration);
            Assert.Equal(3, result[1].AttenuationValue);
            Assert.Equal(20, result[1].TotalRiskScore);
            Assert.Equal(UserRiskLevel.MediumHigh, result[1].TransmissionRiskLevel);
        }

        [Fact]
        public void GetExposureInformationListTests_NoData()
        {
            var unitUnderTest = CreateRepository();

            mockSecureStorageService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns((string)(object)null);

            var result = unitUnderTest.GetExposureInformationList();

            Assert.Null(result);
        }

        [Fact]
        public void SetExposureInformationTests_Success()
        {
            var unitUnderTest = CreateRepository();

            var actualExposureSummaryJson = "";
            mockSecureStorageService.Setup(x => x.SetValue("ExposureSummary", It.IsAny<string>())).Callback<string, string>((k, v) =>
            {
                actualExposureSummaryJson = v;
            });

            var actualExposureInformaionJson = "";
            mockSecureStorageService.Setup(x => x.SetValue("ExposureInformation", It.IsAny<string>())).Callback<string, string>((k, v) =>
            {
                actualExposureInformaionJson = v;
            });

            var testExposureSummary = new UserExposureSummary(1, 2, 30, new[] { new TimeSpan(0, 5, 0), new TimeSpan(0, 10, 0) }, 40);
            var testExposureInformation = new List<UserExposureInfo> {
                new UserExposureInfo(new DateTime(2020,12,21), new TimeSpan(0, 10, 0), 20, 30, UserRiskLevel.Lowest),
                new UserExposureInfo(new DateTime(2020,12,22), new TimeSpan(0, 20, 0), 30, 40, UserRiskLevel.Low)
            };

            unitUnderTest.SetExposureInformation(testExposureSummary, testExposureInformation);

            mockSecureStorageService.Verify(x => x.SetValue("ExposureSummary", It.IsAny<string>()), Times.Once());
            mockSecureStorageService.Verify(x => x.SetValue("ExposureInformation", It.IsAny<string>()), Times.Once());

            Assert.NotEmpty(actualExposureSummaryJson);
            Assert.Contains("\"DaysSinceLastExposure\":1", actualExposureSummaryJson);
            Assert.Contains("\"MatchedKeyCount\":2", actualExposureSummaryJson);
            Assert.Contains("\"HighestRiskScore\":30", actualExposureSummaryJson);
            Assert.Contains("\"SummationRiskScore\":40", actualExposureSummaryJson);
            Assert.Contains("\"AttenuationDurations\":[\"00:05:00\",\"00:10:00\"]", actualExposureSummaryJson);

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

            mockSecureStorageService.Reset();
            mockLoggerService.Reset();

            unitUnderTest.RemoveExposureInformation();

            mockSecureStorageService.Verify(x => x.RemoveValue("ExposureSummary"), Times.Once());
            mockSecureStorageService.Verify(x => x.RemoveValue("ExposureInformation"), Times.Once());

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

            mockSecureStorageService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns("[{\"Timestamp\":\"2021-01-01T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-02T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-03T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-04T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4}]");
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

            mockSecureStorageService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns((string)(object)null);
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(new DateTime(2021, 1, 1, 0, 0, 0));

            var result = unitUnderTest.GetExposureInformationList(AppConstants.DaysOfExposureInformationToDisplay);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(17, 9, 0, 3, 2)]
        [InlineData(17, 9, 1, 2, 3)]
        public void GetExposureInformationListToDisplayTests_Jst(int day, int hour, int minute, int expectedCount, int expectedStartDay)
        {
            var unitUnderTest = CreateRepository();

            mockSecureStorageService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns("[{\"Timestamp\":\"2021-01-01T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-02T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-03T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-04T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4}]");
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
        public void GetExposureCountToDisplayTests_Success(int day, int expectedCount)
        {
            var unitUnderTest = CreateRepository();

            mockSecureStorageService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns("[{\"Timestamp\":\"2021-01-01T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-02T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-03T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-04T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4}]");
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(new DateTime(2021, 1, day, 0, 0, 0));

            var result = unitUnderTest.GetExposureCount(AppConstants.DaysOfExposureInformationToDisplay);

            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public void GetExposureCountToDisplayTests_NoData()
        {
            var unitUnderTest = CreateRepository();

            mockSecureStorageService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns((string)(object)null);
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(new DateTime(2021, 1, 1, 0, 0, 0));

            var result = unitUnderTest.GetExposureCount(AppConstants.DaysOfExposureInformationToDisplay);

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

            mockSecureStorageService.Setup(x => x.SetValue("ExposureInformation", It.IsAny<string>())).Callback<string, string>((k, v) =>
            {
                mockSecureStorageService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns(v);
            });

            mockSecureStorageService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns("[{\"Timestamp\":\"2021-01-01T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-02T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-03T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2021-01-04T00:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":21,\"TransmissionRiskLevel\":4}]");
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(new DateTime(2021, 1, day, 0, 0, 0));

            unitUnderTest.RemoveOutOfDateExposureInformation(offsetDays);

            mockSecureStorageService.Verify(x => x.SetValue("ExposureSummary", It.IsAny<string>()), Times.Once());
            mockSecureStorageService.Verify(x => x.SetValue("ExposureInformation", It.IsAny<string>()), Times.Once());

            var result = unitUnderTest.GetExposureInformationList();

            Assert.Equal(expectedCount, result.Count);
            for (int idx = 0; idx < expectedCount; idx++)
            {
                Assert.Equal(new DateTime(2021, 1, expectedStartDay + idx, 0, 0, 0), result[idx].Timestamp);
            }
        }
    }
}
