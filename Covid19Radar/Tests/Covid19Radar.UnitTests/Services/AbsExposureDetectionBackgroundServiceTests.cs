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
using System.Linq;
using System.IO;

namespace Covid19Radar.UnitTests.Services
{
    public class AbsExposureDetectionBackgroundServiceTests
    {
        #region Instance Properties

        private readonly MockRepository mockRepository;
        private readonly Mock<IDiagnosisKeyRepository> diagnosisKeyRepository;
        //private readonly ExposureNotificationApiServiceMock exposureNotificationApiService;
        private readonly Mock<AbsExposureNotificationApiService> exposureNotificationApiService;
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
            exposureNotificationApiService = new Mock<AbsExposureNotificationApiService>(loggerService.Object);//new ExposureNotificationApiServiceMock(loggerService.Object);
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
                exposureNotificationApiService.Object,
                exposureConfigurationRepository.Object,
                loggerService.Object,
                userDataRepository.Object,
                serverConfigurationRepository.Object
                );
        }

        private IList<DiagnosisKeyEntry> CreateMultipleDiagnosisKeyEntryList(int region)
        {
            return new List<DiagnosisKeyEntry>() {
                new DiagnosisKeyEntry
                {
                    Region = region,
                    Url = "https://example.com/1.zip",
                    Created = 1638543600 // 2021/12/04 00:00:00
                },
                new DiagnosisKeyEntry
                {
                    Region = region,
                    Url = "https://example.com/2.zip",
                    Created = 1638630000 // 2021/12/05 00:00:00
                },
                new DiagnosisKeyEntry
                {
                    Region = region,
                    Url = "https://example.com/3.zip",
                    Created = 1638457200 // 2021/12/03 00:00:00
                }
            };
        }

        #endregion

        #region Test Methods

        #region ExposureDetectionAsync()

        [Fact]
        public async Task ExposureDetectionAsync_NoNewDiagnosisKeyFound()
        {
            // Test Data
            IList<DiagnosisKeyEntry> diagnosisKeyEntryList = new List<DiagnosisKeyEntry>();

            // Mock Setup
            serverConfigurationRepository
                .Setup(x => x.Regions)
                .Returns(new string[] { "440" });
            serverConfigurationRepository
                .Setup(x => x.GetDiagnosisKeyListProvideServerUrl(It.IsAny<string>()))
                .Returns("https://example.com");
            exposureConfigurationRepository
                .Setup(x => x.GetExposureConfigurationAsync())
                .Returns(Task.FromResult(new ExposureConfiguration()));
            diagnosisKeyRepository
                .Setup(x => x.GetDiagnosisKeysListAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(diagnosisKeyEntryList));
            userDataRepository
                .Setup(x => x.GetLastProcessDiagnosisKeyTimestampAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(0L));


            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.ExposureDetectionAsync();


            // Assert
            exposureNotificationApiService
                .Verify(x => x.ProvideDiagnosisKeysAsync(
                                It.IsAny<List<string>>(),
                                It.IsAny<ExposureConfiguration>(),
                                It.IsAny<CancellationTokenSource>()), Times.Never);
        }

        [Fact]
        public async Task ExposureDetectionAsync_NewDiagnosisKeyFound()
        {
            // Test Data
            IList<DiagnosisKeyEntry> diagnosisKeyEntryList = CreateMultipleDiagnosisKeyEntryList(440);

            ExposureConfiguration exposureConfiguration = new ExposureConfiguration();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            // Create Tmp Files
            var tempPath = Path.GetTempPath();

            // Mock Setup
            diagnosisKeyRepository
                .Setup(x => x.GetDiagnosisKeysListAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(diagnosisKeyEntryList));
            diagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(
                    It.Is<DiagnosisKeyEntry>(s => s.Url == "https://example.com/1.zip"),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult($"{tempPath}/diagnosis_keys/440/1"));
            diagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(
                    It.Is<DiagnosisKeyEntry>(s => s.Url == "https://example.com/2.zip"),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult($"{tempPath}/diagnosis_keys/440/2"));
         
            diagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(
                     It.Is<DiagnosisKeyEntry>(s => s.Url == "https://example.com/3.zip"),
                     It.IsAny<string>(),
                     It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult($"{tempPath}/diagnosis_keys/440/3"));

            serverConfigurationRepository
                .Setup(x => x.Regions)
                .Returns(new string[] { "440" });
            serverConfigurationRepository
                .Setup(x => x.GetDiagnosisKeyListProvideServerUrl(It.IsAny<string>()))
                .Returns("https://example.com");

            exposureConfigurationRepository
                .Setup(x => x.GetExposureConfigurationAsync())
                .Returns(Task.FromResult(exposureConfiguration));

            userDataRepository
                .Setup(x => x.GetLastProcessDiagnosisKeyTimestampAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(0L));


            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.ExposureDetectionAsync(cancellationTokenSource);


            // Assert
            exposureNotificationApiService
                .Verify(x => x.ProvideDiagnosisKeysAsync(
                                It.Is<List<string>>( s => s.SequenceEqual(new List<string>() { $"{tempPath}/diagnosis_keys/440/1", $"{tempPath}/diagnosis_keys/440/2", $"{tempPath}/diagnosis_keys/440/3" })),
                                It.Is<ExposureConfiguration>(s => s.Equals(exposureConfiguration)),
                                It.Is<CancellationTokenSource>(s => s.Equals(cancellationTokenSource))), Times.Once);
            userDataRepository
                .Verify(x => x.SetLastProcessDiagnosisKeyTimestampAsync("440", 1638630000), Times.Once);
        }

        [Fact]
        public async Task ExposureDetectionAsync_MultiRegion()
        {
            // Test Data
            IList<DiagnosisKeyEntry> diagnosisKeyEntryList440 = CreateMultipleDiagnosisKeyEntryList(440);
            IList<DiagnosisKeyEntry> diagnosisKeyEntryList540 = CreateMultipleDiagnosisKeyEntryList(540);

            ExposureConfiguration exposureConfiguration = new ExposureConfiguration();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            // Mock Setup
            diagnosisKeyRepository
                .Setup(x => x.GetDiagnosisKeysListAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(diagnosisKeyEntryList440));
            diagnosisKeyRepository
                .Setup(x => x.GetDiagnosisKeysListAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(diagnosisKeyEntryList540));
            diagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(
                    It.IsAny<DiagnosisKeyEntry>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult("DLFile"));

            serverConfigurationRepository
                .Setup(x => x.Regions)
                .Returns(new string[] { "440", "540" });
            serverConfigurationRepository
                .Setup(x => x.GetDiagnosisKeyListProvideServerUrl(It.IsAny<string>()))
                .Returns("https://example.com");

            exposureConfigurationRepository
                .Setup(x => x.GetExposureConfigurationAsync())
                .Returns(Task.FromResult(exposureConfiguration));

            userDataRepository
                .Setup(x => x.GetLastProcessDiagnosisKeyTimestampAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(0L));


            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.ExposureDetectionAsync(cancellationTokenSource);


            // Assert
            serverConfigurationRepository.Verify(x => x.GetDiagnosisKeyListProvideServerUrl("440"), Times.Once);
            serverConfigurationRepository.Verify(x => x.GetDiagnosisKeyListProvideServerUrl("540"), Times.Once);

            userDataRepository.Verify(x => x.GetLastProcessDiagnosisKeyTimestampAsync("440"), Times.Once);
            userDataRepository.Verify(x => x.GetLastProcessDiagnosisKeyTimestampAsync("540"), Times.Once);

            exposureNotificationApiService
                .Verify(x => x.ProvideDiagnosisKeysAsync(
                                It.Is<List<string>>(s => s.SequenceEqual(new List<string>() { "DLFile", "DLFile", "DLFile" })),
                                It.Is<ExposureConfiguration>(s => s.Equals(exposureConfiguration)),
                                It.Is<CancellationTokenSource>(s => s.Equals(cancellationTokenSource))), Times.Exactly(2));

            userDataRepository
                .Verify(x => x.SetLastProcessDiagnosisKeyTimestampAsync("440", 1638630000), Times.Once);
            userDataRepository
                .Verify(x => x.SetLastProcessDiagnosisKeyTimestampAsync("540", 1638630000), Times.Once);
        }

        [Fact]
        public async Task ExposureDetectionAsync_FileRemoved()
        {

        }

        [Fact]
        public async Task ExposureDetectionAsync_ExceptionFileRemoved()
        {

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
    #endregion
}
