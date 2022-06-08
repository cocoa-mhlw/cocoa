/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.UnitTests.Mocks;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class HttpDataServiceTests
    {
        #region Instance Properties

        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IHttpClientService> mockHttpClientService;

        private readonly ReleaseServerConfigurationRepository _serverConfigurationRepository;

        #endregion

        #region Constructors

        public HttpDataServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockHttpClientService = mockRepository.Create<IHttpClientService>();
            _serverConfigurationRepository = new ReleaseServerConfigurationRepository();
        }

        #endregion

        #region Other Private Methods

        private IHttpDataService CreateService()
        {
            return new HttpDataService(
                mockLoggerService.Object,
                mockHttpClientService.Object,
                _serverConfigurationRepository
                );
        }

        #endregion

        #region Test Methods

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

            mockHttpClientService.Setup(x => x.Create()).Returns(mockHttpClient);

            var unitUnderTest = CreateService();

            var result = await unitUnderTest.PostRegisterUserAsync();

            mockLoggerService.Verify(x => x.StartMethod("PostRegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod("PostRegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
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

            mockHttpClientService.Setup(x => x.Create()).Returns(mockHttpClient);

            var unitUnderTest = CreateService();

            var result = await unitUnderTest.PostRegisterUserAsync();

            mockLoggerService.Verify(x => x.StartMethod("PostRegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod("PostRegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
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

            mockHttpClientService.Setup(x => x.Create()).Returns(mockHttpClient);

            var unitUnderTest = CreateService();


            Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await unitUnderTest.PostRegisterUserAsync();
            });

            mockLoggerService.Verify(x => x.StartMethod("PostRegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod("PostRegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        #endregion

        #region PutSelfExposureKeysAsync()

        [Fact]
        public async Task PutSelfExposureKeysAsync_Success()
        {
            HttpContent requestContent = null;
            var mockHttpClient = new HttpClient(new MockHttpHandler((r, c) =>
            {
                var absoluteUri = r.RequestUri.AbsoluteUri;
                if (absoluteUri.EndsWith("/api/v3/diagnosis"))
                {
                    requestContent = r.Content;
                    var response = new HttpResponseMessage(HttpStatusCode.NoContent);
                    response.Content = new StringContent("");
                    return response;
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }));

            mockHttpClientService.Setup(x => x.Create()).Returns(mockHttpClient);

            var unitUnderTest = CreateService();

            var request = new DiagnosisSubmissionParameter()
            {
                Keys = new DiagnosisSubmissionParameter.Key[] {
                    new DiagnosisSubmissionParameter.Key(){
                        KeyData = "key-data",
                        RollingStartNumber = 1,
                        RollingPeriod = 2
                    }
                },
                Regions = new string[] { "440" },
                Platform = "platform",
                DeviceVerificationPayload = "device-verification-payload",
                AppPackageName = "app-package-name",
                VerificationPayload = "verification-payload",
                Padding = "padding"
            };

            var result = await unitUnderTest.PutSelfExposureKeysAsync(request);

            mockLoggerService.Verify(x => x.StartMethod("PutSelfExposureKeysAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod("PutSelfExposureKeysAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());

            Assert.Equal(HttpStatusCode.NoContent, result);
            Assert.NotNull(requestContent);

            var stringContent = await requestContent.ReadAsStringAsync();
            Assert.NotEmpty(stringContent);

            var jsonContent = JsonConvert.DeserializeObject(stringContent) as JObject;
            Assert.NotNull(jsonContent);

            var keys = jsonContent["keys"] as JArray;
            Assert.Single(keys);
            Assert.Equal("key-data", keys[0]["keyData"].Value<string>());
            Assert.Equal(1, keys[0]["rollingStartNumber"].Value<long>());
            Assert.Equal(2, keys[0]["rollingPeriod"].Value<long>());

            var rgions = jsonContent["regions"] as JArray;
            Assert.Equal("440", rgions[0].Value<string>());

            Assert.Equal("platform", jsonContent["platform"].Value<string>());
            Assert.Equal("device-verification-payload", jsonContent["deviceVerificationPayload"].Value<string>());
            Assert.Equal("app-package-name", jsonContent["appPackageName"].Value<string>());
            Assert.Equal("verification-payload", jsonContent["verificationPayload"].Value<string>());
            Assert.Equal("padding", jsonContent["padding"].Value<string>());

            Assert.Null(jsonContent["userUuid"]);
            Assert.Null(mockHttpClient.DefaultRequestHeaders.Authorization);
        }

        #endregion

        #region PutEventLogTests

        [Fact]
        public async Task PutEventLogTests_Success()
        {
            HttpContent requestContent = null;
            var mockHttpClient = new HttpClient(new MockHttpHandler((r, c) =>
            {
                var absoluteUri = r.RequestUri.AbsoluteUri;
                if (absoluteUri.EndsWith("/api/v1/event_log"))
                {
                    requestContent = r.Content;
                    var response = new HttpResponseMessage(HttpStatusCode.Created);
                    response.Content = new StringContent("");
                    return response;
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }));

            mockHttpClientService.Setup(x => x.Create()).Returns(mockHttpClient);

            var unitUnderTest = CreateService();

            var request = new V1EventLogRequest()
            {
                IdempotencyKey = "05A6A158-E216-4599-B99E-3708D360FF2F",
                Platform = "platform",
                AppPackageName = "app-package-name",
                DeviceVerificationPayload = "device-verification-payload",
                EventLogs = new List<EventLog>()
                {
                    new EventLog()
                    {
                        HasConsent = true,
                        Epoch = 1000,
                        Type = "type",
                        Subtype = "subtype",
                        Content = "content"
                    }
                }
            };

            var result = await unitUnderTest.PutEventLog(request);

            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            Assert.NotNull(requestContent);

            var stringContent = await requestContent.ReadAsStringAsync();
            Assert.NotEmpty(stringContent);

            var jsonContent = JsonConvert.DeserializeObject(stringContent) as JObject;
            Assert.NotNull(jsonContent);

            Assert.Equal("05A6A158-E216-4599-B99E-3708D360FF2F", jsonContent["idempotency_key"].Value<string>());
            Assert.Equal("platform", jsonContent["platform"].Value<string>());
            Assert.Equal("app-package-name", jsonContent["appPackageName"].Value<string>());
            Assert.Equal("device-verification-payload", jsonContent["deviceVerificationPayload"].Value<string>());

            var eventLogs = jsonContent["event_logs"] as JArray;
            Assert.Single(eventLogs);

            Assert.True(eventLogs[0]["has_consent"].Value<bool>());
            Assert.Equal(1000, eventLogs[0]["epoch"].Value<long>());
            Assert.Equal("type", eventLogs[0]["type"].Value<string>());
            Assert.Equal("subtype", eventLogs[0]["subtype"].Value<string>());
            Assert.Equal("content", eventLogs[0]["content"].Value<string>());

            Assert.Null(mockHttpClient.DefaultRequestHeaders.Authorization);
        }

        #endregion

        #endregion
    }
}
