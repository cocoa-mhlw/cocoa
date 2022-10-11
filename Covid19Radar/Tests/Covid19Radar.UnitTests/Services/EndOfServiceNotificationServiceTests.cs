// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class EndOfServiceNotificationServiceTests
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<ILoggerService> _mockLoggerService;
        private readonly Mock<ILocalNotificationService> _mockLocalNotificationService;
        private readonly Mock<IDateTimeUtility> _mockDateTimeUtility;
        private readonly Mock<IUserDataRepository> _mockUserDataRepository;

        public EndOfServiceNotificationServiceTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockLoggerService = _mockRepository.Create<ILoggerService>();
            _mockLocalNotificationService = _mockRepository.Create<ILocalNotificationService>();
            _mockDateTimeUtility = _mockRepository.Create<IDateTimeUtility>();
            _mockUserDataRepository = _mockRepository.Create<IUserDataRepository>();
        }

        public EndOfServiceNotificationService CreateService()
        {
            return new EndOfServiceNotificationService(
                _mockLoggerService.Object,
                _mockLocalNotificationService.Object,
                _mockDateTimeUtility.Object,
                _mockUserDataRepository.Object
                );
        }

        [Fact]
        public async Task ShowNotificationAsyncTests_NotAgree()
        {
            DateTime testNowUtc = DateTime.UtcNow;
            _mockDateTimeUtility.SetupGet(x => x.UtcNow).Returns(testNowUtc);

            _mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(false);

            EndOfServiceNotificationService unitUnderTest = CreateService();
            await unitUnderTest.ShowNotificationAsync();

            _mockLocalNotificationService.Verify(x => x.ShowEndOfServiceNoticationAsync(), Times.Never());
        }

        [Fact]
        public async Task ShowNotificationAsyncTests_SurveyEnded()
        {
            DateTime testNowUtc = new DateTimeOffset(2023, 1, 1, 0, 0, 0, new TimeSpan(9, 0, 0)).UtcDateTime;
            _mockDateTimeUtility.SetupGet(x => x.UtcNow).Returns(testNowUtc);

            _mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);

            EndOfServiceNotificationService unitUnderTest = CreateService();
            await unitUnderTest.ShowNotificationAsync();

            _mockLoggerService.Verify(x => x.Info("No notification (Survey end)", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _mockLocalNotificationService.Verify(x => x.ShowEndOfServiceNoticationAsync(), Times.Never());
        }

        [Fact]
        public async Task ShowNotificationAsyncTests_SurveyNotEnded()
        {
            DateTime testNowUtc = new DateTimeOffset(2022, 12, 31, 23, 59, 59, new TimeSpan(9, 0, 0)).UtcDateTime;
            _mockDateTimeUtility.SetupGet(x => x.UtcNow).Returns(testNowUtc);

            _mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);

            EndOfServiceNotificationService unitUnderTest = CreateService();
            await unitUnderTest.ShowNotificationAsync();

            _mockLoggerService.Verify(x => x.Info("No notification (Survey end)", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _mockLocalNotificationService.Verify(x => x.ShowEndOfServiceNoticationAsync(), Times.Never());
        }

        [Fact]
        public async Task ShowNotificationAsyncTests_ToManyNotifications()
        {
            DateTime testNowUtc = new DateTimeOffset(2022, 12, 31, 23, 59, 59, new TimeSpan(9, 0, 0)).UtcDateTime;
            _mockDateTimeUtility.SetupGet(x => x.UtcNow).Returns(testNowUtc);

            _mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationCount()).Returns(2);

            EndOfServiceNotificationService unitUnderTest = CreateService();
            await unitUnderTest.ShowNotificationAsync();

            _mockLoggerService.Verify(x => x.Info("No notification (Max notifications)", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _mockLocalNotificationService.Verify(x => x.ShowEndOfServiceNoticationAsync(), Times.Never());
        }

        [Fact]
        public async Task ShowNotificationAsyncTests_NotToManyNotifications()
        {
            DateTime testNowUtc = new DateTimeOffset(2022, 12, 31, 23, 59, 59, new TimeSpan(9, 0, 0)).UtcDateTime;
            _mockDateTimeUtility.SetupGet(x => x.UtcNow).Returns(testNowUtc);

            _mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationCount()).Returns(1);

            EndOfServiceNotificationService unitUnderTest = CreateService();
            await unitUnderTest.ShowNotificationAsync();

            _mockLoggerService.Verify(x => x.Info("No notification (Max notifications)", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _mockLocalNotificationService.Verify(x => x.ShowEndOfServiceNoticationAsync(), Times.Never());
        }

        [Fact]
        public async Task ShowNotificationAsyncTests_FirstSchedule()
        {
            DateTime testNowUtc = new DateTimeOffset(2022, 12, 31, 23, 59, 59, new TimeSpan(9, 0, 0)).UtcDateTime;
            _mockDateTimeUtility.SetupGet(x => x.UtcNow).Returns(testNowUtc);

            _mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationCount()).Returns(0);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationNextSchedule()).Returns((DateTime?)null);

            EndOfServiceNotificationService unitUnderTest = CreateService();
            await unitUnderTest.ShowNotificationAsync();

            _mockUserDataRepository.Verify(x => x.SetEndOfServiceNotificationNextSchedule(It.Is<DateTime>(x => x.ToUnixEpoch() >= 1672704000 && x.ToUnixEpoch() <= 1672747199)), Times.Once());
            _mockLoggerService.Verify(x => x.Info(It.Is<string>(x => x.StartsWith("No notification (First schedule)")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _mockLocalNotificationService.Verify(x => x.ShowEndOfServiceNoticationAsync(), Times.Never());
        }

        [Fact]
        public async Task ShowNotificationAsyncTests_NotFirstSchedule()
        {
            DateTime testNowUtc = new DateTimeOffset(2022, 10, 1, 12, 0, 0, new TimeSpan(9, 0, 0)).UtcDateTime;
            DateTime testSchedule = new DateTimeOffset(2022, 10, 3, 12, 0, 0, new TimeSpan(9, 0, 0)).UtcDateTime;

            _mockDateTimeUtility.SetupGet(x => x.UtcNow).Returns(testNowUtc);

            _mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationCount()).Returns(0);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationNextSchedule()).Returns(testSchedule);

            EndOfServiceNotificationService unitUnderTest = CreateService();
            await unitUnderTest.ShowNotificationAsync();

            _mockUserDataRepository.Verify(x => x.SetEndOfServiceNotificationNextSchedule(It.IsAny<DateTime>()), Times.Never());
            _mockLoggerService.Verify(x => x.Info(It.Is<string>(x => x.StartsWith("No notification (First schedule)")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _mockLocalNotificationService.Verify(x => x.ShowEndOfServiceNoticationAsync(), Times.Never());
        }

        [Fact]
        public async Task ShowNotificationAsyncTests_BeforeSchedule()
        {
            DateTime testNowUtc = new DateTimeOffset(2022, 10, 2, 23, 59, 59, new TimeSpan(9, 0, 0)).UtcDateTime;
            DateTime testSchedule = new DateTimeOffset(2022, 10, 3, 0, 0, 0, new TimeSpan(9, 0, 0)).UtcDateTime;

            _mockDateTimeUtility.SetupGet(x => x.UtcNow).Returns(testNowUtc);

            _mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationCount()).Returns(0);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationNextSchedule()).Returns(testSchedule);

            EndOfServiceNotificationService unitUnderTest = CreateService();
            await unitUnderTest.ShowNotificationAsync();

            _mockLoggerService.Verify(x => x.Info("No notification (Before schedule)", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _mockLocalNotificationService.Verify(x => x.ShowEndOfServiceNoticationAsync(), Times.Never());
        }

        [Fact]
        public async Task ShowNotificationAsyncTests_NotBeforeSchedule()
        {
            DateTime testNowUtc = new DateTimeOffset(2022, 10, 3, 0, 0, 0, new TimeSpan(9, 0, 0)).UtcDateTime;
            DateTime testSchedule = new DateTimeOffset(2022, 10, 3, 0, 0, 0, new TimeSpan(9, 0, 0)).UtcDateTime;

            _mockDateTimeUtility.SetupGet(x => x.UtcNow).Returns(testNowUtc);

            _mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationCount()).Returns(0);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationNextSchedule()).Returns(testSchedule);

            EndOfServiceNotificationService unitUnderTest = CreateService();
            await unitUnderTest.ShowNotificationAsync();

            _mockLoggerService.Verify(x => x.Info("No notification (Before schedule)", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _mockLocalNotificationService.Verify(x => x.ShowEndOfServiceNoticationAsync(), Times.Never());
        }

        [Theory]
        [InlineData(8, 59, 59)]
        [InlineData(21, 0, 0)]
        public async Task ShowNotificationAsyncTests_OffHours(int testHours, int testMinutes, int testSeconds)
        {
            DateTime testNowUtc = new DateTimeOffset(2022, 10, 3, testHours, testMinutes, testSeconds, new TimeSpan(9, 0, 0)).UtcDateTime;
            DateTime testSchedule = new DateTimeOffset(2022, 10, 2, 20, 59, 59, new TimeSpan(9, 0, 0)).UtcDateTime;

            _mockDateTimeUtility.SetupGet(x => x.UtcNow).Returns(testNowUtc);

            _mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationCount()).Returns(0);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationNextSchedule()).Returns(testSchedule);

            EndOfServiceNotificationService unitUnderTest = CreateService();
            await unitUnderTest.ShowNotificationAsync();

            _mockLoggerService.Verify(x => x.Info("No notification (Off hours)", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _mockLocalNotificationService.Verify(x => x.ShowEndOfServiceNoticationAsync(), Times.Never());
        }

        [Theory]
        [InlineData(9, 0, 0)]
        [InlineData(20, 59, 59)]
        public async Task ShowNotificationAsyncTests_NotOffHours(int testHours, int testMinutes, int testSeconds)
        {
            DateTime testNowUtc = new DateTimeOffset(2022, 10, 3, testHours, testMinutes, testSeconds, new TimeSpan(9, 0, 0)).UtcDateTime;
            DateTime testSchedule = new DateTimeOffset(2022, 10, 2, 20, 59, 59, new TimeSpan(9, 0, 0)).UtcDateTime;

            _mockDateTimeUtility.SetupGet(x => x.UtcNow).Returns(testNowUtc);

            _mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationCount()).Returns(0);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationNextSchedule()).Returns(testSchedule);

            EndOfServiceNotificationService unitUnderTest = CreateService();
            await unitUnderTest.ShowNotificationAsync();

            _mockLoggerService.Verify(x => x.Info("No notification (Off hours)", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            _mockLocalNotificationService.Verify(x => x.ShowEndOfServiceNoticationAsync(), Times.Once());
        }

        [Fact]
        public async Task ShowNotificationAsyncTests_NextSchedule()
        {
            DateTime testNowUtc = new DateTimeOffset(2022, 10, 3, 9, 0, 0, new TimeSpan(9, 0, 0)).UtcDateTime;
            DateTime testSchedule = new DateTimeOffset(2022, 10, 2, 20, 59, 59, new TimeSpan(9, 0, 0)).UtcDateTime;

            _mockDateTimeUtility.SetupGet(x => x.UtcNow).Returns(testNowUtc);

            _mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationCount()).Returns(0);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationNextSchedule()).Returns(testSchedule);

            EndOfServiceNotificationService unitUnderTest = CreateService();
            await unitUnderTest.ShowNotificationAsync();

            _mockUserDataRepository.Verify(x => x.SetEndOfServiceNotificationCount(1), Times.Once());
            _mockUserDataRepository.Verify(x => x.SetEndOfServiceNotificationNextSchedule(It.Is<DateTime>(x => x.ToUnixEpoch() >= 1665014400 && x.ToUnixEpoch() <= 1665057599)), Times.Once());
            _mockLoggerService.Verify(x => x.Info(It.Is<string>(x => x.StartsWith("Set next schedule.")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());

            _mockLocalNotificationService.Verify(x => x.ShowEndOfServiceNoticationAsync(), Times.Once());
        }

        [Fact]
        public async Task ShowNotificationAsyncTests_NotNextSchedule()
        {
            DateTime testNowUtc = new DateTimeOffset(2022, 10, 3, 9, 0, 0, new TimeSpan(9, 0, 0)).UtcDateTime;
            DateTime testSchedule = new DateTimeOffset(2022, 10, 2, 20, 59, 59, new TimeSpan(9, 0, 0)).UtcDateTime;

            _mockDateTimeUtility.SetupGet(x => x.UtcNow).Returns(testNowUtc);

            _mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationCount()).Returns(1);
            _mockUserDataRepository.Setup(x => x.GetEndOfServiceNotificationNextSchedule()).Returns(testSchedule);

            EndOfServiceNotificationService unitUnderTest = CreateService();
            await unitUnderTest.ShowNotificationAsync();

            _mockUserDataRepository.Verify(x => x.SetEndOfServiceNotificationCount(2), Times.Once());
            _mockUserDataRepository.Verify(x => x.SetEndOfServiceNotificationNextSchedule(It.IsAny<DateTime>()), Times.Never());
            _mockLoggerService.Verify(x => x.Info(It.Is<string>(x => x.StartsWith("Set next schedule.")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());

            _mockLocalNotificationService.Verify(x => x.ShowEndOfServiceNoticationAsync(), Times.Once());
        }
    }
}

