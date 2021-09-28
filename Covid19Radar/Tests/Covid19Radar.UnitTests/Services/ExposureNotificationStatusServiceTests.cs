/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class ExposureNotificationStatusServiceTests
    {
        #region Test Methods

        #region ExposureNotificationStoppedReason

        [Fact]
        public void ExposureNotificationStoppedReasonTest_InitialValue()
        {
            var mockILoggerService = CreateDefaultMockILoggerService();
            var mockIUserDataRepository = CreateDefaultMockIUserDataRepository();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var mockIExposureNotificationStatusPlatformService = CreateDefaultMockIExposureNotificationStatusPlatformService();

            mockIUserDataRepository.Setup(s => s.GetLastConfirmedDate()).Returns<DateTime?>(null);

            var exposureNotificationStatusService =
                CreateDefaultExposureNotificationStatusService(
                    mockILoggerService.Object,
                    mockIEssentialsService.Object,
                    mockIExposureNotificationStatusPlatformService.Object,
                    mockIUserDataRepository.Object
                    );

            var actualReason = exposureNotificationStatusService.ExposureNotificationStoppedReason;
            Assert.Equal(ExposureNotificationStoppedReason.NotStopping, actualReason);
        }

        [Theory]
        [InlineData("10.0", true, true, true, ExposureNotificationStoppedReason.NotStopping)]
        [InlineData("10.0", true, true, false, ExposureNotificationStoppedReason.ExposureNotificationOff)]
        [InlineData("10.0", true, false, false, ExposureNotificationStoppedReason.BluetoothOff)]
        [InlineData("10.0", false, false, false, ExposureNotificationStoppedReason.GpsOff)]
        [InlineData("10.0", false, true, false, ExposureNotificationStoppedReason.GpsOff)]
        [InlineData("11.0", true, true, true, ExposureNotificationStoppedReason.NotStopping)]
        [InlineData("11.0", true, true, false, ExposureNotificationStoppedReason.ExposureNotificationOff)]
        [InlineData("11.0", true, false, false, ExposureNotificationStoppedReason.BluetoothOff)]
        [InlineData("11.0", false, false, false, ExposureNotificationStoppedReason.BluetoothOff)]
        [InlineData("11.0", false, true, false, ExposureNotificationStoppedReason.ExposureNotificationOff)]
        [InlineData("11.0", false, true, true, ExposureNotificationStoppedReason.NotStopping)]
        public async Task ExposureNotificationStoppedReasonTest_Android(
            string deviceVersion,
            bool gpsEnabled, bool bluetoothEnabled, bool exposureNotificationEnabled, ExposureNotificationStoppedReason expectedReason)
        {
            var mockILoggerService = CreateDefaultMockILoggerService();
            var mockIUserDataRepository = CreateDefaultMockIUserDataRepository();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var mockIExposureNotificationStatusPlatformService = CreateDefaultMockIExposureNotificationStatusPlatformService();
            var exposureNotificationStatusService =
                CreateDefaultExposureNotificationStatusService(
                    mockILoggerService.Object,
                    mockIEssentialsService.Object,
                    mockIExposureNotificationStatusPlatformService.Object,
                    mockIUserDataRepository.Object
                    );

            mockIEssentialsService.Reset();
            mockIEssentialsService.Setup(s => s.IsAndroid).Returns(true);
            mockIEssentialsService.Setup(s => s.IsIos).Returns(false);
            mockIEssentialsService.Setup(s => s.DeviceVersion).Returns(new Version(deviceVersion));

            mockIExposureNotificationStatusPlatformService.Reset();
            mockIExposureNotificationStatusPlatformService.Setup(s => s.GetGpsEnabledAsync()).ReturnsAsync(gpsEnabled);
            mockIExposureNotificationStatusPlatformService.Setup(s => s.GetBluetoothEnabledAsync()).ReturnsAsync(bluetoothEnabled);
            mockIExposureNotificationStatusPlatformService.Setup(s => s.GetExposureNotificationEnabledAsync()).ReturnsAsync(exposureNotificationEnabled);

            mockILoggerService.Reset();

            await exposureNotificationStatusService.UpdateStatuses();
            var actualReason = exposureNotificationStatusService.ExposureNotificationStoppedReason;
            Assert.Equal(expectedReason, actualReason);
        }

        [Theory]
        [InlineData(true, true, ExposureNotificationStoppedReason.NotStopping)]
        [InlineData(true, false, ExposureNotificationStoppedReason.ExposureNotificationOff)]
        [InlineData(false, false, ExposureNotificationStoppedReason.BluetoothOff)]
        public async Task ExposureNotificationStoppedReasonTest_Ios(
            bool bluetoothEnabled, bool exposureNotificationEnabled, ExposureNotificationStoppedReason expectedReason)
        {
            var mockILoggerService = CreateDefaultMockILoggerService();
            var mockIUserDataRepository = CreateDefaultMockIUserDataRepository();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var mockIExposureNotificationStatusPlatformService = CreateDefaultMockIExposureNotificationStatusPlatformService();
            var exposureNotificationStatusService =
                CreateDefaultExposureNotificationStatusService(
                    mockILoggerService.Object,
                    mockIEssentialsService.Object,
                    mockIExposureNotificationStatusPlatformService.Object,
                    mockIUserDataRepository.Object
                    );

            mockIEssentialsService.Reset();
            mockIEssentialsService.Setup(s => s.IsAndroid).Returns(false);
            mockIEssentialsService.Setup(s => s.IsIos).Returns(true);
            mockIEssentialsService.Setup(s => s.DeviceVersion).Returns(new Version("14.7.1"));

            mockIExposureNotificationStatusPlatformService.Reset();
            mockIExposureNotificationStatusPlatformService.Setup(s => s.GetGpsEnabledAsync()).ReturnsAsync(true);
            mockIExposureNotificationStatusPlatformService.Setup(s => s.GetBluetoothEnabledAsync()).ReturnsAsync(bluetoothEnabled);
            mockIExposureNotificationStatusPlatformService.Setup(s => s.GetExposureNotificationEnabledAsync()).ReturnsAsync(exposureNotificationEnabled);

            mockILoggerService.Reset();

            await exposureNotificationStatusService.UpdateStatuses();
            var actualReason = exposureNotificationStatusService.ExposureNotificationStoppedReason;
            Assert.Equal(expectedReason, actualReason);
        }

        #endregion

        #region UpdateStatuses()

        [Theory]
        [InlineData("10.0", true, true, true, 1, 1, 1)]
        [InlineData("10.0", false, true, false, 1, 1, 1)]
        [InlineData("10.0", true, false, false, 1, 1, 1)]
        [InlineData("10.0", false, false, false, 1, 1, 1)]
        [InlineData("11.0", true, true, true, 0, 1, 1)]
        [InlineData("11.0", true, true, false, 0, 1, 1)]
        [InlineData("11.0", true, false, false, 0, 1, 1)]
        public async Task UpdateStatusesTestAsyncTest_Android(
            string deviceVersion,
            bool gpsEnabled, bool bluetoothEnabled, bool exposureNotificationEnabled,
            int gpsEnabledCalls, int bluetoothEnabledCalls, int exposureNotificationEnabledCalls)
        {
            var mockILoggerService = CreateDefaultMockILoggerService();
            var mockIUserDataRepository = CreateDefaultMockIUserDataRepository();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var mockIExposureNotificationStatusPlatformService = CreateDefaultMockIExposureNotificationStatusPlatformService();
            var exposureNotificationStatusService =
                CreateDefaultExposureNotificationStatusService(
                    mockILoggerService.Object,
                    mockIEssentialsService.Object,
                    mockIExposureNotificationStatusPlatformService.Object,
                    mockIUserDataRepository.Object
                    );

            mockIEssentialsService.Reset();
            mockIEssentialsService.Setup(s => s.IsAndroid).Returns(true);
            mockIEssentialsService.Setup(s => s.IsIos).Returns(false);
            mockIEssentialsService.Setup(s => s.DeviceVersion).Returns(new Version(deviceVersion));

            mockIExposureNotificationStatusPlatformService.Reset();
            mockIExposureNotificationStatusPlatformService.Setup(s => s.GetGpsEnabledAsync()).ReturnsAsync(gpsEnabled);
            mockIExposureNotificationStatusPlatformService.Setup(s => s.GetBluetoothEnabledAsync()).ReturnsAsync(bluetoothEnabled);
            mockIExposureNotificationStatusPlatformService.Setup(s => s.GetExposureNotificationEnabledAsync()).ReturnsAsync(exposureNotificationEnabled);

            mockILoggerService.Reset();

            await exposureNotificationStatusService.UpdateStatuses();

            mockIExposureNotificationStatusPlatformService.Verify(s => s.GetGpsEnabledAsync(), Times.Exactly(gpsEnabledCalls));
            mockIExposureNotificationStatusPlatformService.Verify(s => s.GetBluetoothEnabledAsync(), Times.Exactly(bluetoothEnabledCalls));
            mockIExposureNotificationStatusPlatformService.Verify(s => s.GetExposureNotificationEnabledAsync(), Times.Exactly(exposureNotificationEnabledCalls));

            mockILoggerService.Verify(s => s.StartMethod("UpdateStatuses", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockILoggerService.Verify(s => s.EndMethod("UpdateStatuses", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Theory]
        [InlineData(true, true, 1, 1)]
        [InlineData(true, false, 1, 1)]
        [InlineData(false, false, 1, 1)]
        public async Task UpdateStatusesTestAsyncTest_Ios(
            bool bluetoothEnabled, bool exposureNotificationEnabled,
            int bluetoothEnabledCalls, int exposureNotificationEnabledCalls)
        {
            var mockILoggerService = CreateDefaultMockILoggerService();
            var mockIUserDataRepository = CreateDefaultMockIUserDataRepository();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var mockIExposureNotificationStatusPlatformService = CreateDefaultMockIExposureNotificationStatusPlatformService();
            var exposureNotificationStatusService =
                CreateDefaultExposureNotificationStatusService(
                    mockILoggerService.Object,
                    mockIEssentialsService.Object,
                    mockIExposureNotificationStatusPlatformService.Object,
                    mockIUserDataRepository.Object
                    );

            mockIEssentialsService.Reset();
            mockIEssentialsService.Setup(s => s.IsAndroid).Returns(false);
            mockIEssentialsService.Setup(s => s.IsIos).Returns(true);
            mockIEssentialsService.Setup(s => s.DeviceVersion).Returns(new Version("14.7.1"));

            mockIExposureNotificationStatusPlatformService.Reset();
            mockIExposureNotificationStatusPlatformService.Setup(s => s.GetGpsEnabledAsync()).ReturnsAsync(true);
            mockIExposureNotificationStatusPlatformService.Setup(s => s.GetBluetoothEnabledAsync()).ReturnsAsync(bluetoothEnabled);
            mockIExposureNotificationStatusPlatformService.Setup(s => s.GetExposureNotificationEnabledAsync()).ReturnsAsync(exposureNotificationEnabled);

            mockILoggerService.Reset();

            await exposureNotificationStatusService.UpdateStatuses();

            mockIExposureNotificationStatusPlatformService.Verify(s => s.GetGpsEnabledAsync(), Times.Never);
            mockIExposureNotificationStatusPlatformService.Verify(s => s.GetBluetoothEnabledAsync(), Times.Exactly(bluetoothEnabledCalls));
            mockIExposureNotificationStatusPlatformService.Verify(s => s.GetExposureNotificationEnabledAsync(), Times.Exactly(exposureNotificationEnabledCalls));

            mockILoggerService.Verify(s => s.StartMethod("UpdateStatuses", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockILoggerService.Verify(s => s.EndMethod("UpdateStatuses", It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        #endregion

        #endregion

        #region Other Private Methods

        private ExposureNotificationStatusService CreateDefaultExposureNotificationStatusService(
            ILoggerService loggerService,
            IEssentialsService essentialsService,
            IExposureNotificationStatusPlatformService exposureNotificationStatusPlatformService,
            IUserDataRepository userDataRepository
            )
        {
            return new ExposureNotificationStatusService(
                loggerService,
                essentialsService,
                exposureNotificationStatusPlatformService,
                userDataRepository
                );
        }

        private Mock<ILoggerService> CreateDefaultMockILoggerService()
        {
            var mock = new Mock<ILoggerService>();
            mock.Setup(s => s.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
            mock.Setup(s => s.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
            mock.Setup(s => s.Verbose(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
            mock.Setup(s => s.Debug(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
            mock.Setup(s => s.Info(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
            mock.Setup(s => s.Warning(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
            mock.Setup(s => s.Error(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
            mock.Setup(s => s.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

            return mock;
        }

        private Mock<IUserDataRepository> CreateDefaultMockIUserDataRepository()
        {
            var mock = new Mock<IUserDataRepository>();
            //mock.Setup(s => s.GetDateTime(PreferenceKey.LastConfirmedUtcDateTime)).Returns(DateTime.UtcNow);
            //mock.Setup(s => s.GetValue(PreferenceKey.CanConfirmExposure, It.IsAny<bool>())).Returns(true);
            //mock.Setup(s => s.SetValue(PreferenceKey.LastConfirmedUtcDateTime, It.IsAny<DateTime>()));
            //mock.Setup(s => s.SetValue(PreferenceKey.CanConfirmExposure, It.IsAny<bool>()));
            //mock.Setup(s => s.RemoveValue(PreferenceKey.LastConfirmedUtcDateTime));
            //mock.Setup(s => s.RemoveValue(PreferenceKey.CanConfirmExposure));
            //mock.Setup(s => s.ContainsKey(PreferenceKey.LastConfirmedUtcDateTime)).Returns(false);
            //mock.Setup(s => s.ContainsKey(PreferenceKey.CanConfirmExposure)).Returns(false);

            return mock;
        }

        private Mock<IEssentialsService> CreateDefaultMockIEssentialsService()
        {
            var mock = new Mock<IEssentialsService>();
            mock.Setup(s => s.IsAndroid).Returns(true);
            mock.Setup(s => s.IsIos).Returns(false);
            mock.Setup(s => s.DeviceVersion).Returns(new Version("10.0"));
            return mock;
        }

        private Mock<IExposureNotificationStatusPlatformService> CreateDefaultMockIExposureNotificationStatusPlatformService()
        {
            var mock = new Mock<IExposureNotificationStatusPlatformService>();
            mock.Setup(s => s.GetExposureNotificationEnabledAsync()).ReturnsAsync(true);
            mock.Setup(s => s.GetGpsEnabledAsync()).ReturnsAsync(true);
            mock.Setup(s => s.GetBluetoothEnabledAsync()).ReturnsAsync(true);
            return mock;
        }

        #endregion
    }
}
