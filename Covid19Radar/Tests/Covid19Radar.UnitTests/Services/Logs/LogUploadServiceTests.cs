/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.IO;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services.Logs
{
    public class LogUploadServiceTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<ILogPathService> mockLogPathService;
        private readonly Mock<IStorageService> mockStorageService;

        public LogUploadServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockLogPathService = mockRepository.Create<ILogPathService>();
            mockStorageService = mockRepository.Create<IStorageService>();
        }

        private LogUploadService CreateService()
        {
            var s = new LogUploadService(
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

            mockLogPathService.Setup(x => x.LogUploadingTmpPath).Returns(testTmpPath);
            mockStorageService.Setup(x => x.UploadAsync("https://LOG_STORAGE_URL_BASE/", "LOG_STORAGE_CONTAINER_NAME", "LOG_STORAGE_ACCOUNT_NAME", testSasToken, Path.Combine(testTmpPath, testZipFileName))).ReturnsAsync(true);

            var result = await unitUnderTest.UploadAsync(testZipFileName, testSasToken);

            Assert.True(result);
            mockStorageService.Verify(x => x.UploadAsync("https://LOG_STORAGE_URL_BASE/", "LOG_STORAGE_CONTAINER_NAME", "LOG_STORAGE_ACCOUNT_NAME", testSasToken, Path.Combine(testTmpPath, testZipFileName)), Times.Once());
        }

        [Fact]
        public async void UploadAsyncTests_UploadAsyncFailure()
        {
            var unitUnderTest = CreateService();

            var testZipFileName = "zip-file.zip";
            var testTmpPath = Path.Combine("log", "tmp", "path");
            var testSasToken = "test-sas-token";

            mockLogPathService.Setup(x => x.LogUploadingTmpPath).Returns(testTmpPath);
            mockStorageService.Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

            var result = await unitUnderTest.UploadAsync(testZipFileName, testSasToken);

            Assert.False(result);
            mockStorageService.Verify(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async void UploadAsyncTests_UnexpectedError()
        {
            var unitUnderTest = CreateService();

            var testZipFileName = "zip-file.zip";
            var testTmpPath = Path.Combine("log", "tmp", "path");
            var testSasToken = "test-sas-token";

            mockStorageService.Setup(x => x.UploadAsync("https://LOG_STORAGE_URL_BASE/", "LOG_STORAGE_CONTAINER_NAME", "LOG_STORAGE_ACCOUNT_NAME", testSasToken, Path.Combine(testTmpPath, testZipFileName))).ReturnsAsync(false);

            var result = await unitUnderTest.UploadAsync(testZipFileName, testSasToken);

            Assert.False(result);
        }
    }
}
