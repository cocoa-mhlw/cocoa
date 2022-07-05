// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class DialogServiceTests : IDisposable
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<IEssentialsService> mockEssentialsService;
        private readonly Mock<IUserDialogs> mockUserDialogs;

        public DialogServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockEssentialsService = mockRepository.Create<IEssentialsService>();
            mockUserDialogs = mockRepository.Create<IUserDialogs>();
            UserDialogs.Instance = mockUserDialogs.Object;
        }

        public void Dispose()
        {
            UserDialogs.Instance = null;
        }

        private IDialogService CraeteDialogService()
        {
            return new DialogService(mockEssentialsService.Object);
        }

        [Fact]
        public async Task ShowExposureNotificationOffWarningTestAsync_Android()
        {
            mockEssentialsService.Setup(x => x.IsAndroid).Returns(true);
            mockEssentialsService.Setup(x => x.IsIos).Returns(false);

            var unitUnderTest = CraeteDialogService();
            await unitUnderTest.ShowExposureNotificationOffWarningAsync();

            mockUserDialogs.Verify(x => x.ConfirmAsync(
                AppResources.ExposureNotificationOffWarningDialogMessage,
                AppResources.CheckSettingsDialogTitle,
                AppResources.ToSettingsButton,
                AppResources.ButtonCancel,
                null),
                Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
        }

        [Fact]
        public async Task ShowExposureNotificationOffWarningTestAsync_Ios()
        {
            mockEssentialsService.Setup(x => x.IsAndroid).Returns(false);
            mockEssentialsService.Setup(x => x.IsIos).Returns(true);

            var unitUnderTest = CraeteDialogService();
            await unitUnderTest.ShowExposureNotificationOffWarningAsync();

            mockUserDialogs.Verify(x => x.ConfirmAsync(
                AppResources.ExposureNotificationOffWarningDialogMessage,
                AppResources.CheckSettingsDialogTitle,
                AppResources.ToSettingsButton,
                AppResources.ButtonCancel,
                null),
                Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
        }

        [Fact]
        public async Task ShowBluetoothOffWarningTest_AndroidAsync()
        {
            mockEssentialsService.Setup(x => x.IsAndroid).Returns(true);
            mockEssentialsService.Setup(x => x.IsIos).Returns(false);

            var unitUnderTest = CraeteDialogService();
            await unitUnderTest.ShowBluetoothOffWarningAsync();

            mockUserDialogs.Verify(x => x.ConfirmAsync(
                AppResources.BluetoothOffWarningDialogMessage,
                AppResources.CheckSettingsDialogTitle,
                AppResources.ToSettingsButton,
                AppResources.ButtonCancel,
                null),
                Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
        }

        [Fact]
        public async Task ShowBluetoothOffWarningTest_IosAsync()
        {
            mockEssentialsService.Setup(x => x.IsAndroid).Returns(false);
            mockEssentialsService.Setup(x => x.IsIos).Returns(true);

            var unitUnderTest = CraeteDialogService();
            await unitUnderTest.ShowBluetoothOffWarningAsync();

            mockUserDialogs.Verify(x => x.ConfirmAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
            mockUserDialogs.Verify(x => x.AlertAsync(
                AppResources.BluetoothOffWarningDialogMessage,
                AppResources.CheckSettingsDialogTitle,
                AppResources.ButtonClose,
                null),
                Times.Once());
        }

        [Fact]
        public async Task ShowLocationOffWarningTest_AndroidAsync()
        {
            mockEssentialsService.Setup(x => x.IsAndroid).Returns(true);
            mockEssentialsService.Setup(x => x.IsIos).Returns(false);

            var unitUnderTest = CraeteDialogService();
            await unitUnderTest.ShowLocationOffWarningAsync();

            mockUserDialogs.Verify(x => x.ConfirmAsync(
                AppResources.LocationOffWarningDialogMessage,
                AppResources.CheckSettingsDialogTitle,
                AppResources.ToSettingsButton,
                AppResources.ButtonCancel,
                null),
                Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
        }

        [Fact]
        public void ShowLocationOffWarningTest_Ios()
        {
            mockEssentialsService.Setup(x => x.IsAndroid).Returns(false);
            mockEssentialsService.Setup(x => x.IsIos).Returns(true);

            var unitUnderTest = CraeteDialogService();
            Assert.ThrowsAsync<PlatformNotSupportedException>(async () => { await unitUnderTest.ShowLocationOffWarningAsync(); });

            mockUserDialogs.Verify(x => x.ConfirmAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
        }

        [Fact]
        public async Task ShowTemporarilyUnavailableWarningAsyncTest()
        {
            mockEssentialsService.Setup(x => x.IsAndroid).Returns(true);
            mockEssentialsService.Setup(x => x.IsIos).Returns(false);

            var unitUnderTest = CraeteDialogService();
            await unitUnderTest.ShowTemporarilyUnavailableWarningAsync();

            mockUserDialogs.Verify(x => x.ConfirmAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
            mockUserDialogs.Verify(x => x.AlertAsync(
                AppResources.TemporarilyUnavailableWarningMessage,
                AppResources.TemporarilyUnavailableWarningTitle,
                AppResources.ButtonOk,
                null),
                Times.Once());
        }

        [Fact]
        public async Task ShowHomePageUnknownErrorWaringAsyncTest()
        {
            mockEssentialsService.Setup(x => x.IsAndroid).Returns(true);
            mockEssentialsService.Setup(x => x.IsIos).Returns(false);

            var unitUnderTest = CraeteDialogService();
            await unitUnderTest.ShowHomePageUnknownErrorWaringAsync();

            mockUserDialogs.Verify(x => x.ConfirmAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
            mockUserDialogs.Verify(x => x.AlertAsync(
                AppResources.HomePageDialogExceptionDescription,
                AppResources.HomePageDialogExceptionTitle,
                AppResources.ButtonOk,
                null),
                Times.Once());
        }

        [Fact]
        public async Task ShowLocalNotificationOffWarningAsyncTest()
        {
            mockEssentialsService.Setup(x => x.IsAndroid).Returns(true);
            mockEssentialsService.Setup(x => x.IsIos).Returns(false);

            var unitUnderTest = CraeteDialogService();
            await unitUnderTest.ShowLocalNotificationOffWarningAsync();

            mockUserDialogs.Verify(x => x.ConfirmAsync(
                AppResources.LocalNotificationOffWarningDialogMessage,
                AppResources.LocalNotificationOffWarningDialogTitle,
                AppResources.ToSettingsButton,
                AppResources.ButtonCancel,
                null),
                Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
        }

        [Fact]
        public async Task ShowUserProfileNotSupportAsyncTest()
        {
            mockEssentialsService.Setup(x => x.IsAndroid).Returns(true);
            mockEssentialsService.Setup(x => x.IsIos).Returns(false);

            var unitUnderTest = CraeteDialogService();
            await unitUnderTest.ShowUserProfileNotSupportAsync();

            mockUserDialogs.Verify(x => x.ConfirmAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
            mockUserDialogs.Verify(x => x.AlertAsync(
                AppResources.UserProfileNotSupportDialogDescription,
                AppResources.UserProfileNotSupportDialogTitle,
                AppResources.ButtonOk,
                null),
                Times.Once());
        }

        [Fact]
        public async Task ShowEventLogSaveCompletedAsyncTest()
        {
            mockEssentialsService.Setup(x => x.IsAndroid).Returns(true);
            mockEssentialsService.Setup(x => x.IsIos).Returns(false);

            var unitUnderTest = CraeteDialogService();
            await unitUnderTest.ShowEventLogSaveCompletedAsync();

            mockUserDialogs.Verify(x => x.ConfirmAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
            mockUserDialogs.Verify(x => x.AlertAsync(
                "",
                AppResources.EventLogSettingPageSaveCompleteMessageTitle,
                AppResources.ButtonOk,
                null),
                Times.Once());
        }
    }
}
