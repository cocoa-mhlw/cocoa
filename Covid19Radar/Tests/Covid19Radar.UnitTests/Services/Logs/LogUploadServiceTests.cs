using System.IO;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services.Logs
{
    public class LogUploadServiceTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<IHttpDataService> mockHttpDataService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<ILogPathService> mockLogPathService;
        private readonly Mock<IStorageService> mockStorageService;

        public LogUploadServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockHttpDataService = mockRepository.Create<IHttpDataService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockLogPathService = mockRepository.Create<ILogPathService>();
            mockStorageService = mockRepository.Create<IStorageService>();
        }

        private LogUploadService CreateService()
        {
            var s = new LogUploadService(
                mockHttpDataService.Object,
                mockLoggerService.Object,
                mockLogPathService.Object,
                mockStorageService.Object);
            return s;
        }

        [Fact]
        public async void UploadAsyncTests_Success()
        {
            var unitUnderTest = CreateService();

            var testZipFileName = "zip-file.zip";
            var testTmpPath = Path.Combine("log", "tmp", "path");
            var testSasToken = "test-sas-token";

            mockHttpDataService.Setup(x => x.GetLogStorageSas()).ReturnsAsync(new ApiResponse<LogStorageSas>(200, new LogStorageSas { SasToken = testSasToken }));

            mockLogPathService.Setup(x => x.LogUploadingTmpPath).Returns(testTmpPath);
            mockStorageService.Setup(x => x.UploadAsync("https://LOG_STORAGE_URL_BASE/", "LOG_STORAGE_CONTAINER_NAME", "LOG_STORAGE_ACCOUNT_NAME", testSasToken, Path.Combine(testTmpPath, testZipFileName))).ReturnsAsync(true);

            var result = await unitUnderTest.UploadAsync(testZipFileName);

            Assert.True(result);
            mockHttpDataService.Verify(x => x.GetLogStorageSas(), Times.Once());
            mockStorageService.Verify(x => x.UploadAsync("https://LOG_STORAGE_URL_BASE/", "LOG_STORAGE_CONTAINER_NAME", "LOG_STORAGE_ACCOUNT_NAME", testSasToken, Path.Combine(testTmpPath, testZipFileName)), Times.Once());
        }

        [Theory]
        [InlineData(500, "sas-token")]
        [InlineData(200, "")]
        [InlineData(200, null)]
        public async void UploadAsyncTests_GetLogStorageSasFailure(int status, string sasToken)
        {
            var unitUnderTest = CreateService();

            var testZipFileName = "zip-file.zip";

            mockHttpDataService.Setup(x => x.GetLogStorageSas()).ReturnsAsync(new ApiResponse<LogStorageSas>(status, new LogStorageSas { SasToken = sasToken }));

            var result = await unitUnderTest.UploadAsync(testZipFileName);

            Assert.False(result);
            mockHttpDataService.Verify(x => x.GetLogStorageSas(), Times.Once());
            mockStorageService.Verify(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async void UploadAsyncTests_UploadAsyncFailure()
        {
            var unitUnderTest = CreateService();

            var testZipFileName = "zip-file.zip";
            var testTmpPath = Path.Combine("log", "tmp", "path");
            var testSasToken = "test-sas-token";

            mockHttpDataService.Setup(x => x.GetLogStorageSas()).ReturnsAsync(new ApiResponse<LogStorageSas>(200, new LogStorageSas { SasToken = testSasToken }));

            mockLogPathService.Setup(x => x.LogUploadingTmpPath).Returns(testTmpPath);
            mockStorageService.Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

            var result = await unitUnderTest.UploadAsync(testZipFileName);

            Assert.False(result);
            mockHttpDataService.Verify(x => x.GetLogStorageSas(), Times.Once());
            mockStorageService.Verify(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async void UploadAsyncTests_UnexpectedError()
        {
            var unitUnderTest = CreateService();

            var testZipFileName = "zip-file.zip";
            var testTmpPath = Path.Combine("log", "tmp", "path");
            var testSasToken = "test-sas-token";

            mockHttpDataService.Setup(x => x.GetLogStorageSas()).ReturnsAsync(new ApiResponse<LogStorageSas>(200, new LogStorageSas { SasToken = testSasToken }));
            mockStorageService.Setup(x => x.UploadAsync("https://LOG_STORAGE_URL_BASE/", "LOG_STORAGE_CONTAINER_NAME", "LOG_STORAGE_ACCOUNT_NAME", testSasToken, Path.Combine(testTmpPath, testZipFileName))).ReturnsAsync(false);

            var result = await unitUnderTest.UploadAsync(testZipFileName);

            Assert.False(result);
        }
    }
}
