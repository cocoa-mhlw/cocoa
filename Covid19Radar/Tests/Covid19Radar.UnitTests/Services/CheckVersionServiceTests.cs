// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class CheckVersionServiceTests
    {
        private const string JSON_VERSION1 = "check_version1.json";

        #region Instance Properties

        private readonly MockRepository _mockRepository;
        private readonly Mock<ILoggerService> _mockLoggerService;
        private readonly Mock<IHttpClientService> _mockClientService;
        private readonly Mock<IEssentialsService> _mockEssentialsService;

        #endregion

        #region Constructors

        public CheckVersionServiceTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockLoggerService = _mockRepository.Create<ILoggerService>();
            _mockClientService = _mockRepository.Create<IHttpClientService>();
            _mockEssentialsService = _mockRepository.Create<IEssentialsService>();
        }

        #endregion

        #region Other Private Methods

        private CheckVersionService CreateService()
        {
            return new CheckVersionService(
                _mockClientService.Object,
                _mockEssentialsService.Object,
                _mockLoggerService.Object
                );
        }

        private static string GetTestJson(string fileName)
        {
            var path = TestDataUtils.GetLocalFilePath(fileName);
            using (var reader = File.OpenText(path))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion

        [Theory]
        [InlineData(false, "2.0.1", true)]
        [InlineData(false, "2.0.2", false)]
        [InlineData(true, "2.0.1", true)]
        [InlineData(true, "2.0.2", true)]
        [InlineData(true, "2.0.3", false)]
        private async void IsUpdate(bool isIos, string version, bool expected)
        {
            string content = GetTestJson(JSON_VERSION1);

            var jsonContent = new StringContent(
                content,
                Encoding.UTF8,
                "application/json"
            );
            var client = HttpClientUtils.CreateHttpClient(HttpStatusCode.OK, jsonContent);
            _mockClientService.Setup(x => x.StaticJsonContentClient).Returns(client);

            _mockEssentialsService.Setup(x => x.IsIos).Returns(isIos);
            _mockEssentialsService.Setup(x => x.AppVersion).Returns(version);

            ICheckVersionService service = CreateService();

            bool isUpdated = await service.IsUpdateVersionExistAsync();

            Assert.Equal(expected, isUpdated);
        }
    }
}
