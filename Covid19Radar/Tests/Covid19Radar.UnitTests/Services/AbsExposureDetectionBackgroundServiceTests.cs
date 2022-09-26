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
using Covid19Radar.Common;
using System.Net;

namespace Covid19Radar.UnitTests.Services
{
    public class AbsExposureDetectionBackgroundServiceTests: IDisposable
    {
        #region Instance Properties

        private readonly MockRepository mockRepository;
        private readonly Mock<IDiagnosisKeyRepository> mockDiagnosisKeyRepository;
        private readonly Mock<AbsExposureNotificationApiService> mockExposureNotificationApiService;
        private readonly Mock<IExposureConfigurationRepository> mockExposureConfigurationRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IUserDataRepository> mockUserDataRepository;
        private readonly Mock<IServerConfigurationRepository> mockServerConfigurationRepository;
        private readonly Mock<ILocalPathService> mockLocalPathService;
        private readonly Mock<IDateTimeUtility> mockDateTimeUtility;
        private readonly Mock<ILocalNotificationService> mockLocalNotificationService;

        #endregion

        #region Constructors

        public AbsExposureDetectionBackgroundServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockDiagnosisKeyRepository = mockRepository.Create<IDiagnosisKeyRepository>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockExposureNotificationApiService = new Mock<AbsExposureNotificationApiService>(mockLoggerService.Object);
            mockExposureConfigurationRepository = mockRepository.Create<IExposureConfigurationRepository>();
            mockUserDataRepository = mockRepository.Create<IUserDataRepository>();
            mockServerConfigurationRepository = mockRepository.Create<IServerConfigurationRepository>();
            mockLocalPathService = mockRepository.Create<ILocalPathService>();
            mockDateTimeUtility = mockRepository.Create<IDateTimeUtility>();
            mockLocalNotificationService = mockRepository.Create<ILocalNotificationService>();
        }

        #endregion

        #region Destructor


        public void Dispose()
        {
            var dir = $"{Path.GetTempPath()}/diagnosis_keys/";
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }

        #endregion

        #region Other Private Methods

        private AbsExposureDetectionBackgroundService CreateService()
        {

            return new ExposureDetectionBackgroundServiceMock(
                mockDiagnosisKeyRepository.Object,
                mockExposureNotificationApiService.Object,
                mockExposureConfigurationRepository.Object,
                mockLoggerService.Object,
                mockUserDataRepository.Object,
                mockServerConfigurationRepository.Object,
                mockLocalPathService.Object,
                mockDateTimeUtility.Object,
                mockLocalNotificationService.Object
                );
        }

        private IList<DiagnosisKeyEntry> CreateDiagnosisKeyEntryList(int region)
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

        [Fact(Skip = "[Occurs on Windows] System.IO.IOException : The process cannot access the file '1.zip' because it is being used by another process.")]
        public async Task ExposureDetectionAsync_NoNewDiagnosisKeyFound()
        {
            // Test Data
            IList<DiagnosisKeyEntry> diagnosisKeyEntryList = new List<DiagnosisKeyEntry>();

            // Mock Setup
            mockLocalPathService
                .Setup(x => x.CacheDirectory)
                .Returns(Path.GetTempPath());
            mockServerConfigurationRepository
                .Setup(x => x.Regions)
                .Returns(new string[] { "440" });
            mockServerConfigurationRepository
                .Setup(x => x.GetDiagnosisKeyListProvideServerUrl(It.IsAny<string>()))
                .Returns("https://example.com");
            mockExposureConfigurationRepository
                .Setup(x => x.GetExposureConfigurationAsync())
                .Returns(Task.FromResult(new ExposureConfiguration()));
            mockDiagnosisKeyRepository
                .Setup(x => x.GetDiagnosisKeysListAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HttpStatusCode.OK, diagnosisKeyEntryList));
            mockUserDataRepository
                .Setup(x => x.GetLastProcessDiagnosisKeyTimestampAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(0L));


            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.ExposureDetectionAsync();


