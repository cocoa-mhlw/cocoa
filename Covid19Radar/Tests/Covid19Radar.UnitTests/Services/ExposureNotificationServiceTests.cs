using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xamarin.ExposureNotifications;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class ExposureNotificationServiceTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IUserDataService> mockUserDataService;
        private readonly Mock<IHttpDataService> mockHttpDataService;
        private readonly Mock<IHttpClientService> mockHttpClientService;

        public ExposureNotificationServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockUserDataService = mockRepository.Create<IUserDataService>();
            mockHttpDataService = mockRepository.Create<IHttpDataService>();
            mockHttpClientService = mockRepository.Create<IHttpClientService>();
        }

        private ExposureNotificationService CreateService()
        {
            return new ExposureNotificationService(
                null,
                mockLoggerService.Object,
                mockUserDataService.Object,
                mockHttpDataService.Object,
                mockHttpClientService.Object);
        }

        private class MockHttpHandler : DelegatingHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler;

            public MockHttpHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler)
            {
                this.handler = handler;
            }
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(handler.Invoke(request, cancellationToken));
            }
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
            mockUserDataService.Setup(x => x.Get()).Returns(new UserDataModel { });
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
            mockUserDataService.Setup(x => x.Get()).Returns(new UserDataModel { });
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
