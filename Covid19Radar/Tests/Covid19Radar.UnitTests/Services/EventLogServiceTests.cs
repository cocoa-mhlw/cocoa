// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class EventLogServiceTests
    {
        private readonly MockRepository _mockRepository;

        private readonly Mock<ISendEventLogStateRepository> _sendEventLogStateRepository;
        private readonly Mock<IEventLogRepository> _eventLogRepository;
        private readonly Mock<IEssentialsService> _essentialsService;
        private readonly Mock<IDeviceVerifier> _deviceVerifier;
        private readonly Mock<IHttpDataService> _httpDataService;

        private readonly Mock<ILoggerService> _loggerService;

        public EventLogServiceTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);

            _sendEventLogStateRepository = _mockRepository.Create<ISendEventLogStateRepository>();
            _eventLogRepository = _mockRepository.Create<IEventLogRepository>();
            _essentialsService = _mockRepository.Create<IEssentialsService>();
            _deviceVerifier = _mockRepository.Create<IDeviceVerifier>();
            _httpDataService = _mockRepository.Create<IHttpDataService>();
            _loggerService = _mockRepository.Create<ILoggerService>();
        }

        private EventLogService CreateService()
        {
            return new EventLogService(
                _sendEventLogStateRepository.Object,
                _eventLogRepository.Object,
                _essentialsService.Object,
                _deviceVerifier.Object,
                _httpDataService.Object,
                _loggerService.Object);
        }

        private List<EventLog> CreateDummyEventLogList()
        {
            return new List<EventLog>()
            {
                new EventLog()
                {
                    HasConsent = true,
                    Epoch = 1,
                    Type = EventType.ExposureNotified.Type,
                    Subtype = EventType.ExposureNotified.SubType,
                    Content = JObject.FromObject(new { key = "value2" }),
                },
                new EventLog()
                {
                    HasConsent = false,
                    Epoch = 1,
                    Type = EventType.ExposureNotified.Type,
                    Subtype = EventType.ExposureNotified.SubType,
                    Content = JObject.FromObject(new { key = "value1" }),
                }
            };
        }

        [Fact]
        public async Task SendAllAsyncTest1()
        {
            // Dummy data
            List<EventLog> dummyEventLogList = CreateDummyEventLogList();

            _httpDataService.Setup(x => x.PutEventLog(It.IsAny<V1EventLogRequest>()))
                .ReturnsAsync(new ApiResponse<string>((int)HttpStatusCode.Created, ""));

            // Mock Setup
            _sendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(EventType.ExposureNotified))
                .Returns(SendEventLogState.Enable);
            _eventLogRepository.Setup(x => x.GetLogsAsync(long.MaxValue))
                .ReturnsAsync(dummyEventLogList);

            _deviceVerifier.Setup(x => x.IsErrorPayload(It.IsAny<string>())).Returns(false);

            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.SendAllAsync(long.MaxValue, 1);

            _loggerService.Verify(x => x.Warning(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _loggerService.Verify(x => x.Info("Event log send successful.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _loggerService.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());

            _httpDataService.Verify(x => x.PutEventLog(It.Is<V1EventLogRequest>(x => x.EventLogs.Count == 1)), Times.Once());

            _deviceVerifier.Verify(x => x.VerifyAsync(It.IsAny<V1EventLogRequest>()), Times.Once());

            Assert.True(dummyEventLogList[0].HasConsent);
            _eventLogRepository.Verify(x => x.RemoveAsync(dummyEventLogList[0]), Times.Once());
        }

        [Fact]
        public async Task SendAllAsyncTest_withdrawn_consent()
        {
            // Dummy data
            List<EventLog> dummyEventLogList = CreateDummyEventLogList();

            // Mock Setup
            _httpDataService.Setup(x => x.PutEventLog(It.IsAny<V1EventLogRequest>()))
                .ReturnsAsync(new ApiResponse<string>((int)HttpStatusCode.Created, ""));

            _sendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(EventType.ExposureNotified))
                .Returns(SendEventLogState.Enable);

            _eventLogRepository.Setup(x => x.GetLogsAsync(long.MaxValue))
                .ReturnsAsync(dummyEventLogList);

            _deviceVerifier.Setup(x => x.IsErrorPayload(It.IsAny<string>())).Returns(false);

            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.SendAllAsync(long.MaxValue, 1);

            _loggerService.Verify(x => x.Warning(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _loggerService.Verify(x => x.Info("Event log send successful.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _loggerService.Verify(x => x.Error("Event log send failed all.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());

            _httpDataService.Verify(x => x.PutEventLog(It.Is<V1EventLogRequest>(x => x.EventLogs.Count == 1 && !x.EventLogs.Exists(x => x.Type != EventType.ExposureNotified.Type))), Times.Once());

            _deviceVerifier.Verify(x => x.VerifyAsync(It.IsAny<V1EventLogRequest>()), Times.Once());

            Assert.True(dummyEventLogList[0].HasConsent);
            _eventLogRepository.Verify(x => x.RemoveAsync(dummyEventLogList[0]), Times.Once());

            // Consent withdrawn
            Assert.False(dummyEventLogList[1].HasConsent);
            _eventLogRepository.Verify(x => x.RemoveAsync(dummyEventLogList[1]), Times.Once());
        }

        [Fact]
        public async Task SendAllAsyncTest_RetrySuccess()
        {
            // Dummy data
            List<EventLog> dummyEventLogList = CreateDummyEventLogList();

            // Mock Setup
            _httpDataService.SetupSequence(x => x.PutEventLog(It.IsAny<V1EventLogRequest>()))
                .ReturnsAsync(new ApiResponse<string>((int)HttpStatusCode.BadGateway, ""))
                .ReturnsAsync(new ApiResponse<string>((int)HttpStatusCode.BadGateway, ""))
                .ReturnsAsync(new ApiResponse<string>((int)HttpStatusCode.Created, ""));

            _sendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(EventType.ExposureNotified))
                .Returns(SendEventLogState.Enable);

            _eventLogRepository.Setup(x => x.GetLogsAsync(long.MaxValue))
                .ReturnsAsync(dummyEventLogList);

            _deviceVerifier.Setup(x => x.IsErrorPayload(It.IsAny<string>())).Returns(false);

            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.SendAllAsync(long.MaxValue, 3);

            _loggerService.Verify(x => x.Warning("Event log send failed. retryAttempt:1 delay:1sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _loggerService.Verify(x => x.Warning("Event log send failed. retryAttempt:2 delay:2sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _loggerService.Verify(x => x.Warning("Event log send failed. retryAttempt:3 delay:4sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _loggerService.Verify(x => x.Info("Event log send successful.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _loggerService.Verify(x => x.Error("Event log send failed all.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());

            _httpDataService.Verify(x => x.PutEventLog(It.IsAny<V1EventLogRequest>()), Times.Exactly(3));
            _deviceVerifier.Verify(x => x.VerifyAsync(It.IsAny<V1EventLogRequest>()), Times.Exactly(3));
            _eventLogRepository.Verify(x => x.RemoveAsync(It.IsAny<EventLog>()), Times.Exactly(dummyEventLogList.Count));
        }

        [Fact]
        public async Task SendAllAsyncTest_RetryFailure()
        {
            // Dummy data
            List<EventLog> dummyEventLogList = CreateDummyEventLogList();

            // Mock Setup
            _httpDataService.Setup(x => x.PutEventLog(It.IsAny<V1EventLogRequest>()))
                .ReturnsAsync(new ApiResponse<string>((int)HttpStatusCode.BadRequest, ""));

            _sendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(EventType.ExposureNotified))
                .Returns(SendEventLogState.Enable);

            _eventLogRepository.Setup(x => x.GetLogsAsync(long.MaxValue))
                .ReturnsAsync(dummyEventLogList);

            _deviceVerifier.Setup(x => x.IsErrorPayload(It.IsAny<string>())).Returns(false);

            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.SendAllAsync(long.MaxValue, 3);

            _loggerService.Verify(x => x.Warning("Event log send failed. retryAttempt:1 delay:1sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _loggerService.Verify(x => x.Warning("Event log send failed. retryAttempt:2 delay:2sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _loggerService.Verify(x => x.Warning("Event log send failed. retryAttempt:3 delay:4sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _loggerService.Verify(x => x.Warning("Event log send failed. retryAttempt:4 delay:8sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _loggerService.Verify(x => x.Info("Event log send successful.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _loggerService.Verify(x => x.Error("Event log send failed all.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());

            _httpDataService.Verify(x => x.PutEventLog(It.IsAny<V1EventLogRequest>()), Times.Exactly(4));
            _deviceVerifier.Verify(x => x.VerifyAsync(It.IsAny<V1EventLogRequest>()), Times.Exactly(4));
            _eventLogRepository.Verify(x => x.RemoveAsync(It.IsAny<EventLog>()), Times.Never());
        }

        [Fact]
        public async Task SendAllAsyncTest_RetryException()
        {
            // Dummy data
            List<EventLog> dummyEventLogList = CreateDummyEventLogList();

            // Mock Setup
            _httpDataService.Setup(x => x.PutEventLog(It.IsAny<V1EventLogRequest>()))
                .Throws(new Exception());

            _sendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(EventType.ExposureNotified))
                .Returns(SendEventLogState.Enable);

            _eventLogRepository.Setup(x => x.GetLogsAsync(long.MaxValue))
                .ReturnsAsync(dummyEventLogList);

            _deviceVerifier.Setup(x => x.IsErrorPayload(It.IsAny<string>())).Returns(false);

            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.SendAllAsync(long.MaxValue, 3);

            _loggerService.Verify(x => x.Warning("Event log send failed. retryAttempt:1 delay:1sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _loggerService.Verify(x => x.Warning("Event log send failed. retryAttempt:2 delay:2sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _loggerService.Verify(x => x.Warning("Event log send failed. retryAttempt:3 delay:4sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _loggerService.Verify(x => x.Warning("Event log send failed. retryAttempt:4 delay:8sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _loggerService.Verify(x => x.Info("Event log send successful.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _loggerService.Verify(x => x.Error("Event log send failed all.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());

            _httpDataService.Verify(x => x.PutEventLog(It.IsAny<V1EventLogRequest>()), Times.Exactly(4));
            _deviceVerifier.Verify(x => x.VerifyAsync(It.IsAny<V1EventLogRequest>()), Times.Exactly(4));
            _eventLogRepository.Verify(x => x.RemoveAsync(It.IsAny<EventLog>()), Times.Never());
        }

        [Fact]
        public async Task SendAllAsyncTest_DisableAll()
        {
            // Dummy data
            List<EventLog> dummyEventLogList = CreateDummyEventLogList();

            // Mock Setup
            _httpDataService.Setup(x => x.PutEventLog(It.IsAny<V1EventLogRequest>()))
                .ReturnsAsync(new ApiResponse<string>((int)HttpStatusCode.Created, ""));

            _sendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(EventType.ExposureNotified))
                .Returns(SendEventLogState.Disable);

            _eventLogRepository.Setup(x => x.GetLogsAsync(long.MaxValue))
                .ReturnsAsync(dummyEventLogList);

            _deviceVerifier.Setup(x => x.IsErrorPayload(It.IsAny<string>())).Returns(false);

            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.SendAllAsync(long.MaxValue, 1);

            _httpDataService.Verify(x => x.PutEventLog(It.IsAny<V1EventLogRequest>()), Times.Never());
        }

        [Fact]
        public async Task SendAllAsyncTest_DeviceVerifierFailure()
        {
            // Dummy data
            List<EventLog> dummyEventLogList = CreateDummyEventLogList();

            // Mock Setup
            _httpDataService.Setup(x => x.PutEventLog(It.IsAny<V1EventLogRequest>()))
                .ReturnsAsync(new ApiResponse<string>((int)HttpStatusCode.Created, ""));

            _sendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(EventType.ExposureNotified))
                .Returns(SendEventLogState.Enable);

            _eventLogRepository.Setup(x => x.GetLogsAsync(long.MaxValue))
                .ReturnsAsync(dummyEventLogList);

            _deviceVerifier.Setup(x => x.IsErrorPayload(It.IsAny<string>())).Returns(true);

            // Test Case
            var unitUnderTest = CreateService();
            await unitUnderTest.SendAllAsync(long.MaxValue, 0);

            _loggerService.Verify(x => x.Warning("Payload creation failed. retryAttempt:1 delay:4sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _loggerService.Verify(x => x.Warning("Payload creation failed. retryAttempt:2 delay:8sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _loggerService.Verify(x => x.Warning("Payload creation failed. retryAttempt:3 delay:16sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _loggerService.Verify(x => x.Warning("Payload creation failed. retryAttempt:4 delay:32sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());

            _loggerService.Verify(x => x.Warning("Event log send failed. retryAttempt:1 delay:1sec", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());

            _loggerService.Verify(x => x.Info("Event log send successful.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _loggerService.Verify(x => x.Error("Event log send failed all.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());

            _httpDataService.Verify(x => x.PutEventLog(It.IsAny<V1EventLogRequest>()), Times.Never());
            _deviceVerifier.Verify(x => x.VerifyAsync(It.IsAny<V1EventLogRequest>()), Times.Exactly(4));
            _eventLogRepository.Verify(x => x.RemoveAsync(It.IsAny<EventLog>()), Times.Never());
        }
    }
}