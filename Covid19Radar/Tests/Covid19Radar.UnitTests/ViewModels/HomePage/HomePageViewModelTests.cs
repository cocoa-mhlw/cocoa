/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Moq;
using Newtonsoft.Json;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class HomePageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IDateTimeUtility> mockDateTimeUtility;
        private readonly Mock<IPreferencesService> mockPreferenceService;
        private readonly UserDataRepository userDataRepository;
        private readonly Mock<AbsExposureNotificationApiService> mockExposureNotificationApiService;
        private readonly Mock<ILocalNotificationService> mockLocalNotificationService;
        private readonly Mock<IServerConfigurationRepository> mockServerConfigurationRepository;
        private readonly Mock<AbsExposureDetectionBackgroundService> mockExposureDetectionBackgroundService;
        private readonly Mock<IDialogService> mockDialogService;
        private readonly Mock<IExternalNavigationService> mockExternalNavigationService;
        private readonly Mock<ILocalPathService> mockLocalPathService;
        private readonly ExposureRiskCalculationService exposureRiskCalculationService;

        public HomePageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockDateTimeUtility = mockRepository.Create<IDateTimeUtility>();
            mockPreferenceService = mockRepository.Create<IPreferencesService>();
            userDataRepository = new UserDataRepository(
                    mockPreferenceService.Object,
                    mockDateTimeUtility.Object,
                    mockLoggerService.Object
                );
            mockExposureNotificationApiService = mockRepository.Create<AbsExposureNotificationApiService>(mockLoggerService.Object);
            mockLocalNotificationService = mockRepository.Create<ILocalNotificationService>();
            mockServerConfigurationRepository = mockRepository.Create<IServerConfigurationRepository>();
            mockLocalPathService = mockRepository.Create<ILocalPathService>();
            mockExposureDetectionBackgroundService = mockRepository.Create<AbsExposureDetectionBackgroundService>(
                mockRepository.Create<IDiagnosisKeyRepository>().Object,
                mockExposureNotificationApiService.Object,
                mockRepository.Create<IExposureConfigurationRepository>().Object,
                mockLoggerService.Object,
                userDataRepository,
                mockServerConfigurationRepository.Object,
                mockLocalPathService.Object
                );
            mockDialogService = mockRepository.Create<IDialogService>();
            mockExternalNavigationService = mockRepository.Create<IExternalNavigationService>();
            exposureRiskCalculationService = new ExposureRiskCalculationService(
                userDataRepository,
                mockLoggerService.Object
                );
        }

        private HomePageViewModel CreateViewModel()
        {
            return new HomePageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                userDataRepository,
                mockExposureNotificationApiService.Object,
                mockLocalNotificationService.Object,
                mockExposureDetectionBackgroundService.Object,
                mockDialogService.Object,
                mockExternalNavigationService.Object,
                exposureRiskCalculationService
                );
        }

        private DailySummary CreateDailySummaryWithDayOffset(DateTime date, int dayOffset)
        {
            return new DailySummary()
            {
                DateMillisSinceEpoch = date.AddDays(dayOffset).ToUnixEpoch() * 1000 // seconds -> milliseconds
            };
        }

        [Fact]
        public void Initialize_CheckExposureNotificationSettings()
        {
            var homePageViewModel = CreateViewModel();
            var parameters = new NavigationParameters();

            homePageViewModel.Initialize(parameters);

            mockLocalNotificationService
                .Verify(x => x.PrepareAsync(), Times.Once);
            mockExposureDetectionBackgroundService
                .Verify(x => x.ExposureDetectionAsync(null), Times.Once);
            mockExposureNotificationApiService
                .Verify(x => x.StartExposureNotificationAsync(), Times.Once);
        }

        [Fact]
        public void OnResume_CheckExposureNotificationSettings()
        {
            mockExposureNotificationApiService
               .Setup(x => x.GetStatusCodesAsync())
               .Returns(Task.FromResult(new List<int>() as IList<int>));

            mockPreferenceService
                .Setup(x => x.GetValue("CanConfirmExposure", true))
                .Returns(true);

            var homePageViewModel = CreateViewModel();
            var parameters = new NavigationParameters();

            homePageViewModel.OnResume();

            mockExposureNotificationApiService
                .Verify(x => x.StartExposureNotificationAsync(), Times.Once);
        }

        [Fact]
        public void OnEnabled_CheckExposureNotificationSettings()
        {
            mockExposureNotificationApiService
               .Setup(x => x.GetStatusCodesAsync())
               .Returns(Task.FromResult(new List<int>() as IList<int>));

            mockPreferenceService
                .Setup(x => x.GetValue("CanConfirmExposure", true))
                .Returns(true);

            var homePageViewModel = CreateViewModel();
            var parameters = new NavigationParameters();

            homePageViewModel.OnEnabled();

            mockExposureNotificationApiService
                .Verify(x => x.StartExposureNotificationAsync(), Times.Once);
        }

        [Fact]
        public void OnDeclined_CheckExposureNotificationSettings()
        {
            mockExposureNotificationApiService
               .Setup(x => x.GetStatusCodesAsync())
               .Returns(Task.FromResult(new List<int>() as IList<int>));

            mockPreferenceService
                .Setup(x => x.GetValue("CanConfirmExposure", true))
                .Returns(true);

            var homePageViewModel = CreateViewModel();
            homePageViewModel.OnDeclined();

            mockExposureNotificationApiService
                .Verify(x => x.StartExposureNotificationAsync(), Times.Never);
        }

        [Theory]
        [InlineData(ExposureNotificationStatus.Code_Android.ACTIVATED, false, false, true, false)]
        [InlineData(ExposureNotificationStatus.Code_Android.INACTIVATED, true, false, false, true)]
        public void UpdateView_ENStatus_Unconfirmed_Stopped(
            int status,
            bool isCanConfirmExposure,
            bool isVisibleActiveLayoutResult,
            bool isVisibleUnconfirmedLayoutResult,
            bool isVisibleStoppedLayoutResult
            )
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationApiService
                .Setup(x => x.GetStatusCodesAsync())
                .Returns(Task.FromResult(new List<int>() { status } as IList<int>));

            mockPreferenceService
                .Setup(x => x.GetValue("CanConfirmExposure", true))
                .Returns(isCanConfirmExposure);

            homePageViewModel.OnAppearing();

            Assert.Equal(isVisibleActiveLayoutResult, homePageViewModel.IsVisibleENStatusActiveLayout);
            Assert.Equal(isVisibleUnconfirmedLayoutResult, homePageViewModel.IsVisibleENStatusUnconfirmedLayout);
            Assert.Equal(isVisibleStoppedLayoutResult, homePageViewModel.IsVisibleENStatusStoppedLayout);
        }

        [Fact]
        public void UpdateView_ENStatus_Active_LatestConfirmationDate_null()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationApiService
                .Setup(x => x.GetStatusCodesAsync())
                .Returns(Task.FromResult(new List<int>() { ExposureNotificationStatus.Code_Android.ACTIVATED } as IList<int>));
            mockPreferenceService
                .Setup(x => x.GetValue("CanConfirmExposure", true))
                .Returns(true);

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
            mockExposureNotificationApiService
                .Setup(x => x.GetStatusCodesAsync()).Returns(Task.FromResult(new List<int>() { ExposureNotificationStatus.Code_Android.ACTIVATED } as IList<int>));
            mockPreferenceService
                .Setup(x => x.ContainsKey("LastConfirmedDateTimeEpoch"))
                .Returns(true);
            mockPreferenceService
                .Setup(x => x.GetValue("LastConfirmedDateTimeEpoch", 0L))
                .Returns(mockLastConfirmedUtcDateTime.ToUnixEpoch());
            mockPreferenceService
                .Setup(x => x.GetValue("CanConfirmExposure", true))
                .Returns(true);

            homePageViewModel.OnAppearing();

            Assert.True(homePageViewModel.IsVisibleENStatusActiveLayout);
            Assert.False(homePageViewModel.IsVisibleENStatusUnconfirmedLayout);
            Assert.False(homePageViewModel.IsVisibleENStatusStoppedLayout);

            var mockLatestConfirmationDate = mockLastConfirmedUtcDateTime.ToLocalTime().ToString(AppResources.DateTimeFormatToDisplayOnHomePage);
            Assert.Equal(mockLatestConfirmationDate, homePageViewModel.LatestConfirmationDate);
        }

        [Fact]
        public void OnClickCheckStopReasonCommandTest_StoppedReason_ExposureNotificationOff_OK_iOS()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationApiService.Setup(x => x.StartExposureNotificationAsync()).Returns(Task.FromResult(true));
            mockExposureNotificationApiService
                .Setup(x => x.GetStatusCodesAsync()).Returns(Task.FromResult(new List<int>() { ExposureNotificationStatus.Code_iOS.Disabled } as IList<int>));
            mockDialogService.Setup(x => x.ShowExposureNotificationOffWarningAsync()).ReturnsAsync(true);
            mockExposureDetectionBackgroundService.Setup(x => x.ExposureDetectionAsync(It.IsAny<CancellationTokenSource>())).Returns(Task.CompletedTask);

            homePageViewModel.OnClickCheckStopReason.Execute(null);

            mockDialogService.Verify(x => x.ShowExposureNotificationOffWarningAsync(), Times.Once());
            mockExternalNavigationService.Verify(x => x.NavigateAppSettings(), Times.Once());
            mockExposureNotificationApiService.Verify(x => x.StartExposureNotificationAsync(), Times.Never());
            mockExposureDetectionBackgroundService.Verify(x => x.ExposureDetectionAsync(It.IsAny<CancellationTokenSource>()), Times.Never());
        }

        [Fact]
        public void OnClickCheckStopReasonCommandTest_StoppedReason_ExposureNotificationOff_OK_Android()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationApiService.Setup(x => x.StartExposureNotificationAsync()).Returns(Task.FromResult(true));
            mockExposureNotificationApiService
                .Setup(x => x.GetStatusCodesAsync()).Returns(Task.FromResult(new List<int>() { ExposureNotificationStatus.Code_Android.INACTIVATED } as IList<int>));
            mockDialogService.Setup(x => x.ShowExposureNotificationOffWarningAsync()).ReturnsAsync(true);
            mockExposureDetectionBackgroundService.Setup(x => x.ExposureDetectionAsync(It.IsAny<CancellationTokenSource>())).Returns(Task.CompletedTask);

            homePageViewModel.OnClickCheckStopReason.Execute(null);

            mockDialogService.Verify(x => x.ShowExposureNotificationOffWarningAsync(), Times.Once());
            mockExternalNavigationService.Verify(x => x.NavigateAppSettings(), Times.Never());
            mockExposureNotificationApiService.Verify(x => x.StartExposureNotificationAsync(), Times.Once());
            mockExposureDetectionBackgroundService.Verify(x => x.ExposureDetectionAsync(It.IsAny<CancellationTokenSource>()), Times.Once());
        }

        [Fact]
        public void OnClickCheckStopReasonCommandTest_StoppedReason_ExposureNotificationOff_Cancel()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationApiService
                .Setup(x => x.GetStatusCodesAsync()).Returns(Task.FromResult(new List<int>() { ExposureNotificationStatus.Code_Android.INACTIVATED } as IList<int>));
            mockDialogService.Setup(x => x.ShowExposureNotificationOffWarningAsync()).ReturnsAsync(false);

            homePageViewModel.OnClickCheckStopReason.Execute(null);

            mockDialogService.Verify(x => x.ShowExposureNotificationOffWarningAsync(), Times.Once());
            mockExternalNavigationService.Verify(x => x.NavigateAppSettings(), Times.Never());
        }

        [Fact]
        public void OnClickCheckStopReasonCommandTest_StoppedReason_BluetoothOff_OK()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationApiService
                .Setup(x => x.GetStatusCodesAsync()).Returns(Task.FromResult(new List<int>() { ExposureNotificationStatus.Code_Android.BLUETOOTH_DISABLED } as IList<int>));
            mockDialogService.Setup(x => x.ShowBluetoothOffWarningAsync()).ReturnsAsync(true);

            homePageViewModel.OnClickCheckStopReason.Execute(null);

            mockDialogService.Verify(x => x.ShowBluetoothOffWarningAsync(), Times.Once());
            mockExternalNavigationService.Verify(x => x.NavigateBluetoothSettings(), Times.Once());
        }

        [Fact]
        public void OnClickCheckStopReasonCommandTest_StoppedReason_BluetoothOff_Cancel()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationApiService
                .Setup(x => x.GetStatusCodesAsync()).Returns(Task.FromResult(new List<int>() { ExposureNotificationStatus.Code_Android.BLUETOOTH_DISABLED } as IList<int>));
            mockDialogService.Setup(x => x.ShowBluetoothOffWarningAsync()).ReturnsAsync(false);

            homePageViewModel.OnClickCheckStopReason.Execute(null);

            mockDialogService.Verify(x => x.ShowBluetoothOffWarningAsync(), Times.Once());
            mockExternalNavigationService.Verify(x => x.NavigateBluetoothSettings(), Times.Never());
        }

        [Fact]
        public void OnClickCheckStopReasonCommandTest_StoppedReason_GpsOff_OK()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationApiService
                .Setup(x => x.GetStatusCodesAsync()).Returns(Task.FromResult(new List<int>() { ExposureNotificationStatus.Code_Android.LOCATION_DISABLED } as IList<int>));
            mockDialogService.Setup(x => x.ShowLocationOffWarningAsync()).ReturnsAsync(true);

            homePageViewModel.OnClickCheckStopReason.Execute(null);

            mockDialogService.Verify(x => x.ShowLocationOffWarningAsync(), Times.Once());
            mockExternalNavigationService.Verify(x => x.NavigateLocationSettings(), Times.Once());
        }

        [Fact]
        public void OnClickCheckStopReasonCommandTest_StoppedReason_GpsOff_Cancel()
        {
            var homePageViewModel = CreateViewModel();

            mockExposureNotificationApiService
                .Setup(x => x.GetStatusCodesAsync()).Returns(Task.FromResult(new List<int>() { ExposureNotificationStatus.Code_Android.LOCATION_DISABLED } as IList<int>));
            mockDialogService.Setup(x => x.ShowLocationOffWarningAsync()).ReturnsAsync(false);

            homePageViewModel.OnClickCheckStopReason.Execute(null);

            mockDialogService.Verify(x => x.ShowLocationOffWarningAsync(), Times.Once());
            mockExternalNavigationService.Verify(x => x.NavigateLocationSettings(), Times.Never());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(-3)]
        [InlineData(-4)]
        [InlineData(-5)]
        [InlineData(-6)]
        [InlineData(-7)]
        [InlineData(-8)]
        [InlineData(-9)]
        [InlineData(-10)]
        [InlineData(-11)]
        [InlineData(-12)]
        [InlineData(-13)]
        [InlineData(-14)]
        public void OnClickExposuresTest_NavigateContactedNotifyPage(int dayOffset)
        {
            var utcNow = DateTime.UtcNow;
            var dailySummaries = new List<DailySummary>()
            {
                CreateDailySummaryWithDayOffset(utcNow, -30),
                CreateDailySummaryWithDayOffset(utcNow, -20),
                CreateDailySummaryWithDayOffset(utcNow, dayOffset)
            };

            var serializeDailySummaries = JsonConvert.SerializeObject(dailySummaries);

            mockDateTimeUtility
                .Setup(x => x.UtcNow)
                .Returns(utcNow);

            mockPreferenceService
                .Setup(x => x.GetValue("DailySummaries", It.IsAny<string>()))
                .Returns(serializeDailySummaries);

            var homePageViewModel = CreateViewModel();
            homePageViewModel.OnClickExposures.Execute(null);

            mockNavigationService
                .Verify(x => x.NavigateAsync("ContactedNotifyPage"), Times.Once);
        }

        [Theory]
        [InlineData(-15)]
        [InlineData(-16)]
        [InlineData(-17)]
        [InlineData(-18)]
        [InlineData(-19)]
        public void OnClickExposuresTest_NavigateNotContactPage(int dayOffset)
        {
            var utcNow = DateTime.UtcNow;
            var dailySummaries = new List<DailySummary>()
            {
                CreateDailySummaryWithDayOffset(utcNow, -30),
                CreateDailySummaryWithDayOffset(utcNow, -20),
                CreateDailySummaryWithDayOffset(utcNow, dayOffset),
            };

            var serializeDailySummaries = JsonConvert.SerializeObject(dailySummaries);

            mockDateTimeUtility
                .Setup(x => x.UtcNow)
                .Returns(utcNow);

            mockPreferenceService
                .Setup(x => x.GetValue("DailySummaries", It.IsAny<string>()))
                .Returns(serializeDailySummaries);

            var homePageViewModel = CreateViewModel();
            homePageViewModel.OnClickExposures.Execute(null);

            mockNavigationService
                .Verify(x => x.NavigateAsync("NotContactPage"), Times.Once);
        }

    }
}