            // Assert
            mockExposureNotificationApiService
                .Verify(x => x.ProvideDiagnosisKeysAsync(
                                It.IsAny<List<string>>(),
                                It.IsAny<CancellationTokenSource>()), Times.Never);
        }

        [Fact(Skip = "[Occurs on Windows] System.IO.IOException : The process cannot access the file '1.zip' because it is being used by another process.")]
        public async Task ExposureDetectionAsync_NewDiagnosisKeyFound()
        {
            // Test Data
            IList<DiagnosisKeyEntry> diagnosisKeyEntryList = CreateDiagnosisKeyEntryList(440);

            ExposureConfiguration exposureConfiguration = new ExposureConfiguration();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            // Mock Setup
            mockLocalPathService
                .Setup(x => x.CacheDirectory)
                .Returns(Path.GetTempPath());

            mockDiagnosisKeyRepository
                .Setup(x => x.GetDiagnosisKeysListAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HttpStatusCode.OK, diagnosisKeyEntryList));
            mockDiagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(
                    It.Is<DiagnosisKeyEntry>(s => s.Url == "https://example.com/1.zip"),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult($"file://tmp/diagnosis_keys/440/1"));
            mockDiagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(
                    It.Is<DiagnosisKeyEntry>(s => s.Url == "https://example.com/2.zip"),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult($"file://tmp/diagnosis_keys/440/2"));

            mockDiagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(
                     It.Is<DiagnosisKeyEntry>(s => s.Url == "https://example.com/3.zip"),
                     It.IsAny<string>(),
                     It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult($"file://tmp/diagnosis_keys/440/3"));

            mockServerConfigurationRepository
                .Setup(x => x.Regions)
                .Returns(new string[] { "440" });
            mockServerConfigurationRepository
                .Setup(x => x.GetDiagnosisKeyListProvideServerUrl(It.IsAny<string>()))
                .Returns("https://example.com");

            mockExposureConfigurationRepository
                .Setup(x => x.GetExposureConfigurationAsync())
                .Returns(Task.FromResult(exposureConfiguration));

            mockUserDataRepository
                .Setup(x => x.GetLastProcessDiagnosisKeyTimestampAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(0L));


            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.ExposureDetectionAsync(cancellationTokenSource);


            // Assert
            mockExposureNotificationApiService
                .Verify(x => x.ProvideDiagnosisKeysAsync(
                                It.Is<List<string>>( s => s.SequenceEqual(new List<string>() { $"file://tmp/diagnosis_keys/440/1", $"file://tmp/diagnosis_keys/440/2", $"file://tmp/diagnosis_keys/440/3" })),
                                It.Is<CancellationTokenSource>(s => s.Equals(cancellationTokenSource))), Times.Once);
            mockUserDataRepository
                .Verify(x => x.SetLastProcessDiagnosisKeyTimestampAsync("440", 1638630000), Times.Once);
        }

        [Fact(Skip = "[Occurs on Windows] System.IO.IOException : The process cannot access the file '1.zip' because it is being used by another process.")]
        public async Task ExposureDetectionAsync_MultiRegion()
        {
            // Test Data
            IList<DiagnosisKeyEntry> diagnosisKeyEntryList440 = CreateDiagnosisKeyEntryList(440);
            IList<DiagnosisKeyEntry> diagnosisKeyEntryList540 = CreateDiagnosisKeyEntryList(540);

            ExposureConfiguration exposureConfiguration = new ExposureConfiguration();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            // Mock Setup
            mockLocalPathService
                .Setup(x => x.CacheDirectory)
                .Returns(Path.GetTempPath());

            mockDiagnosisKeyRepository
                .Setup(x => x.GetDiagnosisKeysListAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HttpStatusCode.OK, diagnosisKeyEntryList440));
            mockDiagnosisKeyRepository
                .Setup(x => x.GetDiagnosisKeysListAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HttpStatusCode.OK, diagnosisKeyEntryList540));
            mockDiagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(
                    It.IsAny<DiagnosisKeyEntry>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult("DLFile"));

            mockServerConfigurationRepository
                .Setup(x => x.Regions)
                .Returns(new string[] { "440", "540" });
            mockServerConfigurationRepository
                .Setup(x => x.GetDiagnosisKeyListProvideServerUrl(It.IsAny<string>()))
                .Returns("https://example.com");

            mockExposureConfigurationRepository
                .Setup(x => x.GetExposureConfigurationAsync())
                .Returns(Task.FromResult(exposureConfiguration));

            mockUserDataRepository
                .Setup(x => x.GetLastProcessDiagnosisKeyTimestampAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(0L));


            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.ExposureDetectionAsync(cancellationTokenSource);


            // Assert
            mockServerConfigurationRepository.Verify(x => x.GetDiagnosisKeyListProvideServerUrl("440"), Times.Once);
            mockServerConfigurationRepository.Verify(x => x.GetDiagnosisKeyListProvideServerUrl("540"), Times.Once);

            mockUserDataRepository.Verify(x => x.GetLastProcessDiagnosisKeyTimestampAsync("440"), Times.Once);
            mockUserDataRepository.Verify(x => x.GetLastProcessDiagnosisKeyTimestampAsync("540"), Times.Once);

            mockExposureNotificationApiService
                .Verify(x => x.ProvideDiagnosisKeysAsync(
                                It.Is<List<string>>(s => s.SequenceEqual(new List<string>() { "DLFile", "DLFile", "DLFile" })),
                                It.Is<CancellationTokenSource>(s => s.Equals(cancellationTokenSource))), Times.Exactly(2));

            mockUserDataRepository
                .Verify(x => x.SetLastProcessDiagnosisKeyTimestampAsync("440", 1638630000), Times.Once);
            mockUserDataRepository
                .Verify(x => x.SetLastProcessDiagnosisKeyTimestampAsync("540", 1638630000), Times.Once);
        }

        [Fact(Skip = "[Occurs on Windows] System.IO.IOException : The process cannot access the file '1.zip' because it is being used by another process.")]
        public async Task ExposureDetectionAsync_DirectoryNotExistsAndFileRemoved()
        {
            // Test Data
            IList<DiagnosisKeyEntry> diagnosisKeyEntryList = CreateDiagnosisKeyEntryList(440);
            var tempPath = Path.GetTempPath();

            // Mock Setup

            ExposureConfiguration exposureConfiguration = new ExposureConfiguration();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            // Mock Setup
            mockLocalPathService
                .Setup(x => x.CacheDirectory)
                .Returns(Path.GetTempPath());

            mockDiagnosisKeyRepository
                .Setup(x => x.GetDiagnosisKeysListAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HttpStatusCode.OK, diagnosisKeyEntryList));
            mockDiagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(
                    It.Is<DiagnosisKeyEntry>(s => s.Url == "https://example.com/1.zip"),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Callback(() => {
                    Assert.True(Directory.Exists($"{tempPath}/diagnosis_keys/440/"));
                    File.Create($"{tempPath}/diagnosis_keys/440/1.zip");
                })
                .Returns(Task.FromResult($"{tempPath}/diagnosis_keys/440/1.zip"));
            mockDiagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(
                    It.Is<DiagnosisKeyEntry>(s => s.Url == "https://example.com/2.zip"),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Callback(() => {
                    Assert.True(Directory.Exists($"{tempPath}/diagnosis_keys/440/"));
                    File.Create($"{tempPath}/diagnosis_keys/440/2.zip");
                })
                .Returns(Task.FromResult($"{tempPath}/diagnosis_keys/440/2.zip"));

            mockDiagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(
                     It.Is<DiagnosisKeyEntry>(s => s.Url == "https://example.com/3.zip"),
                     It.IsAny<string>(),
                     It.IsAny<CancellationToken>()))
                .Callback(() => {
                    Assert.True(Directory.Exists($"{tempPath}/diagnosis_keys/440/"));
                    File.Create($"{tempPath}/diagnosis_keys/440/3.zip");
                })
                .Returns(Task.FromResult($"{tempPath}/diagnosis_keys/440/3.zip"));


            mockServerConfigurationRepository
                .Setup(x => x.Regions)
                .Returns(new string[] { "440" });
            mockServerConfigurationRepository
                .Setup(x => x.GetDiagnosisKeyListProvideServerUrl(It.IsAny<string>()))
                .Returns("https://example.com");

            mockExposureConfigurationRepository
                .Setup(x => x.GetExposureConfigurationAsync())
                .Returns(Task.FromResult(exposureConfiguration));

            mockUserDataRepository
                .Setup(x => x.GetLastProcessDiagnosisKeyTimestampAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(0L));


            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.ExposureDetectionAsync(cancellationTokenSource);


            // Assert
            mockDiagnosisKeyRepository.VerifyAll();
            Assert.False(File.Exists($"{tempPath}/diagnosis_keys/440/1.zip"));
            Assert.False(File.Exists($"{tempPath}/diagnosis_keys/440/2.zip"));
            Assert.False(File.Exists($"{tempPath}/diagnosis_keys/440/3.zip"));
            Assert.True(Directory.Exists($"{tempPath}/diagnosis_keys/440/"));
        }

        [Fact]
        public async Task ExposureDetectionAsync_ListFileNotFound()
        {
            ExposureConfiguration exposureConfiguration = new ExposureConfiguration();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            // Mock Setup
            mockLocalPathService
                .Setup(x => x.CacheDirectory)
                .Returns(Path.GetTempPath());

            // Setup ExposureNotification API
            mockExposureNotificationApiService.Setup(x => x.IsEnabledAsync())
                .ReturnsAsync(true);
            mockExposureNotificationApiService.Setup(x => x.GetStatusCodesAsync())
                .ReturnsAsync(new List<int>() {
                    ExposureNotificationStatus.Code_Android.ACTIVATED,
                });

            mockDiagnosisKeyRepository
                .Setup(x => x.GetDiagnosisKeysListAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HttpStatusCode.NotFound, new List<DiagnosisKeyEntry>()));

            mockServerConfigurationRepository
                .Setup(x => x.Regions)
                .Returns(new string[] { "440" });
            mockServerConfigurationRepository
                .Setup(x => x.GetDiagnosisKeyListProvideServerUrl(It.IsAny<string>()))
                .Returns("https://example.com");

            mockExposureConfigurationRepository
                .Setup(x => x.GetExposureConfigurationAsync())
                .ReturnsAsync(exposureConfiguration);

            mockUserDataRepository
                .Setup(x => x.GetLastProcessDiagnosisKeyTimestampAsync(It.IsAny<string>()))
                .ReturnsAsync(0L);


            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.ExposureDetectionAsync(cancellationTokenSource);


            // Assert
            mockDiagnosisKeyRepository.Verify(x => x.GetDiagnosisKeysListAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            mockDiagnosisKeyRepository.Verify(x => x.DownloadDiagnosisKeysAsync(It.IsAny<DiagnosisKeyEntry>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            mockUserDataRepository.Verify(x => x.SetCanConfirmExposure(false), Times.Once());
            mockUserDataRepository.Verify(x => x.SetIsMaxPerDayExposureDetectionAPILimitReached(false), Times.Once());
        }

        [Fact(Skip = "[Occurs on Windows] System.IO.IOException : The process cannot access the file '1.zip' because it is being used by another process.")]
        public async Task ExposureDetectionAsync_DirectoryExistsAndFileRemoved()
        {
            // Test Data
            IList<DiagnosisKeyEntry> diagnosisKeyEntryList = CreateDiagnosisKeyEntryList(440);
            var tempPath = Path.GetTempPath();
            Directory.CreateDirectory($"{tempPath}/diagnosis_keys/440/");

            // Mock Setup

            ExposureConfiguration exposureConfiguration = new ExposureConfiguration();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            // Mock Setup
            mockLocalPathService
                .Setup(x => x.CacheDirectory)
                .Returns(Path.GetTempPath());

            mockDiagnosisKeyRepository
                .Setup(x => x.GetDiagnosisKeysListAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HttpStatusCode.OK, diagnosisKeyEntryList));
            mockDiagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(
                    It.Is<DiagnosisKeyEntry>(s => s.Url == "https://example.com/1.zip"),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Callback(() => {
                    Assert.True(Directory.Exists($"{tempPath}/diagnosis_keys/440/"));
                    File.Create($"{tempPath}/diagnosis_keys/440/1.zip");
                })
                .Returns(Task.FromResult($"{tempPath}/diagnosis_keys/440/1.zip"));
            mockDiagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(
                    It.Is<DiagnosisKeyEntry>(s => s.Url == "https://example.com/2.zip"),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Callback(() => {
                    Assert.True(Directory.Exists($"{tempPath}/diagnosis_keys/440/"));
                    File.Create($"{tempPath}/diagnosis_keys/440/2.zip");
                })
                .Returns(Task.FromResult($"{tempPath}/diagnosis_keys/440/2.zip"));

            mockDiagnosisKeyRepository
                .Setup(x => x.DownloadDiagnosisKeysAsync(
                     It.Is<DiagnosisKeyEntry>(s => s.Url == "https://example.com/3.zip"),
                     It.IsAny<string>(),
                     It.IsAny<CancellationToken>()))
                .Callback(() => {
                    Assert.True(Directory.Exists($"{tempPath}/diagnosis_keys/440/"));
                    File.Create($"{tempPath}/diagnosis_keys/440/3.zip");
                })
                .Returns(Task.FromResult($"{tempPath}/diagnosis_keys/440/3.zip"));


            mockServerConfigurationRepository
                .Setup(x => x.Regions)
                .Returns(new string[] { "440" });
            mockServerConfigurationRepository
                .Setup(x => x.GetDiagnosisKeyListProvideServerUrl(It.IsAny<string>()))
                .Returns("https://example.com");

            mockExposureConfigurationRepository
                .Setup(x => x.GetExposureConfigurationAsync())
                .Returns(Task.FromResult(exposureConfiguration));

            mockUserDataRepository
                .Setup(x => x.GetLastProcessDiagnosisKeyTimestampAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(0L));


            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.ExposureDetectionAsync(cancellationTokenSource);


            // Assert
            mockDiagnosisKeyRepository.VerifyAll();
            Assert.False(File.Exists($"{tempPath}/diagnosis_keys/440/1.zip"));
            Assert.False(File.Exists($"{tempPath}/diagnosis_keys/440/2.zip"));
            Assert.False(File.Exists($"{tempPath}/diagnosis_keys/440/3.zip"));
            Assert.True(Directory.Exists($"{tempPath}/diagnosis_keys/440/"));
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
            IServerConfigurationRepository serverConfigurationRepository,
            ILocalPathService localPathService,
            IDateTimeUtility dateTimeUtility,
            ILocalNotificationService localNotificationService
        ) : base(
            diagnosisKeyRepository,
            exposureNotificationApiService,
            exposureConfigurationRepository,
            loggerService,
            userDataRepository,
            serverConfigurationRepository,
            localPathService,
            dateTimeUtility,
            localNotificationService
        )
        {

        }

        public override void Schedule()
        {
            throw new NotImplementedException();
        }

        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        public override async Task ShowEndOfServiceNotificationAync(CancellationTokenSource cancellationTokenSource = null)
        {
            await Task.CompletedTask;
        }
    }
    #endregion
}
