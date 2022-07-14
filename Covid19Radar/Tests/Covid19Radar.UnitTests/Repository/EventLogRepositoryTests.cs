// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Covid19Radar.UnitTests.Repository
{
    public class EventLogRepositoryTests
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<ISendEventLogStateRepository> _mockSendEventLogStateRepository;
        private readonly Mock<IDateTimeUtility> _mockDateTimeUtility;
        private readonly Mock<ILocalPathService> _mockLocalPathService;
        private readonly Mock<ILoggerService> _mockLoggerService;

        public EventLogRepositoryTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockSendEventLogStateRepository = _mockRepository.Create<ISendEventLogStateRepository>();
            _mockDateTimeUtility = _mockRepository.Create<IDateTimeUtility>();
            _mockLocalPathService = _mockRepository.Create<ILocalPathService>();
            _mockLoggerService = _mockRepository.Create<ILoggerService>();
        }

        private EventLogRepository CreateRepository()
        {
            return new EventLogRepository(
                _mockSendEventLogStateRepository.Object,
                _mockDateTimeUtility.Object,
                _mockLocalPathService.Object,
                _mockLoggerService.Object);
        }

        [Fact]
        public async Task RotateAsyncTest_NoDir()
        {
            string eventLogsTempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "test_eventlogs");

            _mockLocalPathService.SetupGet(x => x.EventLogDirPath).Returns(eventLogsTempPath);

            EventLogRepository unitUnderTest = CreateRepository();
            await unitUnderTest.RotateAsync(14 * 24 * 60 * 60);

            _mockLoggerService.Verify(x => x.Info(It.Is<string>(x => x.StartsWith("file:")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _mockLoggerService.Verify(x => x.Info("Delete", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _mockLoggerService.Verify(x => x.Info("Keep", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public async Task RotateAsyncTest_Empty()
        {
            string eventLogsTempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "test_eventlogs");
            Directory.CreateDirectory(eventLogsTempPath);

            _mockLocalPathService.SetupGet(x => x.EventLogDirPath).Returns(eventLogsTempPath);

            EventLogRepository unitUnderTest = CreateRepository();
            await unitUnderTest.RotateAsync(14 * 24 * 60 * 60);

            _mockLoggerService.Verify(x => x.Info(It.Is<string>(x => x.StartsWith("file:")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _mockLoggerService.Verify(x => x.Info("Delete", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _mockLoggerService.Verify(x => x.Info("Keep", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());

            Directory.Delete(eventLogsTempPath, true);
        }

        [Fact]
        public async Task RotateAsyncTest_Remove13days()
        {
            string eventLogsTempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "test_eventlogs");
            Directory.CreateDirectory(eventLogsTempPath);

            DateTime now = DateTime.UtcNow;
            await WriteEventLog(eventLogsTempPath, "test-01.log", epoch: now.AddDays(-12).ToUnixEpoch());
            await WriteEventLog(eventLogsTempPath, "test-02.log", epoch: now.AddDays(-13).ToUnixEpoch());
            await WriteEventLog(eventLogsTempPath, "test-03.log", epoch: now.AddDays(-14).ToUnixEpoch());
            await WriteEventLog(eventLogsTempPath, "test-04.log", epoch: now.AddDays(-15).ToUnixEpoch());

            _mockLocalPathService.SetupGet(x => x.EventLogDirPath).Returns(eventLogsTempPath);

            await Task.Delay(1500);

            EventLogRepository unitUnderTest = CreateRepository();
            await unitUnderTest.RotateAsync(13 * 24 * 60 * 60);

            _mockLoggerService.Verify(x => x.Info(It.Is<string>(x => x.StartsWith("file:")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(4));
            _mockLoggerService.Verify(x => x.Info("Delete", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(3));
            _mockLoggerService.Verify(x => x.Info("Keep", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());

            Assert.True(File.Exists(Path.Combine(eventLogsTempPath, "test-01.log")));
            Assert.False(File.Exists(Path.Combine(eventLogsTempPath, "test-02.log")));
            Assert.False(File.Exists(Path.Combine(eventLogsTempPath, "test-03.log")));
            Assert.False(File.Exists(Path.Combine(eventLogsTempPath, "test-04.log")));

            Directory.Delete(eventLogsTempPath, true);
        }

        [Fact]
        public async Task RotateAsyncTest_Remove14days()
        {
            string eventLogsTempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "test_eventlogs");
            Directory.CreateDirectory(eventLogsTempPath);

            DateTime now = DateTime.UtcNow;
            await WriteEventLog(eventLogsTempPath, "test-01.log", epoch: now.AddDays(-12).ToUnixEpoch());
            await WriteEventLog(eventLogsTempPath, "test-02.log", epoch: now.AddDays(-13).ToUnixEpoch());
            await WriteEventLog(eventLogsTempPath, "test-03.log", epoch: now.AddDays(-14).ToUnixEpoch());
            await WriteEventLog(eventLogsTempPath, "test-04.log", epoch: now.AddDays(-15).ToUnixEpoch());

            _mockLocalPathService.SetupGet(x => x.EventLogDirPath).Returns(eventLogsTempPath);

            await Task.Delay(1500);

            EventLogRepository unitUnderTest = CreateRepository();
            await unitUnderTest.RotateAsync(14 * 24 * 60 * 60);

            _mockLoggerService.Verify(x => x.Info(It.Is<string>(x => x.StartsWith("file:")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(4));
            _mockLoggerService.Verify(x => x.Info("Delete", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(2));
            _mockLoggerService.Verify(x => x.Info("Keep", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(2));
            _mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());

            Assert.True(File.Exists(Path.Combine(eventLogsTempPath, "test-01.log")));
            Assert.True(File.Exists(Path.Combine(eventLogsTempPath, "test-02.log")));
            Assert.False(File.Exists(Path.Combine(eventLogsTempPath, "test-03.log")));
            Assert.False(File.Exists(Path.Combine(eventLogsTempPath, "test-04.log")));

            Directory.Delete(eventLogsTempPath, true);
        }

        [Fact]
        public async Task RotateAsyncTest_Exception()
        {
            string eventLogsTempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "test_eventlogs");
            Directory.CreateDirectory(eventLogsTempPath);

            DateTime now = DateTime.UtcNow;
            await WriteEventLog(eventLogsTempPath, "test-01.log", epoch: now.AddDays(-12).ToUnixEpoch());
            await WriteEventLog(eventLogsTempPath, "test-02.log", epoch: now.AddDays(-13).ToUnixEpoch());
            await WriteEventLog(eventLogsTempPath, "test-03.log", epoch: now.AddDays(-14).ToUnixEpoch());
            await WriteEventLog(eventLogsTempPath, "test-04.log", epoch: now.AddDays(-15).ToUnixEpoch());
            await File.WriteAllTextAsync(Path.Combine(eventLogsTempPath, "test-error.log"), "json-parse-error-content");

            _mockLocalPathService.SetupGet(x => x.EventLogDirPath).Returns(eventLogsTempPath);

            await Task.Delay(1500);

            EventLogRepository unitUnderTest = CreateRepository();
            await unitUnderTest.RotateAsync(14 * 24 * 60 * 60);

            _mockLoggerService.Verify(x => x.Info(It.Is<string>(x => x.StartsWith("file:")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(5));
            _mockLoggerService.Verify(x => x.Info("Delete", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(2));
            _mockLoggerService.Verify(x => x.Info("Keep", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(2));
            _mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());

            Assert.True(File.Exists(Path.Combine(eventLogsTempPath, "test-01.log")));
            Assert.True(File.Exists(Path.Combine(eventLogsTempPath, "test-02.log")));
            Assert.False(File.Exists(Path.Combine(eventLogsTempPath, "test-03.log")));
            Assert.False(File.Exists(Path.Combine(eventLogsTempPath, "test-04.log")));
            Assert.False(File.Exists(Path.Combine(eventLogsTempPath, "test-error.log")));

            Directory.Delete(eventLogsTempPath, true);
        }

        private static async Task WriteEventLog(
            string basePath,
            string fileName,
            bool hasConsent = true,
            long epoch = 0,
            string type = "type",
            string subType = "sub-type",
            string content = "\"content\"")
        {
            EventLog eventLog = new EventLog
            {
                HasConsent = hasConsent,
                Epoch = epoch,
                Type = type,
                Subtype = subType,
                Content = JToken.Parse(content)
            };
            string eventLogContent = JsonConvert.SerializeObject(eventLog);
            await File.WriteAllTextAsync(Path.Combine(basePath, fileName), eventLogContent);
        }
    }
}

