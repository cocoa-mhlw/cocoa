// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System.Net.Http;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;
using System.Net;
using System.Text;
using System.IO;

namespace Covid19Radar.UnitTests.Repository
{
    public class DiagnosisKeyRepositoryTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IHttpClientService> mockClientService;

        public DiagnosisKeyRepositoryTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockClientService = mockRepository.Create<IHttpClientService>();
        }

        private IDiagnosisKeyRepository CreateRepository()
            => new DiagnosisKeyRepository(
                mockClientService.Object,
                mockLoggerService.Object
            );

        [Fact]
        public async void GetDiagnosisKeysListAsyncTests_Success()
        {
            var client = HttpClientUtils.CreateHttpClient(
                HttpStatusCode.OK,
                new StringContent(
                        "[" +
                        "  {region: 1, url:\"https://example.com/1\", created: 12345678}, " +
                        "  {region: 2, url:\"https://example.com/2\", created: 87654321}" +
                        "]",
                        Encoding.UTF8,
                        "application/json"
                    )
            );
            mockClientService.Setup(x => x.Create()).Returns(client);

            var unitUnderTest = CreateRepository();
            var (_, result) = await unitUnderTest.GetDiagnosisKeysListAsync("https://example.com", default);

            Assert.Equal(2, result.Count);

            Assert.Equal(1, result[0].Region);
            Assert.Equal("https://example.com/1", result[0].Url);
            Assert.Equal(12345678, result[0].Created);

            Assert.Equal(2, result[1].Region);
            Assert.Equal("https://example.com/2", result[1].Url);
            Assert.Equal(87654321, result[1].Created);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async void GetDiagnosisKeysListAsyncTests_HttpError(System.Net.HttpStatusCode statusCode)
        {
            var client = HttpClientUtils.CreateHttpClient(
                statusCode,
                new StringContent("", Encoding.UTF8, "application/json")
            );
            mockClientService.Setup(x => x.Create()).Returns(client);

            var unitUnderTest = CreateRepository();
            var (_, result) = await unitUnderTest.GetDiagnosisKeysListAsync("https://example.com", default);

            Assert.Equal(0, result.Count);
        }

        [Fact]
        public async void DownloadDiagnosisKeysAsyncTests_Success()
        {
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(""));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(@"application/zip");

            var client = HttpClientUtils.CreateHttpClient(
                HttpStatusCode.OK,
                content
            );
            mockClientService.Setup(x => x.Create()).Returns(client);

            var entry = new DiagnosisKeyEntry()
            {
                Region = 1,
                Url = "https://example.com/1.zip",
                Created = 12345678
            };

            var tmpPath = Path.GetTempPath();
            var unitUnderTest = CreateRepository();
            var result = await unitUnderTest.DownloadDiagnosisKeysAsync(entry, tmpPath, default);
            
            Assert.Equal(Path.Combine(tmpPath, "1.zip"), result);

            if (File.Exists(result))
            {
                File.Delete(result);
            }
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async void DownloadDiagnosisKeysAsyncTests_HttpError(System.Net.HttpStatusCode statusCode)
        {
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(""));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(@"application/zip");

            var client = HttpClientUtils.CreateHttpClient(
                statusCode,
                content
            );
            mockClientService.Setup(x => x.Create()).Returns(client);

            var entry = new DiagnosisKeyEntry()
            {
                Region = 1,
                Url = "https://example.com/1.zip",
                Created = 12345678
            };

            var tmpPath = Path.GetTempPath();

            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                var unitUnderTest = CreateRepository();
                string result = await unitUnderTest.DownloadDiagnosisKeysAsync(entry, tmpPath, default);
            });

            var outputPath = Path.Combine(tmpPath, "1.zip");
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }
}