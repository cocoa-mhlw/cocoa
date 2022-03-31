// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Covid19Radar.UnitTests.Repository
{
    public class ExposureRiskCalculationConfigurationRepositoryTests
    {

        private const string JSON_EXPOSURE_RISK_CONFIGURATION1 = "exposure_risk_configuration1.json";
        private const string JSON_EXPOSURE_RISK_CONFIGURATION2 = "exposure_risk_configuration2.json";

        private readonly string CURRENT_EXPOSURE_RISK_CONFIGURATION_FILE_PATH = "./dummy_for_test.json";

        private readonly MockRepository mockRepository;
        private readonly Mock<IHttpClientService> mockClientService;
        private readonly Mock<ILocalPathService> mockLocalPathService;
        private readonly Mock<IServerConfigurationRepository> mockServerConfigurationRepository;
        private readonly Mock<ILoggerService> mockLoggerService;

        public ExposureRiskCalculationConfigurationRepositoryTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockClientService = mockRepository.Create<IHttpClientService>();
            mockLocalPathService = mockRepository.Create<ILocalPathService>();
            mockServerConfigurationRepository = mockRepository.Create<IServerConfigurationRepository>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
        }

        private IExposureRiskCalculationConfigurationRepository CreateRepository()
            => new ExposureRiskCalculationConfigurationRepository(
                mockClientService.Object,
                mockLocalPathService.Object,
                mockServerConfigurationRepository.Object,
                mockLoggerService.Object
                );

        private static string GetTestJson(string fileName)
        {
            var path = TestDataUtils.GetLocalFilePath(fileName);
            using (var reader = File.OpenText(path))
            {
                return reader.ReadToEnd();
            }
        }

        [Fact]
        public async Task GetExposureRiskConfigurationTest_firsttime()
        {
            string content = GetTestJson(JSON_EXPOSURE_RISK_CONFIGURATION1);

            V1ExposureRiskCalculationConfiguration expect
                = JsonConvert.DeserializeObject<V1ExposureRiskCalculationConfiguration>(content);

            var jsonContent = new StringContent(
                content,
                Encoding.UTF8,
                "application/json"
            );
            var client = HttpClientUtils.CreateHttpClient(HttpStatusCode.OK, jsonContent);
            mockClientService.Setup(x => x.StaticJsonContentClient).Returns(client);

            mockLocalPathService.Setup(x => x.ExposureConfigurationDirPath).Returns("./");
            mockLocalPathService.Setup(x => x.CurrentExposureRiskCalculationConfigurationPath).Returns(CURRENT_EXPOSURE_RISK_CONFIGURATION_FILE_PATH);
            mockServerConfigurationRepository.Setup(x => x.ExposureRiskCalculationConfigurationUrl).Returns("https://example.com/exposure_risk_configuration.json");

            var unitUnderTest = CreateRepository();
            var result = await unitUnderTest.GetExposureRiskCalculationConfigurationAsync(preferCache: false);

            mockServerConfigurationRepository.Verify(s => s.LoadAsync(), Times.Once());

            Assert.NotNull(result);

            Assert.Equal(result, expect);
        }

        [Fact]
        public async Task GetExposureRiskConfigurationTest_preferCache()
        {
            string existConfigurationJson = GetTestJson(JSON_EXPOSURE_RISK_CONFIGURATION1);
            File.WriteAllText(CURRENT_EXPOSURE_RISK_CONFIGURATION_FILE_PATH, existConfigurationJson);

            V1ExposureRiskCalculationConfiguration expect
                = JsonConvert.DeserializeObject<V1ExposureRiskCalculationConfiguration>(existConfigurationJson);

            string newConfigurationJson = GetTestJson(JSON_EXPOSURE_RISK_CONFIGURATION2);

            var jsonContent = new StringContent(
                newConfigurationJson,
                Encoding.UTF8,
                "application/json"
            );
            var client = HttpClientUtils.CreateHttpClient(HttpStatusCode.OK, jsonContent);
            mockClientService.Setup(x => x.StaticJsonContentClient).Returns(client);

            mockLocalPathService.Setup(x => x.ExposureConfigurationDirPath).Returns("./");
            mockLocalPathService.Setup(x => x.CurrentExposureRiskCalculationConfigurationPath).Returns(CURRENT_EXPOSURE_RISK_CONFIGURATION_FILE_PATH);
            mockServerConfigurationRepository.Setup(x => x.ExposureRiskCalculationConfigurationUrl).Returns("https://example.com/exposure_risk_configuration.json");

            var unitUnderTest = CreateRepository();
            var result = await unitUnderTest.GetExposureRiskCalculationConfigurationAsync(preferCache: true);

            mockServerConfigurationRepository.Verify(s => s.LoadAsync(), Times.Never());

            Assert.NotNull(result);

            Assert.Equal(result, expect);
        }

        [Fact]
        public async Task GetExposureRiskConfigurationTest_updated()
        {
            string currentConfigurationJson = GetTestJson(JSON_EXPOSURE_RISK_CONFIGURATION1);
            File.WriteAllText(CURRENT_EXPOSURE_RISK_CONFIGURATION_FILE_PATH, currentConfigurationJson);
            V1ExposureRiskCalculationConfiguration currentConfiguration
                = JsonConvert.DeserializeObject<V1ExposureRiskCalculationConfiguration>(currentConfigurationJson);

            string newConfigurationJson = GetTestJson(JSON_EXPOSURE_RISK_CONFIGURATION2);
            V1ExposureRiskCalculationConfiguration newConfiguration
                = JsonConvert.DeserializeObject<V1ExposureRiskCalculationConfiguration>(newConfigurationJson);

            var jsonContent = new StringContent(
                newConfigurationJson,
                Encoding.UTF8,
                "application/json"
            );
            var client = HttpClientUtils.CreateHttpClient(HttpStatusCode.OK, jsonContent);
            mockClientService.Setup(x => x.StaticJsonContentClient).Returns(client);

            mockLocalPathService.Setup(x => x.ExposureConfigurationDirPath).Returns("./");
            mockLocalPathService.Setup(x => x.CurrentExposureRiskCalculationConfigurationPath).Returns(CURRENT_EXPOSURE_RISK_CONFIGURATION_FILE_PATH);
            mockServerConfigurationRepository.Setup(x => x.ExposureRiskCalculationConfigurationUrl).Returns("https://example.com/exposure_risk_configuration.json");

            var unitUnderTest = CreateRepository();
            var result = await unitUnderTest.GetExposureRiskCalculationConfigurationAsync(preferCache: false);

            mockServerConfigurationRepository.Verify(s => s.LoadAsync(), Times.Once());

            Assert.NotNull(result);

            Assert.NotEqual(result, currentConfiguration);
            Assert.Equal(result, newConfiguration);
        }

        [Fact]
        public async Task GetExposureRiskConfigurationTest_not_updated()
        {
            string currentConfigurationJson = GetTestJson(JSON_EXPOSURE_RISK_CONFIGURATION1);
            File.WriteAllText(CURRENT_EXPOSURE_RISK_CONFIGURATION_FILE_PATH, currentConfigurationJson);
            V1ExposureRiskCalculationConfiguration currentConfiguration
                = JsonConvert.DeserializeObject<V1ExposureRiskCalculationConfiguration>(currentConfigurationJson);

            string newConfigurationJson = GetTestJson(JSON_EXPOSURE_RISK_CONFIGURATION2);
            V1ExposureRiskCalculationConfiguration newConfiguration
                = JsonConvert.DeserializeObject<V1ExposureRiskCalculationConfiguration>(newConfigurationJson);

            var jsonContent = new StringContent(
                newConfigurationJson,
                Encoding.UTF8,
                "application/json"
            );
            var client = HttpClientUtils.CreateHttpClient(HttpStatusCode.OK, jsonContent);
            mockClientService.Setup(x => x.StaticJsonContentClient).Returns(client);

            mockLocalPathService.Setup(x => x.ExposureConfigurationDirPath).Returns("./");
            mockLocalPathService.Setup(x => x.CurrentExposureRiskCalculationConfigurationPath).Returns(CURRENT_EXPOSURE_RISK_CONFIGURATION_FILE_PATH);
            mockServerConfigurationRepository.Setup(x => x.ExposureRiskCalculationConfigurationUrl).Returns("https://example.com/exposure_risk_configuration.json");

            var unitUnderTest = CreateRepository();
            var result = await unitUnderTest.GetExposureRiskCalculationConfigurationAsync(preferCache: false);

            result = await unitUnderTest.GetExposureRiskCalculationConfigurationAsync(preferCache: false);

            mockServerConfigurationRepository.Verify(s => s.LoadAsync(), Times.Exactly(2));
            mockLoggerService.Verify(x => x.Info("ExposureRiskCalculationConfiguration have not been changed.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),Times.Once);

            Assert.NotNull(result);

            Assert.NotEqual(result, currentConfiguration);
            Assert.Equal(result, newConfiguration);
        }

    }
}
