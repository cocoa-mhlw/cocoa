/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Covid19Radar.Common;
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
        private readonly Mock<IPreferencesService> mockPreferencesService;
        private readonly Mock<IDateTimeUtility> mockDateTimeUtility;

        public ExposureNotificationServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockHttpClientService = mockRepository.Create<IHttpClientService>();
            mockPreferencesService = mockRepository.Create<IPreferencesService>();
            mockDateTimeUtility = mockRepository.Create<IDateTimeUtility>();
            DateTimeUtility.Instance = mockDateTimeUtility.Object;
        }

        private ExposureNotificationService CreateService()
        {
            return new ExposureNotificationService(
                mockLoggerService.Object,
                mockHttpClientService.Object,
                mockPreferencesService.Object
                );
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
    }
}
