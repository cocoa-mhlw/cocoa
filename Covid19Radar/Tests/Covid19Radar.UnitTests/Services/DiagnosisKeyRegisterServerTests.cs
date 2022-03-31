// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

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
    public class DiagnosisKeyRegisterServerTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IHttpDataService> mockHttpDataService;
        private readonly Mock<IDeviceVerifier> mockDeviceVerifier;
        private readonly Mock<IServerConfigurationRepository> mockServerConfigurationRepository;

        public DiagnosisKeyRegisterServerTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockHttpDataService = mockRepository.Create<IHttpDataService>();
            mockDeviceVerifier = mockRepository.Create<IDeviceVerifier>();
            mockServerConfigurationRepository = mockRepository.Create<IServerConfigurationRepository>();
        }

        private IDiagnosisKeyRegisterServer CreateService()
        {
            return new DiagnosisKeyRegisterServer(
                mockLoggerService.Object,
                mockHttpDataService.Object,
                mockDeviceVerifier.Object,
                mockServerConfigurationRepository.Object
                );
        }

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

            mockHttpDataService.Setup(x => x.HttpClient).Returns(mockHttpClient);
            mockServerConfigurationRepository.Setup(x => x.DiagnosisKeyRegisterApiUrls)
                .Returns(new List<string>() { IServerConfigurationRepository.CombineAsUrl(AppSettings.Instance.ApiUrlBase, "api/v3/diagnosis") });

            var unitUnderTest = CreateService();

            var temporaryExposureKeys = new DiagnosisSubmissionParameter.Key[] {
                    new DiagnosisSubmissionParameter.Key(){
                        KeyData = "key-data",
                        RollingStartNumber = 1,
                        RollingPeriod = 2
                    }
                };

            var result = await unitUnderTest.PutSelfExposureKeysAsync(
                new DiagnosisSubmissionParameter()
                {
                    Regions = new string[] { "440" },
                    Keys = temporaryExposureKeys,
                    Platform = "platform",
                    DeviceVerificationPayload = "device-verification-payload",
                    AppPackageName = "app-package-name",
                    VerificationPayload = "verification-payload",
                    Padding = "padding"

                }
                );

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
    }
}
