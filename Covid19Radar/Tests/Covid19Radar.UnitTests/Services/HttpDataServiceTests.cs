/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Covid19Radar.Model;
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
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IHttpClientService> mockHttpClientService;
        private readonly Mock<ISecureStorageService> mockSecureStorageService;
        private readonly Mock<IApplicationPropertyService> mockApplicationPropertyService;

        public HttpDataServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockHttpClientService = mockRepository.Create<IHttpClientService>();
            mockSecureStorageService = mockRepository.Create<ISecureStorageService>();
            mockApplicationPropertyService = mockRepository.Create<IApplicationPropertyService>();
        }

        private IHttpDataService CreateService()
        {
            return new HttpDataService(
                mockLoggerService.Object,
                mockHttpClientService.Object,
                mockSecureStorageService.Object,
                mockApplicationPropertyService.Object);
        }

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

            mockSecureStorageService.Reset();

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

            mockSecureStorageService.Reset();

            var result = await unitUnderTest.PostRegisterUserAsync();

            mockLoggerService.Verify(x => x.StartMethod("PostRegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod("PostRegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());

            Assert.False(result);
        }

        [Fact]
        public async Task PostRegisterUserAsyncTestsAsync_Exception()
        {
            var mockHttpClient = new HttpClient(new MockHttpHandler((r, c) =>
            {
                var absoluteUri = r.RequestUri.AbsoluteUri;
                if (absoluteUri.EndsWith("/api/register"))
                {
                    throw new HttpRequestException("unit-test");
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }));

            mockHttpClientService.Setup(x => x.Create()).Returns(mockHttpClient);

            var unitUnderTest = CreateService();

            mockSecureStorageService.Reset();

            var result = await unitUnderTest.PostRegisterUserAsync();

            mockLoggerService.Verify(x => x.StartMethod("PostRegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod("PostRegisterUserAsync", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());

            Assert.False(result);
        }

        [Fact]
        public async Task PutSelfExposureKeysAsync_Success()
        {
            HttpContent requestContent = null;
            var mockHttpClient = new HttpClient(new MockHttpHandler((r, c) =>
            {
                var absoluteUri = r.RequestUri.AbsoluteUri;
                if (absoluteUri.EndsWith("/api/v2/diagnosis"))
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

            mockSecureStorageService.Reset();

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
    }
}
