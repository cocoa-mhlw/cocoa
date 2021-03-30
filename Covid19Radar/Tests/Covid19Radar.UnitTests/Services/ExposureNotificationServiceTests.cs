/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.UnitTests.Mocks;
using Moq;
using Xamarin.ExposureNotifications;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class ExposureNotificationServiceTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IHttpClientService> mockHttpClientService;
        private readonly Mock<ISecureStorageService> mockSecureStorageService;
        private readonly Mock<IPreferencesService> mockPreferencesService;
        private readonly Mock<IApplicationPropertyService> mockApplicationPropertyService;

        public ExposureNotificationServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockHttpClientService = mockRepository.Create<IHttpClientService>();
            mockSecureStorageService = mockRepository.Create<ISecureStorageService>();
            mockPreferencesService = mockRepository.Create<IPreferencesService>();
            mockApplicationPropertyService = mockRepository.Create<IApplicationPropertyService>();
        }

        private ExposureNotificationService CreateService()
        {
            return new ExposureNotificationService(
                mockLoggerService.Object,
                mockHttpClientService.Object,
                mockSecureStorageService.Object,
                mockPreferencesService.Object,
                mockApplicationPropertyService.Object);
        }

        [Theory]
        [InlineData("2020-11-22T00:00:00+09:00", 0, "", "")]
        [InlineData("2020-11-21T00:00:00+09:00", 1, "2020-11-18T00:00:00Z", "2020-11-18T00:00:00Z")]
        [InlineData("2020-11-20T00:00:00+09:00", 2, "2020-11-18T00:00:00Z", "2020-11-17T00:00:00Z")]
        [InlineData("2020-11-19T00:00:00+09:00", 3, "2020-11-18T00:00:00Z", "2020-11-16T00:00:00Z")]
        [InlineData("2020-11-18T00:00:00+09:00", 4, "2020-11-18T00:00:00Z", "2020-11-15T00:00:00Z")]
        [InlineData("2020-11-17T00:00:00+09:00", 6, "2020-11-18T00:00:00Z", "2020-11-13T15:00:00Z")]
        [InlineData("2020-11-16T00:00:00+09:00", 8, "2020-11-18T00:00:00Z", "2020-11-13T00:00:00Z")]
        [InlineData("2020-11-15T00:00:00+09:00", 9, "2020-11-18T00:00:00Z", "2020-11-12T00:00:00Z")]
        [InlineData("2020-11-14T00:00:00+09:00", 9, "2020-11-18T00:00:00Z", "2020-11-12T00:00:00Z")]
        public void FliterTemporaryExposureKeysTests_Success(string diagnosisDateTest, int expectedCount, string expectedFirst, string expedtedLast)
        {
            mockHttpClientService.Setup(x => x.Create()).Returns(new HttpClient(new MockHttpHandler((request, cancellationToken) =>
            {
                // Make an error not to process code that cannot be Mocked
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            })));

            var unitUnderTest = CreateService();

            var temporaryExposureKeys = new List<TemporaryExposureKey>() {
                new TemporaryExposureKey { RollingStart = DateTimeOffset.Parse("2020-11-18T00:00:00Z") },
                new TemporaryExposureKey { RollingStart = DateTimeOffset.Parse("2020-11-17T00:00:00Z") },
                new TemporaryExposureKey { RollingStart = DateTimeOffset.Parse("2020-11-16T00:00:00Z") },
                new TemporaryExposureKey { RollingStart = DateTimeOffset.Parse("2020-11-15T00:00:00Z") },
                new TemporaryExposureKey { RollingStart = DateTimeOffset.Parse("2020-11-14T00:00:00Z") },
                new TemporaryExposureKey { RollingStart = DateTimeOffset.Parse("2020-11-13T15:00:00Z") },
                new TemporaryExposureKey { RollingStart = DateTimeOffset.Parse("2020-11-13T14:59:59Z") },
                new TemporaryExposureKey { RollingStart = DateTimeOffset.Parse("2020-11-13T00:00:00Z") },
                new TemporaryExposureKey { RollingStart = DateTimeOffset.Parse("2020-11-12T00:00:00Z") },
            };

            unitUnderTest.DiagnosisDate = DateTime.Parse(diagnosisDateTest);
            var filteredTemporaryExposureKeys = unitUnderTest.FliterTemporaryExposureKeys(temporaryExposureKeys);

            var actualCount = filteredTemporaryExposureKeys.Count();
            Assert.Equal(expectedCount, actualCount);
            if (actualCount > 0)
            {
                Assert.Equal(DateTimeOffset.Parse(expectedFirst), filteredTemporaryExposureKeys.First().RollingStart);
                Assert.Equal(DateTimeOffset.Parse(expedtedLast), filteredTemporaryExposureKeys.Last().RollingStart);
            }
        }

        [Fact]
        public void FliterTemporaryExposureKeysTests_Failure()
        {
            mockHttpClientService.Setup(x => x.Create()).Returns(new HttpClient(new MockHttpHandler((request, cancellationToken) =>
            {
                // Make an error not to process code that cannot be Mocked
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            })));

            var unitUnderTest = CreateService();

            var temporaryExposureKeys = new List<TemporaryExposureKey>() {
                new TemporaryExposureKey { RollingStart = DateTimeOffset.Parse("2020-11-18T00:00:00Z") },
            };

            Assert.Throws<InvalidOperationException>(() =>
            {
                _ = unitUnderTest.FliterTemporaryExposureKeys(temporaryExposureKeys);
            });
        }

        [Fact]
        public void GetExposureInformationListTests_Success()
        {
            mockHttpClientService.Setup(x => x.Create()).Returns(new HttpClient(new MockHttpHandler((request, cancellationToken) =>
            {
                // Make an error not to process code that cannot be Mocked
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            })));

            var unitUnderTest = CreateService();

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
            mockHttpClientService.Setup(x => x.Create()).Returns(new HttpClient(new MockHttpHandler((request, cancellationToken) =>
            {
                // Make an error not to process code that cannot be Mocked
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            })));

            var unitUnderTest = CreateService();

            mockSecureStorageService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns((string)(object)null);

            var result = unitUnderTest.GetExposureInformationList();

            Assert.Null(result);
        }

        [Fact]
        public void GetExposureCountTests_Success()
        {
            mockHttpClientService.Setup(x => x.Create()).Returns(new HttpClient(new MockHttpHandler((request, cancellationToken) =>
            {
                // Make an error not to process code that cannot be Mocked
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            })));

            var unitUnderTest = CreateService();

            mockSecureStorageService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns("[{\"Timestamp\":\"2020-12-21T10:00:00\",\"Duration\":\"00:05:00.000\",\"AttenuationValue\":2,\"TotalRiskScore\":19,\"TransmissionRiskLevel\":4},{\"Timestamp\":\"2020-12-21T11:00:00\",\"Duration\":\"00:15:00.000\",\"AttenuationValue\":3,\"TotalRiskScore\":20,\"TransmissionRiskLevel\":5}]");

            var result = unitUnderTest.GetExposureCount();

            Assert.Equal(2, result);
        }

        [Fact]
        public void GetExposureCountTests_NoData()
        {
            mockHttpClientService.Setup(x => x.Create()).Returns(new HttpClient(new MockHttpHandler((request, cancellationToken) =>
            {
                // Make an error not to process code that cannot be Mocked
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            })));

            var unitUnderTest = CreateService();

            mockSecureStorageService.Setup(x => x.GetValue<string>("ExposureInformation", default)).Returns((string)(object)null);

            var result = unitUnderTest.GetExposureCount();

            Assert.Equal(0, result);
        }

        [Fact]
        public void SetExposureInformationTests_Success()
        {
            mockHttpClientService.Setup(x => x.Create()).Returns(new HttpClient(new MockHttpHandler((request, cancellationToken) =>
            {
                // Make an error not to process code that cannot be Mocked
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            })));

            var unitUnderTest = CreateService();

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
            mockHttpClientService.Setup(x => x.Create()).Returns(new HttpClient(new MockHttpHandler((request, cancellationToken) =>
            {
                // Make an error not to process code that cannot be Mocked
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            })));

            var unitUnderTest = CreateService();

            mockSecureStorageService.Reset();
            mockLoggerService.Reset();

            unitUnderTest.RemoveExposureInformation();

            mockSecureStorageService.Verify(x => x.RemoveValue("ExposureSummary"), Times.Once());
            mockSecureStorageService.Verify(x => x.RemoveValue("ExposureInformation"), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod("RemoveExposureInformation", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod("RemoveExposureInformation", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }
    }
}
