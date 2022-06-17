// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.UnitTests.Mocks;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class EventLogServiceTests
    {
        private readonly MockRepository mockRepository;

        private readonly Mock<ISendEventLogStateRepository> _sendEventLogStateRepository;
        private readonly Mock<IEventLogRepository> _eventLogRepository;
        private readonly Mock<IServerConfigurationRepository> _serverConfigurationRepository;
        private readonly Mock<IEssentialsService> _essentialsService;
        private readonly Mock<IDeviceVerifier> _deviceVerifier;
        private readonly Mock<IHttpClientService> _httpClientService;


        private readonly Mock<ILoggerService> _loggerService;

        public EventLogServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);

            _sendEventLogStateRepository = mockRepository.Create<ISendEventLogStateRepository>();
            _eventLogRepository = mockRepository.Create<IEventLogRepository>();
            _serverConfigurationRepository = mockRepository.Create<IServerConfigurationRepository>();
            _essentialsService = mockRepository.Create<IEssentialsService>();
            _deviceVerifier = mockRepository.Create<IDeviceVerifier>();
            _httpClientService = mockRepository.Create<IHttpClientService>();
            _loggerService = mockRepository.Create<ILoggerService>();
        }

        private EventLogService CreateService()
        {
            _httpClientService.Setup(x => x.Create())
                .Returns(new HttpClient());

            return new EventLogService(
                _sendEventLogStateRepository.Object,
                _eventLogRepository.Object,
                _serverConfigurationRepository.Object,
                _essentialsService.Object,
                _deviceVerifier.Object,
                _httpClientService.Object,
                _loggerService.Object
                );
        }

        private List<EventLog> CreateDummyEventLogList()
        {
            return new List<EventLog>()
            {
                new EventLog()
                {
                    HasConsent = true,
                    Epoch = 1,
                    Type = ISendEventLogStateRepository.EVENT_TYPE_EXPOSURE_NOTIFIED.Type,
                    Subtype = ISendEventLogStateRepository.EVENT_TYPE_EXPOSURE_NOTIFIED.SubType,
                    Content = "{\"key\" : \"value\"}",
                },
                new EventLog()
                {
                    HasConsent = true,
                    Epoch = 2,
                    Type = ISendEventLogStateRepository.EVENT_TYPE_EXPOSURE_DATA.Type,
                    Subtype = ISendEventLogStateRepository.EVENT_TYPE_EXPOSURE_DATA.SubType,
                    Content = "{\"key\" : \"value\"}",
                }
            };
        }

        [Fact]
        public async Task SendAllAsyncTest1()
        {
            // Dummy data
            List<EventLog> dummyEventLogList = CreateDummyEventLogList();

            // Mock Setup
            var mockHttpClient = new HttpClient(new MockHttpHandler((r, c) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.Created);
                response.Content = new StringContent("{}");
                response.Content.Headers.Remove("Content-Type");
                response.Content.Headers.Add("Content-Type", "application/json");
                return response;
            }));
            _httpClientService.Setup(x => x.CreateApiClient()).Returns(mockHttpClient);

            _serverConfigurationRepository.Setup(x => x.EventLogApiEndpoint).Returns("https://example.com/api");
            _sendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(ISendEventLogStateRepository.EVENT_TYPE_EXPOSURE_NOTIFIED))
                .Returns(SendEventLogState.Enable);
            _sendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(ISendEventLogStateRepository.EVENT_TYPE_EXPOSURE_DATA))
                .Returns(SendEventLogState.Enable);
            _eventLogRepository.Setup(x => x.GetLogsAsync(long.MaxValue))
                .ReturnsAsync(dummyEventLogList);

            // Test Case
            var unitUnderTest = CreateService();
            var sentEventList = await unitUnderTest.SendAllAsync(
                long.MaxValue,
                1
                );

            // Assert
            Assert.Equal(2, sentEventList.Count);
            Assert.Equal(dummyEventLogList[0], sentEventList[0]);
            Assert.Equal(dummyEventLogList[1], sentEventList[1]);

            _loggerService.Verify(x => x.Info("Send complete.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            _loggerService.Verify(x => x.Info("Done.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);

            _serverConfigurationRepository.Verify(x => x.LoadAsync(), Times.Once);
            _deviceVerifier.Verify(x => x.VerifyAsync(It.IsAny<V1EventLogRequest>()));

            Assert.True(dummyEventLogList[0].HasConsent);
            _eventLogRepository.Verify(x => x.RemoveAsync(dummyEventLogList[0]), Times.Exactly(1));

            Assert.True(dummyEventLogList[1].HasConsent);
            _eventLogRepository.Verify(x => x.RemoveAsync(dummyEventLogList[1]), Times.Exactly(1));

        }

        [Fact]
        public async Task SendAllAsyncTest_withdrawn_consent()
        {
            // Dummy data
            List<EventLog> dummyEventLogList = CreateDummyEventLogList();

            // Mock Setup
            var mockHttpClient = new HttpClient(new MockHttpHandler((r, c) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.Created);
                response.Content = new StringContent("{}");
                response.Content.Headers.Remove("Content-Type");
                response.Content.Headers.Add("Content-Type", "application/json");
                return response;
            }));
            _httpClientService.Setup(x => x.CreateApiClient()).Returns(mockHttpClient);

            _serverConfigurationRepository.Setup(x => x.EventLogApiEndpoint).Returns("https://example.com/api");
            _sendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(ISendEventLogStateRepository.EVENT_TYPE_EXPOSURE_NOTIFIED))
                .Returns(SendEventLogState.Enable);

            // Consent withdrawn
            _sendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(ISendEventLogStateRepository.EVENT_TYPE_EXPOSURE_DATA))
                .Returns(SendEventLogState.Disable);

            _eventLogRepository.Setup(x => x.GetLogsAsync(long.MaxValue))
                .ReturnsAsync(dummyEventLogList);

            // Test Case
            var unitUnderTest = CreateService();
            var sentEventList = await unitUnderTest.SendAllAsync(
                long.MaxValue,
                1
                );

            // Assert
            Assert.Single(sentEventList);
            Assert.Equal(dummyEventLogList[0], sentEventList[0]);

            _loggerService.Verify(x => x.Info("Send complete.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            _loggerService.Verify(x => x.Info("Done.", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);

            _serverConfigurationRepository.Verify(x => x.LoadAsync(), Times.Once);
            _deviceVerifier.Verify(x => x.VerifyAsync(It.IsAny<V1EventLogRequest>()));

            Assert.True(dummyEventLogList[0].HasConsent);
            _eventLogRepository.Verify(x => x.RemoveAsync(dummyEventLogList[0]), Times.Exactly(1));

            // Consent withdrawn
            Assert.False(dummyEventLogList[1].HasConsent);
            _eventLogRepository.Verify(x => x.RemoveAsync(dummyEventLogList[1]), Times.Exactly(1));

        }
    }
}
