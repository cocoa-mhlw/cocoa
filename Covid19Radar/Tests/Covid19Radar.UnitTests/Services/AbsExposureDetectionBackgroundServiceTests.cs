// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Repository;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using Chino;
using System.Threading;

namespace Covid19Radar.UnitTests.Services
{
    public class AbsExposureDetectionBackgroundServiceTests
    {
        #region Instance Properties

        private readonly MockRepository mockRepository;
        private readonly Mock<IDiagnosisKeyRepository> diagnosisKeyRepository;
        private readonly ExposureNotificationApiServiceMock exposureNotificationApiService;
        private readonly Mock<IExposureConfigurationRepository> exposureConfigurationRepository;
        private readonly Mock<ILoggerService> loggerService;
        private readonly Mock<IUserDataRepository> userDataRepository;
        private readonly Mock<IServerConfigurationRepository> serverConfigurationRepository;


        #endregion

        #region Constructors

        public AbsExposureDetectionBackgroundServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            diagnosisKeyRepository = mockRepository.Create<IDiagnosisKeyRepository>();
            loggerService = mockRepository.Create<ILoggerService>();
            exposureNotificationApiService = new ExposureNotificationApiServiceMock(loggerService.Object);
            exposureConfigurationRepository = mockRepository.Create<IExposureConfigurationRepository>();
            userDataRepository = mockRepository.Create<IUserDataRepository>();
            serverConfigurationRepository = mockRepository.Create<IServerConfigurationRepository>();
        }

        #endregion

        #region Other Private Methods

        private AbsExposureDetectionBackgroundService CreateService()
        {

            return new ExposureDetectionBackgroundServiceMock(
                diagnosisKeyRepository.Object,
                exposureNotificationApiService,
                exposureConfigurationRepository.Object,
                loggerService.Object,
                userDataRepository.Object,
                serverConfigurationRepository.Object
                );
        }

        #endregion

        #region Test Methods

        #region ExposureDetectionAsync()

        [Fact]
        public async Task ExposureDetectionAsync_NoNewDiagnosisKeyFound()
        {
            var unitUnderTest = CreateService();

            serverConfigurationRepository.Setup(x => x.Regions).Returns(new string[] { "440" });
            serverConfigurationRepository.Setup(x => x.GetDiagnosisKeyListProvideServerUrl(It.IsAny<string>())).Returns("https://example.com");
            exposureConfigurationRepository.Setup(x => x.GetExposureConfigurationAsync()).Returns(Task.FromResult(new ExposureConfiguration()));
            exposureConfigurationRepository.Setup(x => x.GetExposureConfigurationAsync()).Returns(Task.FromResult(new ExposureConfiguration()));
            IList<DiagnosisKeyEntry> diagnosisKeyEntryList = new List<DiagnosisKeyEntry>();
            diagnosisKeyRepository.Setup(x => x.GetDiagnosisKeysListAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(diagnosisKeyEntryList));
            userDataRepository.Setup(x => x.GetLastProcessDiagnosisKeyTimestampAsync(It.IsAny<string>())).Returns(Task.FromResult(0L));

            await unitUnderTest.ExposureDetectionAsync();

            Assert.Equal<int>(0, exposureNotificationApiService.ProvideDiagnosisKeysAsync_CallCount);
        }

        public async Task ExposureDetectionAsync_NewDiagnosisKeyFound()
        {
            var unitUnderTest = CreateService();

            serverConfigurationRepository.Setup(x => x.Regions).Returns(new string[] { "440" });
            serverConfigurationRepository.Setup(x => x.GetDiagnosisKeyListProvideServerUrl(It.IsAny<string>())).Returns("https://example.com");
            exposureConfigurationRepository.Setup(x => x.GetExposureConfigurationAsync()).Returns(Task.FromResult(new ExposureConfiguration()));
            exposureConfigurationRepository.Setup(x => x.GetExposureConfigurationAsync()).Returns(Task.FromResult(new ExposureConfiguration()));
            IList<DiagnosisKeyEntry> diagnosisKeyEntryList = new List<DiagnosisKeyEntry>() {
                new DiagnosisKeyEntry
                {
                    Region = 440,
                    Url = "https://example.com",
                    Created = 0
                }
            };
            diagnosisKeyRepository
                .Setup(x => x.GetDiagnosisKeysListAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(diagnosisKeyEntryList));
            userDataRepository
                .Setup(x => x.GetLastProcessDiagnosisKeyTimestampAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(0L));
            diagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(It.IsAny<DiagnosisKeyEntry>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult("filePath"));

            await unitUnderTest.ExposureDetectionAsync();

            Assert.Single(exposureNotificationApiService.ProvideDiagnosisKeysAsync_Args.keyFiles);
        }
    #endregion

    #endregion
    }

    #region Test Target (Mock)
    class ExposureDetectionBackgroundServiceMock : AbsExposureDetectionBackgroundService
    {
        public ExposureDetectionBackgroundServiceMock(
            IDiagnosisKeyRepository diagnosisKeyRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            IExposureConfigurationRepository exposureConfigurationRepository,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            IServerConfigurationRepository serverConfigurationRepository
        ) : base(
            diagnosisKeyRepository,
            exposureNotificationApiService,
            exposureConfigurationRepository,
            loggerService,
            userDataRepository,
            serverConfigurationRepository
        )
        {

        }


        public override void Schedule()
        {
            throw new NotImplementedException();
        }
    }

    class ExposureNotificationApiServiceMock : AbsExposureNotificationApiService
    {
        public ExposureNotificationApiServiceMock(
            ILoggerService loggerService
        ) : base(loggerService)
        {

        }

        public override Task<IList<ExposureNotificationStatus>> GetStatusesAsync()
        {
            IList<ExposureNotificationStatus> statusList  = new List<ExposureNotificationStatus>();
            return Task.FromResult(statusList);
        }

        public override Task<List<TemporaryExposureKey>> GetTemporaryExposureKeyHistoryAsync()
        {
            return Task.FromResult(new List<TemporaryExposureKey>());
        }

        public override Task<long> GetVersionAsync()
        {
            return Task.FromResult(0L);
        }

        public override Task<bool> IsEnabledAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(List<string> keyFiles, CancellationTokenSource cancellationTokenSource = null)
        {
            throw new NotImplementedException();
        }

        public int ProvideDiagnosisKeysAsync_CallCount = 0;
        public (List<string> keyFiles, ExposureConfiguration configuration, CancellationTokenSource cancellationTokenSource) ProvideDiagnosisKeysAsync_Args;
        public override Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(List<string> keyFiles, ExposureConfiguration configuration, CancellationTokenSource cancellationTokenSource = null)
        {
            ProvideDiagnosisKeysAsync_CallCount += 1;
            return Task.FromResult(ProvideDiagnosisKeysResult.NoDiagnosisKeyFound);
        }

        public override Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(List<string> keyFiles, ExposureConfiguration configuration, string token, CancellationTokenSource cancellationTokenSource = null)
        {
            throw new NotImplementedException();
        }

        public override Task RequestPreAuthorizedTemporaryExposureKeyHistoryAsync()
        {
            throw new NotImplementedException();
        }

        public override Task RequestPreAuthorizedTemporaryExposureKeyReleaseAsync()
        {
            throw new NotImplementedException();
        }

        public override Task StartAsync()
        {
            throw new NotImplementedException();
        }

        public override Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
