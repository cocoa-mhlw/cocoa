// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Covid19Radar.UnitTests.Repository
{
    public class ExposureConfigurationRepositoryTests
    {

        private const string JSON_EXPOSURE_CONFIGURATION1 = "exposure_configuration1.json";
        private const string JSON_EXPOSURE_CONFIGURATION2 = "exposure_configuration2.json";
        private const string JSON_EXPOSURE_CONFIGURATION_MAPPING_UPDATED = "exposure_configuration_update_infectiousness_for_days_since_onset_of_symptoms.json";

        private readonly MockRepository mockRepository;
        private readonly Mock<IHttpClientService> mockClientService;
        private readonly Mock<ILocalPathService> mockLocalPathService;
        private readonly Mock<IPreferencesService> mockPreferencesService;
        private readonly Mock<IServerConfigurationRepository> mockServerConfigurationRepository;
        private readonly Mock<IDateTimeUtility> mockDateTimeUtility;
        private readonly Mock<ILoggerService> mockLoggerService;

        public ExposureConfigurationRepositoryTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockClientService = mockRepository.Create<IHttpClientService>();
            mockLocalPathService = mockRepository.Create<ILocalPathService>();
            mockPreferencesService = mockRepository.Create<IPreferencesService>();
            mockServerConfigurationRepository = mockRepository.Create<IServerConfigurationRepository>();
            mockDateTimeUtility = mockRepository.Create<IDateTimeUtility>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
        }

        private IExposureConfigurationRepository CreateRepository()
            => new ExposureConfigurationRepository(
                mockClientService.Object,
                mockLocalPathService.Object,
                mockPreferencesService.Object,
                mockServerConfigurationRepository.Object,
                mockDateTimeUtility.Object,
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

        private readonly string CURRENT_JSON_PATH = Path.Combine("./", "current.json");

        [Fact]
        public void GetExposureConfigurationTest()
        {
            mockLocalPathService.Setup(x => x.ExposureConfigurationDirPath).Returns("./");
            var unitUnderTest = CreateRepository();

            Assert.Equal(CURRENT_JSON_PATH, unitUnderTest.CurrentConfigFilePath);
        }

        private static DateTime Date
            => DateTime.SpecifyKind(new DateTime(2021, 11, 21), DateTimeKind.Utc);

        [Fact]
        public async Task GetExposureConfigurationTest_firsttime()
        {
            var date = Date;

            string content = GetTestJson(JSON_EXPOSURE_CONFIGURATION1);
            var jsonContent = new StringContent(
                content,
                Encoding.UTF8,
                "application/json"
            );
            var client = HttpClientUtils.CreateHttpClient(HttpStatusCode.OK, jsonContent);
            mockClientService.Setup(x => x.Create()).Returns(client);

            mockLocalPathService.Setup(x => x.ExposureConfigurationDirPath).Returns("./");
            mockServerConfigurationRepository.Setup(x => x.ExposureConfigurationUrl).Returns("https://example.com/exposure_configuration.json");
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(date);

            var unitUnderTest = CreateRepository();
            var result = await unitUnderTest.GetExposureConfigurationAsync();

            mockServerConfigurationRepository.Verify(s => s.LoadAsync(), Times.Once());
            mockPreferencesService.Verify(s => s.SetValue(PreferenceKey.ExposureConfigurationDownloadedEpoch, date.ToUnixEpoch()), Times.Once());
            mockPreferencesService.Verify(s => s.SetValue(PreferenceKey.IsExposureConfigurationUpdated, true), Times.Once());

            Assert.NotNull(result);

            Assert.NotNull(result.GoogleExposureConfig);
            Assert.NotNull(result.AppleExposureConfigV1);

            // Google ExposureWindow mode
            Assert.NotNull(result.GoogleDailySummariesConfig);
            Assert.NotNull(result.GoogleDiagnosisKeysDataMappingConfig);

            // Apple ENv2
            Assert.NotNull(result.AppleExposureConfigV2);
        }

        [Fact]
        public async Task GetExposureConfigurationTest_updated_but_not_cache_expired()
        {
            var date = Date;

            string testJson1 = GetTestJson(JSON_EXPOSURE_CONFIGURATION1);
            using (var writer = File.CreateText(CURRENT_JSON_PATH))
            {
                writer.WriteAsync(testJson1).GetAwaiter().GetResult();
            }
            ExposureConfiguration result1 = JsonConvert.DeserializeObject<ExposureConfiguration>(testJson1);

            string testJson2 = GetTestJson(JSON_EXPOSURE_CONFIGURATION2);
            var jsonContent = new StringContent(
                testJson2,
                Encoding.UTF8,
                "application/json"
            );
            var client = HttpClientUtils.CreateHttpClient(HttpStatusCode.OK, jsonContent);
            mockClientService.Setup(x => x.Create()).Returns(client);

            mockLocalPathService.Setup(x => x.ExposureConfigurationDirPath).Returns("./");
            mockServerConfigurationRepository.Setup(x => x.ExposureConfigurationUrl).Returns("https://example.com/exposure_configuration.json");
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(date);

            mockPreferencesService.Setup(x => x.GetValue(PreferenceKey.ExposureConfigurationDownloadedEpoch, It.IsAny<long>())).Returns(date.ToUnixEpoch());
            mockPreferencesService.Setup(x => x.GetValue(PreferenceKey.IsExposureConfigurationUpdated, false)).Returns(false);

            var unitUnderTest = CreateRepository();
            var result2 = await unitUnderTest.GetExposureConfigurationAsync();

            mockPreferencesService.Verify(s => s.SetValue(PreferenceKey.ExposureConfigurationDownloadedEpoch, date.ToUnixEpoch()), Times.Never());
            mockPreferencesService.Verify(s => s.SetValue(PreferenceKey.ExposureConfigurationAppliedEpoch, date.ToUnixEpoch()), Times.Never());
            mockPreferencesService.Verify(s => s.SetValue(PreferenceKey.IsExposureConfigurationUpdated, true), Times.Never());

            Assert.Equal(result1, result2);
        }

        [Fact]
        public async Task GetExposureConfigurationTest_updated_and_cache_expired()
        {
            var date = Date;
            var cacheExpireDate = date + TimeSpan.FromDays(2) + TimeSpan.FromSeconds(1);

            string testJson1 = GetTestJson(JSON_EXPOSURE_CONFIGURATION1);
            using (var writer = File.CreateText(CURRENT_JSON_PATH))
            {
                writer.WriteAsync(testJson1).GetAwaiter().GetResult();
            }
            ExposureConfiguration result1 = JsonConvert.DeserializeObject<ExposureConfiguration>(testJson1);

            string testJson2 = GetTestJson(JSON_EXPOSURE_CONFIGURATION2);
            ExposureConfiguration expectedResult = JsonConvert.DeserializeObject<ExposureConfiguration>(testJson2);

            var jsonContent = new StringContent(
                testJson2,
                Encoding.UTF8,
                "application/json"
            );
            var client = HttpClientUtils.CreateHttpClient(HttpStatusCode.OK, jsonContent);
            mockClientService.Setup(x => x.Create()).Returns(client);

            mockLocalPathService.Setup(x => x.ExposureConfigurationDirPath).Returns("./");
            mockServerConfigurationRepository.Setup(x => x.ExposureConfigurationUrl).Returns("https://example.com/exposure_configuration.json");
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(cacheExpireDate);

            mockPreferencesService.Setup(x => x.GetValue(PreferenceKey.ExposureConfigurationDownloadedEpoch, It.IsAny<long>())).Returns(date.ToUnixEpoch());
            mockPreferencesService.Setup(x => x.GetValue(PreferenceKey.IsExposureConfigurationUpdated, false)).Returns(false);

            var unitUnderTest = CreateRepository();
            var result2 = await unitUnderTest.GetExposureConfigurationAsync();

            mockPreferencesService.Verify(s => s.SetValue(PreferenceKey.ExposureConfigurationDownloadedEpoch, cacheExpireDate.ToUnixEpoch()), Times.Once());
            mockPreferencesService.Verify(s => s.SetValue(PreferenceKey.ExposureConfigurationAppliedEpoch, cacheExpireDate.ToUnixEpoch()), Times.Never());
            mockPreferencesService.Verify(s => s.SetValue(PreferenceKey.IsExposureConfigurationUpdated, true), Times.Once());

            Assert.NotEqual(result1, result2);
            Assert.Equal(expectedResult, result2);
        }

        [Fact]
        public async Task GetExposureConfigurationTest_updated_mapping_but_not_cache_expired()
        {
            var date = Date;
            var cacheExpireDate = date + TimeSpan.FromDays(8);

            string testJson1 = GetTestJson(JSON_EXPOSURE_CONFIGURATION1);
            using (var writer = File.CreateText(CURRENT_JSON_PATH))
            {
                writer.WriteAsync(testJson1).GetAwaiter().GetResult();
            }
            ExposureConfiguration result1 = JsonConvert.DeserializeObject<ExposureConfiguration>(testJson1);

            string testJson3 = GetTestJson(JSON_EXPOSURE_CONFIGURATION_MAPPING_UPDATED);
            var jsonContent = new StringContent(
                testJson3,
                Encoding.UTF8,
                "application/json"
            );
            var client = HttpClientUtils.CreateHttpClient(HttpStatusCode.OK, jsonContent);
            mockClientService.Setup(x => x.Create()).Returns(client);

            mockLocalPathService.Setup(x => x.ExposureConfigurationDirPath).Returns("./");
            mockServerConfigurationRepository.Setup(x => x.ExposureConfigurationUrl).Returns("https://example.com/exposure_configuration.json");
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(cacheExpireDate);

            mockPreferencesService.Setup(x => x.GetValue(PreferenceKey.ExposureConfigurationAppliedEpoch, It.IsAny<long>())).Returns(date.ToUnixEpoch());
            mockPreferencesService.Setup(x => x.GetValue(PreferenceKey.ExposureConfigurationDownloadedEpoch, It.IsAny<long>())).Returns(date.ToUnixEpoch());
            mockPreferencesService.Setup(x => x.GetValue(PreferenceKey.IsExposureConfigurationUpdated, false)).Returns(false);

            var unitUnderTest = CreateRepository();
            var result2 = await unitUnderTest.GetExposureConfigurationAsync();

            mockPreferencesService.Verify(s => s.SetValue(PreferenceKey.ExposureConfigurationDownloadedEpoch, cacheExpireDate.ToUnixEpoch()), Times.Once());
            mockPreferencesService.Verify(s => s.SetValue(PreferenceKey.ExposureConfigurationAppliedEpoch, date.ToUnixEpoch()), Times.Never());
            mockPreferencesService.Verify(s => s.SetValue(PreferenceKey.IsExposureConfigurationUpdated, true), Times.Never());

            Assert.Equal(result1, result2);
        }

        [Fact]
        public async Task GetExposureConfigurationTest_updated_mapping_and_cache_expired()
        {
            var date = Date;
            var cacheExpireDate = date + TimeSpan.FromDays(8) + TimeSpan.FromSeconds(1);

            string testJson1 = GetTestJson(JSON_EXPOSURE_CONFIGURATION1);
            using (var writer = File.CreateText(CURRENT_JSON_PATH))
            {
                writer.WriteAsync(testJson1).GetAwaiter().GetResult();
            }
            ExposureConfiguration result1 = JsonConvert.DeserializeObject<ExposureConfiguration>(testJson1);

            string testJson3 = GetTestJson(JSON_EXPOSURE_CONFIGURATION_MAPPING_UPDATED);
            ExposureConfiguration expectedResult = JsonConvert.DeserializeObject<ExposureConfiguration>(testJson3);

            var jsonContent = new StringContent(
                testJson3,
                Encoding.UTF8,
                "application/json"
            );
            var client = HttpClientUtils.CreateHttpClient(HttpStatusCode.OK, jsonContent);
            mockClientService.Setup(x => x.Create()).Returns(client);

            mockLocalPathService.Setup(x => x.ExposureConfigurationDirPath).Returns("./");
            mockServerConfigurationRepository.Setup(x => x.ExposureConfigurationUrl).Returns("https://example.com/exposure_configuration.json");
            mockDateTimeUtility.Setup(x => x.UtcNow).Returns(cacheExpireDate);

            mockPreferencesService.Setup(x => x.GetValue(PreferenceKey.ExposureConfigurationAppliedEpoch, It.IsAny<long>())).Returns(date.ToUnixEpoch());
            mockPreferencesService.Setup(x => x.GetValue(PreferenceKey.ExposureConfigurationDownloadedEpoch, It.IsAny<long>())).Returns(date.ToUnixEpoch());
            mockPreferencesService.Setup(x => x.GetValue(PreferenceKey.IsExposureConfigurationUpdated, false)).Returns(false);

            var unitUnderTest = CreateRepository();
            var result2 = await unitUnderTest.GetExposureConfigurationAsync();

            mockPreferencesService.Verify(s => s.SetValue(PreferenceKey.ExposureConfigurationDownloadedEpoch, cacheExpireDate.ToUnixEpoch()), Times.Once());
            mockPreferencesService.Verify(s => s.SetValue(PreferenceKey.ExposureConfigurationAppliedEpoch, cacheExpireDate.ToUnixEpoch()), Times.Never());
            mockPreferencesService.Verify(s => s.SetValue(PreferenceKey.IsExposureConfigurationUpdated, true), Times.Once());

            Assert.NotEqual(result1, result2);
            Assert.Equal(expectedResult, result2);
        }

    }
}
