/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.IO;
using System.Net;
using Covid19Radar.Repository;
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
        private readonly Mock<IHttpDataService> mockHttpDataService;
        private readonly Mock<IServerConfigurationRepository> mockServerConfigurationRepository;

        public LogUploadServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockLogPathService = mockRepository.Create<ILogPathService>();
            mockStorageService = mockRepository.Create<IStorageService>();
            mockHttpDataService = mockRepository.Create<IHttpDataService>();
            mockServerConfigurationRepository = mockRepository.Create<IServerConfigurationRepository>();
        }

        private LogUploadService CreateService()
        {
            var s = new LogUploadService(
                mockLoggerService.Object,
                mockLogPathService.Object,
                mockStorageService.Object,
                mockHttpDataService.Object,
                mockServerConfigurationRepository.Object
                );
            return s;
        }

        [Fact]
        public async void UploadAsyncTests_Success()
        {
            var unitUnderTest = CreateService();

            var testZipFileName = "zip-file.zip";
            var testTmpPath = Path.Combine("log", "tmp", "path");
            var testSasToken = "test-sas-token";
            var testZipFilePath = Path.Combine(testTmpPath, testZipFileName);

            mockLogPathService.Setup(x => x.LogUploadingTmpPath).Returns(testTmpPath);
            mockStorageService.Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), testSasToken, testZipFilePath))
                .ReturnsAsync(HttpStatusCode.Created);

            var result = await unitUnderTest.UploadAsync(testZipFilePath, testSasToken);

            Assert.Equal(HttpStatusCode.Created, result);
            mockStorageService.Verify(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), testSasToken, testZipFilePath), Times.Once());
        }

        [Fact]
        public async void UploadAsyncTests_UploadAsyncFailure()
        {
            var unitUnderTest = CreateService();

            var testZipFileName = "zip-file.zip";
            var testTmpPath = Path.Combine("log", "tmp", "path");
            var testSasToken = "test-sas-token";
            var testZipFilePath = Path.Combine(testTmpPath, testZipFileName);

            mockLogPathService.Setup(x => x.LogUploadingTmpPath).Returns(testTmpPath);
            mockStorageService.Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(HttpStatusCode.Forbidden);

            var result = await unitUnderTest.UploadAsync(testZipFilePath, testSasToken);

            Assert.NotEqual(HttpStatusCode.Created, result);
            mockStorageService.Verify(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async void UploadAsyncTests_UnexpectedError()
        {
            var unitUnderTest = CreateService();

            var testZipFileName = "zip-file.zip";
            var testTmpPath = Path.Combine("log", "tmp", "path");
            var testSasToken = "test-sas-token";
            var testZipFilePath = Path.Combine(testTmpPath, testZipFileName);

            mockStorageService.Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), testSasToken, testZipFilePath))
                .ReturnsAsync(HttpStatusCode.Forbidden);

            var result = await unitUnderTest.UploadAsync(testZipFilePath, testSasToken);

            Assert.NotEqual(HttpStatusCode.Created, result);
        }
    }
}
