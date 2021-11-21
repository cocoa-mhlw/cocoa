// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.UnitTests.Mocks;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Repository
{
    public class ExposureConfigurationRepositoryTests
    {

        private const string JSON_EXPOSURE_CONFIGURATION = "exposure_configuration1.json";

        private readonly MockRepository mockRepository;
        private readonly Mock<IHttpClientService> mockClientService;
        private readonly Mock<ILocalPathService> mockLocalPathService;
        private readonly Mock<IPreferencesService> mockPreferencesService;
        private readonly Mock<IServerConfigurationRepository> mockServerConfigurationRepository;
        private readonly Mock<ILoggerService> mockLoggerService;

        private readonly Mock<IDateTimeUtility> mockDateTimeUtility;

        public ExposureConfigurationRepositoryTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockClientService = mockRepository.Create<IHttpClientService>();
            mockLocalPathService = mockRepository.Create<ILocalPathService>();
            mockPreferencesService = mockRepository.Create<IPreferencesService>();
            mockServerConfigurationRepository = mockRepository.Create<IServerConfigurationRepository>();
            mockLoggerService = mockRepository.Create<ILoggerService>();

            mockDateTimeUtility = mockRepository.Create<IDateTimeUtility>();
            DateTimeUtility.Instance = mockDateTimeUtility.Object;
        }

        private IExposureConfigurationRepository CreateRepository()
            => new ExposureConfigurationRepository(
                mockClientService.Object,
                mockLocalPathService.Object,
                mockPreferencesService.Object,
                mockServerConfigurationRepository.Object,
                mockLoggerService.Object
                );

        private HttpClient SetupMockHttpClient(string responseJson)
        {
            var jsonContent = new StringContent(
                        responseJson,
                        Encoding.UTF8,
                        "application/json"
                    );
            return new HttpClient(new MockHttpHandler((r, c) =>
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = jsonContent,
                };
            }));
        }

        [Fact]
        public void GetExposureConfigurationTest()
        {
            var path = TestDataUtils.GetLocalFilePath(JSON_EXPOSURE_CONFIGURATION);
            
            string content;
            using (var reader = File.OpenText(path))
            {
                content = reader.ReadToEnd();
            }

            var client = SetupMockHttpClient(content);
            mockClientService.Setup(x => x.Create()).Returns(client);

            mockLocalPathService.Setup(x => x.ExposureConfigurationDirPath).Returns("./");
            mockServerConfigurationRepository.Setup(x => x.ExposureConfigurationUrl).Returns("https://example.com/exposure_configuration.json");

            var unitUnderTest = CreateRepository();
            var result = unitUnderTest.GetExposureConfigurationAsync().GetAwaiter().GetResult();

            mockServerConfigurationRepository.Verify(s => s.LoadAsync(), Times.Once());

            Assert.NotNull(result.GoogleExposureConfig);
            Assert.NotNull(result.AppleExposureConfigV1);

            // Google ExposureWindow mode
            Assert.NotNull(result.GoogleDailySummariesConfig);
            Assert.NotNull(result.GoogleDiagnosisKeysDataMappingConfig);

            // Apple ENv2
            Assert.NotNull(result.AppleExposureConfigV2);
        }
    }
}
