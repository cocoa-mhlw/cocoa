// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Repository
{
    public class SendEventLogStateRepositoryTests
    {
        private static EventType DUMMY_EVENT1 = new EventType("DummyEventType1", "DummyEventSubtype1");
        private static EventType DUMMY_EVENT2 = new EventType("DummyEventType2", "DummyEventSubtype2");

        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IPreferencesService> mockPreferencesService;

        public SendEventLogStateRepositoryTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockPreferencesService = mockRepository.Create<IPreferencesService>();
        }

        private ISendEventLogStateRepository CreateRepository()
            => new SendEventLogStateRepository(
                mockPreferencesService.Object,
                mockLoggerService.Object
                );

        [Fact]
        public void GetSendEventLogStateTest_NotExists()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.SendEventLogState)).Returns(false);

            var sendEventLogStateRepository = CreateRepository();

            var exposureNotificationNotified = sendEventLogStateRepository
                .GetSendEventLogState(DUMMY_EVENT1);

            Assert.Equal(SendEventLogState.NotSet, exposureNotificationNotified);
        }

        [Fact]
        public void GetSendEventLogStateTest_InvalidJson()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.SendEventLogState)).Returns(true);
            mockPreferencesService.Setup(s => s.GetStringValue(PreferenceKey.SendEventLogState, It.IsAny<string>()))
                .Returns("{]");

            var sendEventLogStateRepository = CreateRepository();

            var exposureNotificationNotified = sendEventLogStateRepository
                .GetSendEventLogState(DUMMY_EVENT1);

            Assert.Equal(SendEventLogState.NotSet, exposureNotificationNotified);
        }

        [Fact]
        public void GetSendEventLogStateTest_Blank()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.SendEventLogState)).Returns(true);
            mockPreferencesService.Setup(s => s.GetStringValue(PreferenceKey.SendEventLogState, It.IsAny<string>()))
                .Returns("{ }");

            var sendEventLogStateRepository = CreateRepository();

            var exposureNotificationNotified = sendEventLogStateRepository
                .GetSendEventLogState(DUMMY_EVENT1);

            Assert.Equal(SendEventLogState.NotSet, exposureNotificationNotified);
        }

        [Fact]
        public void GetSendEventLogStateTest_NotSet()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.SendEventLogState)).Returns(true);
            mockPreferencesService.Setup(s => s.GetStringValue(PreferenceKey.SendEventLogState, It.IsAny<string>()))
                .Returns("{ \"DummyEventType\": 0 }");

            var sendEventLogStateRepository = CreateRepository();

            var exposureNotificationNotified = sendEventLogStateRepository
                .GetSendEventLogState(DUMMY_EVENT1);

            Assert.Equal(SendEventLogState.NotSet, exposureNotificationNotified);
        }

        [Fact]
        public void GetSendEventLogStateTest_Disable()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.SendEventLogState)).Returns(true);
            mockPreferencesService.Setup(s => s.GetStringValue(PreferenceKey.SendEventLogState, It.IsAny<string>()))
                .Returns("{ \"DummyEventType1-DummyEventSubtype1\": -1 }");

            var sendEventLogStateRepository = CreateRepository();

            var exposureNotificationNotified = sendEventLogStateRepository
                .GetSendEventLogState(DUMMY_EVENT1);

            Assert.Equal(SendEventLogState.Disable, exposureNotificationNotified);
        }

        [Fact]
        public void GetSendEventLogStateTest_Enable()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.SendEventLogState)).Returns(true);
            mockPreferencesService.Setup(s => s.GetStringValue(PreferenceKey.SendEventLogState, It.IsAny<string>()))
                .Returns("{ \"DummyEventType1-DummyEventSubtype1\": 1, \"DummyEventType2-DummyEventSubtype2\": -11 }");

            var sendEventLogStateRepository = CreateRepository();

            var exposureNotificationNotified = sendEventLogStateRepository
                .GetSendEventLogState(DUMMY_EVENT1);

            Assert.Equal(SendEventLogState.Enable, exposureNotificationNotified);
        }

        [Fact]
        public void SetSendEventLogStateTest_Create()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.SendEventLogState)).Returns(true);
            mockPreferencesService.Setup(s => s.GetStringValue(PreferenceKey.SendEventLogState, It.IsAny<string>()))
                .Returns("{}");

            var sendEventLogStateRepository = CreateRepository();

            sendEventLogStateRepository
                .SetSendEventLogState(DUMMY_EVENT1, SendEventLogState.Enable);

            mockPreferencesService.Verify(s => s.SetStringValue(
                PreferenceKey.SendEventLogState,
                "{\"DummyEventType1-DummyEventSubtype1\":1}"
                ), Times.Once());
        }

        [Fact]
        public void SetSendEventLogStateTest_Create_InvalidJson()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.SendEventLogState)).Returns(true);
            mockPreferencesService.Setup(s => s.GetStringValue(PreferenceKey.SendEventLogState, It.IsAny<string>()))
                .Returns("{]");

            var sendEventLogStateRepository = CreateRepository();

            sendEventLogStateRepository
                .SetSendEventLogState(DUMMY_EVENT1, SendEventLogState.Enable);

            mockPreferencesService.Verify(s => s.SetStringValue(
                PreferenceKey.SendEventLogState,
                "{\"DummyEventType1-DummyEventSubtype1\":1}"
                ), Times.Once());
        }

        [Fact]
        public void SetSendEventLogStateTest_Update()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.SendEventLogState)).Returns(true);
            mockPreferencesService.Setup(s => s.GetStringValue(PreferenceKey.SendEventLogState, It.IsAny<string>()))
                .Returns("{\"DummyEventType1-DummyEventSubtype1\": -1}");

            var sendEventLogStateRepository = CreateRepository();

            sendEventLogStateRepository
                .SetSendEventLogState(DUMMY_EVENT1, SendEventLogState.Enable);

            mockPreferencesService.Verify(s => s.SetStringValue(
                PreferenceKey.SendEventLogState,
                "{\"DummyEventType1-DummyEventSubtype1\":1}"
                ), Times.Once());
        }

        [Fact]
        public void SetSendEventLogStateTest_Append()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.SendEventLogState)).Returns(true);
            mockPreferencesService.Setup(s => s.GetStringValue(PreferenceKey.SendEventLogState, It.IsAny<string>()))
                .Returns("{ \"DummyEventType0-DummyEventSubtype0\": -1 }");

            var sendEventLogStateRepository = CreateRepository();

            sendEventLogStateRepository
                .SetSendEventLogState(DUMMY_EVENT1, SendEventLogState.Enable);

            mockPreferencesService.Verify(s => s.SetStringValue(
                PreferenceKey.SendEventLogState,
                "{\"DummyEventType0-DummyEventSubtype0\":-1,\"DummyEventType1-DummyEventSubtype1\":1}"
                ), Times.Once());
        }

        [Fact]
        public void IsExistNotSetEventTypeTest_NotSet()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.SendEventLogState)).Returns(true);

            mockPreferencesService.Setup(s => s.GetStringValue(PreferenceKey.SendEventLogState, It.IsAny<string>()))
                .Returns($"{{ \"{EventType.ExposureNotified}\": {(int)SendEventLogState.NotSet} }}");

            var sendEventLogStateRepository = CreateRepository();

            Assert.True(sendEventLogStateRepository.IsExistNotSetEventType());
        }

        [Fact]
        public void IsExistNotSetEventTypeTest_Disabled()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.SendEventLogState)).Returns(true);

            mockPreferencesService.Setup(s => s.GetStringValue(PreferenceKey.SendEventLogState, It.IsAny<string>()))
                .Returns($"{{ \"{EventType.ExposureNotified}\": {(int)SendEventLogState.Disable} }}");

            var sendEventLogStateRepository = CreateRepository();

            Assert.False(sendEventLogStateRepository.IsExistNotSetEventType());
        }

        [Fact]
        public void IsExistNotSetEventTypeTest_Enabled()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.SendEventLogState)).Returns(true);

            mockPreferencesService.Setup(s => s.GetStringValue(PreferenceKey.SendEventLogState, It.IsAny<string>()))
                .Returns($"{{ \"{EventType.ExposureNotified}\": {(int)SendEventLogState.Enable} }}");

            var sendEventLogStateRepository = CreateRepository();

            Assert.False(sendEventLogStateRepository.IsExistNotSetEventType());
        }
    }
}
