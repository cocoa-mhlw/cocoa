/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Repository
{
    public class UserDataRepositoryTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IPreferencesService> mockPreferencesService;

        public UserDataRepositoryTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockPreferencesService = mockRepository.Create<IPreferencesService>();
        }

        private IUserDataRepository CreateRepository()
            => new UserDataRepository(mockPreferencesService.Object, mockLoggerService.Object);

        #region LastConfirmedUtcDateTime

        [Fact]
        public void LastConfirmedUtcDateTimeTest_NotExists()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.LastConfirmedDateTimeEpoch)).Returns(false);

            var userDataRepository = CreateRepository();

            var lastConfirmedUtcDateTime = userDataRepository.GetLastConfirmedDate();

            Assert.Null(lastConfirmedUtcDateTime);
        }

        [Fact]
        public void LastConfirmedUtcDateTimeTest_Exists()
        {
            mockPreferencesService.Setup(s => s.ContainsKey(PreferenceKey.LastConfirmedDateTimeEpoch)).Returns(true);
            mockPreferencesService.Setup(s => s.GetValue(PreferenceKey.LastConfirmedDateTimeEpoch, 0L)).Returns(800);

            var userDataRepository = CreateRepository();

            var lastConfirmedUtcDateTime = userDataRepository.GetLastConfirmedDate();

            var expoectedDateTime = DateTimeOffset.FromUnixTimeSeconds(800).DateTime;

            Assert.NotNull(lastConfirmedUtcDateTime);
            Assert.Equal(expoectedDateTime, lastConfirmedUtcDateTime);
        }

        #endregion

        #region RemoveAllExposureNotificationStatus()

        [Fact]
        public void RemoveAllExposureNotificationStatusTest()
        {
            var userDataRepository = CreateRepository();

            userDataRepository.RemoveAllExposureNotificationStatus();

            mockPreferencesService.Verify(s => s.RemoveValue(PreferenceKey.LastConfirmedDateTimeEpoch), Times.Once());
            mockPreferencesService.Verify(s => s.RemoveValue(PreferenceKey.CanConfirmExposure), Times.Once());
        }

        #endregion

    }
}
