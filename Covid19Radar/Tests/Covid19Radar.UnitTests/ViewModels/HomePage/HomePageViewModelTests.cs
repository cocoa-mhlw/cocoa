/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */


using System;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Moq;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class HomePageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IUserDataRepository> mockUserDataRepository;
        private readonly Mock<IExposureNotificationService> mockExposureNotificationService;
        private readonly Mock<ILocalNotificationService> mockLocalNotificationService;
        private readonly Mock<IExposureNotificationStatusService> mockExposureNotificationStatusService;
        private readonly Mock<IDialogService> mockDialogService;
        private readonly Mock<IExternalNavigationService> mockExternalNavigationService;

        public HomePageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockUserDataRepository = mockRepository.Create<IUserDataRepository>();
            mockExposureNotificationService = mockRepository.Create<IExposureNotificationService>();
            mockLocalNotificationService = mockRepository.Create<ILocalNotificationService>();
            mockExposureNotificationStatusService = mockRepository.Create<IExposureNotificationStatusService>();
            mockDialogService = mockRepository.Create<IDialogService>();
            mockExternalNavigationService = mockRepository.Create<IExternalNavigationService>();
        }

        private HomePageViewModel CreateViewModel()
        {
            return new HomePageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                mockUserDataRepository.Object,
                mockExposureNotificationService.Object,
                mockLocalNotificationService.Object,
                mockExposureNotificationStatusService.Object,
                mockDialogService.Object,
                mockExternalNavigationService.Object);
        }

        [Theory]
        [InlineData(ExposureNotificationStatus.Unconfirmed, false, true, false)]
        [InlineData(ExposureNotificationStatus.Stopped, false, false, true)]
        public void UpdateView_ENStatus_Unconfirmed_Stopped(ExposureNotificationStatus enStatus, bool isVisibleActiveLayoutResult, bool isVisibleUnconfirmedLayoutResult, bool isVisibleStoppedLayoutResult)
        {
            var homePageViewModel = CreateViewModel();            

            mockExposureNotificationStatusService.Setup(x => x.ExposureNotificationStatus).Returns(enStatus);

            homePageViewModel.OnAppearing();

            Assert.Equal(isVisibleActiveLayoutResult, homePageViewModel.IsVisibleENStatusActiveLayout);
            Assert.Equal(isVisibleUnconfirmedLayoutResult, homePageViewModel.IsVisibleENStatusUnconfirmedLayout);
            Assert.Equal(isVisibleStoppedLayoutResult, homePageViewModel.IsVisibleENStatusStoppedLayout);
        }

        [Fact]
        public void UpdateView_ENStatus_Active_LatestConfirmationDate_null()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationStatusService.Setup(x => x.ExposureNotificationStatus).Returns(ExposureNotificationStatus.Active);

            homePageViewModel.OnAppearing();

            Assert.True(homePageViewModel.IsVisibleENStatusActiveLayout);
            Assert.False(homePageViewModel.IsVisibleENStatusUnconfirmedLayout);
            Assert.False(homePageViewModel.IsVisibleENStatusStoppedLayout);
            Assert.Equal(AppResources.InProgressText, homePageViewModel.LatestConfirmationDate);
        }

        [Fact]
        public void UpdateView_ENStatus_Active()
        {
            var homePageViewModel = CreateViewModel();

            var mockLastConfirmedUtcDateTime = DateTime.UtcNow;
            mockExposureNotificationStatusService.Setup(x => x.ExposureNotificationStatus).Returns(ExposureNotificationStatus.Active);
            mockUserDataRepository.Setup(x => x.GetLastConfirmedDate()).Returns(mockLastConfirmedUtcDateTime);

            homePageViewModel.OnAppearing();

            Assert.True(homePageViewModel.IsVisibleENStatusActiveLayout);
            Assert.False(homePageViewModel.IsVisibleENStatusUnconfirmedLayout);
            Assert.False(homePageViewModel.IsVisibleENStatusStoppedLayout);

            var mockLatestConfirmationDate = mockLastConfirmedUtcDateTime.ToLocalTime().ToString(AppResources.DateTimeFormatToDisplayOnHomePage);
            Assert.Equal(mockLatestConfirmationDate, homePageViewModel.LatestConfirmationDate);
        }

        [Fact]
        public void OnClickCheckStopReasonCommandTest_StoppedReason_ExposureNotificationOff_OK()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationStatusService.Setup(x => x.ExposureNotificationStoppedReason).Returns(ExposureNotificationStoppedReason.ExposureNotificationOff);
            mockDialogService.Setup(x => x.ShowExposureNotificationOffWarningAsync()).ReturnsAsync(true);

            homePageViewModel.OnClickCheckStopReason.Execute(null);

            mockDialogService.Verify(x => x.ShowExposureNotificationOffWarningAsync(), Times.Once());
            mockExternalNavigationService.Verify(x => x.NavigateAppSettings(), Times.Once());
        }

        [Fact]
        public void OnClickCheckStopReasonCommandTest_StoppedReason_ExposureNotificationOff_Cancel()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationStatusService.Setup(x => x.ExposureNotificationStoppedReason).Returns(ExposureNotificationStoppedReason.ExposureNotificationOff);
            mockDialogService.Setup(x => x.ShowExposureNotificationOffWarningAsync()).ReturnsAsync(false);

            homePageViewModel.OnClickCheckStopReason.Execute(null);

            mockDialogService.Verify(x => x.ShowExposureNotificationOffWarningAsync(), Times.Once());
            mockExternalNavigationService.Verify(x => x.NavigateAppSettings(), Times.Never());
        }

        [Fact]
        public void OnClickCheckStopReasonCommandTest_StoppedReason_BluetoothOff_OK()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationStatusService.Setup(x => x.ExposureNotificationStoppedReason).Returns(ExposureNotificationStoppedReason.BluetoothOff);
            mockDialogService.Setup(x => x.ShowBluetoothOffWarningAsync()).ReturnsAsync(true);

            homePageViewModel.OnClickCheckStopReason.Execute(null);

            mockDialogService.Verify(x => x.ShowBluetoothOffWarningAsync(), Times.Once());
            mockExternalNavigationService.Verify(x => x.NavigateBluetoothSettings(), Times.Once());
        }

        [Fact]
        public void OnClickCheckStopReasonCommandTest_StoppedReason_BluetoothOff_Cancel()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationStatusService.Setup(x => x.ExposureNotificationStoppedReason).Returns(ExposureNotificationStoppedReason.BluetoothOff);
            mockDialogService.Setup(x => x.ShowBluetoothOffWarningAsync()).ReturnsAsync(false);

            homePageViewModel.OnClickCheckStopReason.Execute(null);

            mockDialogService.Verify(x => x.ShowBluetoothOffWarningAsync(), Times.Once());
            mockExternalNavigationService.Verify(x => x.NavigateBluetoothSettings(), Times.Never());
        }

        [Fact]
        public void OnClickCheckStopReasonCommandTest_StoppedReason_GpsOff_OK()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationStatusService.Setup(x => x.ExposureNotificationStoppedReason).Returns(ExposureNotificationStoppedReason.GpsOff);
            mockDialogService.Setup(x => x.ShowLocationOffWarningAsync()).ReturnsAsync(true);

            homePageViewModel.OnClickCheckStopReason.Execute(null);

            mockDialogService.Verify(x => x.ShowLocationOffWarningAsync(), Times.Once());
            mockExternalNavigationService.Verify(x => x.NavigateLocationSettings(), Times.Once());
        }

        [Fact]
        public void OnClickCheckStopReasonCommandTest_StoppedReason_GpsOff_Cancel()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationStatusService.Setup(x => x.ExposureNotificationStoppedReason).Returns(ExposureNotificationStoppedReason.GpsOff);
            mockDialogService.Setup(x => x.ShowLocationOffWarningAsync()).ReturnsAsync(false);

            homePageViewModel.OnClickCheckStopReason.Execute(null);

            mockDialogService.Verify(x => x.ShowLocationOffWarningAsync(), Times.Once());
            mockExternalNavigationService.Verify(x => x.NavigateLocationSettings(), Times.Never());
        }
    }
}
