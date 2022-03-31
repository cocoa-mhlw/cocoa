// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.UnitTests.Mocks;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class UserDataServiceTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IHttpDataService> mockHttpDataService;
        private readonly Mock<IUserDataRepository> mockUserDataRepository;
        private readonly Mock<IServerConfigurationRepository> mockServerConfigurationRepository;

        public UserDataServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockHttpDataService = mockRepository.Create<IHttpDataService>();
            mockUserDataRepository = mockRepository.Create<IUserDataRepository>();
            mockServerConfigurationRepository = mockRepository.Create<IServerConfigurationRepository>();
        }

        private UserDataService CreateService()
        {
            return new UserDataService(
                mockHttpDataService.Object,
                mockLoggerService.Object,
                mockUserDataRepository.Object,
                mockServerConfigurationRepository.Object
                );
        }

        #region PostRegisterUserAsync()

        [Fact]
        public async Task PostRegisterUserAsyncTestsAsync_Success()
        {
            var mockHttpClient = new HttpClient(new MockHttpHandler((r, c) =>
            {
                var absoluteUri = r.RequestUri.AbsoluteUri;
                if (absoluteUri.EndsWith("/api/register"))
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent("{}");
                    response.Content.Headers.Remove("Content-Type");
                    response.Content.Headers.Add("Content-Type", "application/json");
                    return response;
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }));

            mockHttpDataService.Setup(x => x.HttpClient).Returns(mockHttpClient);
            mockServerConfigurationRepository.Setup(x => x.UserRegisterApiEndpoint)
                .Returns(IServerConfigurationRepository.CombineAsUrl(AppSettings.Instance.ApiUrlBase, "api/register"));

            var unitUnderTest = CreateService();

            var result = await unitUnderTest.RegisterUserAsync();

            mockLoggerService.Verify(x => x.StartMethod("RegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod("RegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());

            Assert.Null(mockHttpClient.DefaultRequestHeaders.Authorization);
        }

        [Fact]
        public async Task PostRegisterUserAsyncTestsAsync_Failure()
        {
            var mockHttpClient = new HttpClient(new MockHttpHandler((r, c) =>
            {
                var absoluteUri = r.RequestUri.AbsoluteUri;
                if (absoluteUri.EndsWith("/api/register"))
                {
                    var response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                    return response;
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }));

            mockHttpDataService.Setup(x => x.HttpClient).Returns(mockHttpClient);
            mockServerConfigurationRepository.Setup(x => x.UserRegisterApiEndpoint)
                .Returns(IServerConfigurationRepository.CombineAsUrl(AppSettings.Instance.ApiUrlBase, "api/register"));

            var unitUnderTest = CreateService();

            var result = await unitUnderTest.RegisterUserAsync();

            mockLoggerService.Verify(x => x.StartMethod("RegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod("RegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());

            Assert.Equal(HttpStatusCode.ServiceUnavailable, result);
        }

        [Fact]
        public void PostRegisterUserAsyncTestsAsync_Exception()
        {
            var exception = new HttpRequestException("unit-test");
            var mockHttpClient = new HttpClient(new MockHttpHandler((r, c) =>
            {
                var absoluteUri = r.RequestUri.AbsoluteUri;
                if (absoluteUri.EndsWith("/api/register"))
                {
                    throw exception;
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }));

            mockHttpDataService.Setup(x => x.HttpClient).Returns(mockHttpClient);
            mockServerConfigurationRepository.Setup(x => x.UserRegisterApiEndpoint)
                .Returns(IServerConfigurationRepository.CombineAsUrl(AppSettings.Instance.ApiUrlBase, "api/register"));

            var unitUnderTest = CreateService();


            Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await unitUnderTest.RegisterUserAsync();
            });

            mockLoggerService.Verify(x => x.StartMethod("RegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod("RegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        #endregion

    }
}
