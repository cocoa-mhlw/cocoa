// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Covid19Radar.Views;
using Moq;
using Prism.Navigation;
using Xamarin.Forms;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels
{
    public class SettingsPageViewModelTests : IDisposable
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<INavigationService> _mockNavigationService;
        private readonly Mock<ILoggerService> _mockLoggerService;
        private readonly Mock<IUserDataRepository> _mockUserDataRepository;
        private readonly Mock<IExposureDataRepository> _mockExposureDataRepository;
        private readonly Mock<IExposureConfigurationRepository> _mockExposureConfigurationRepository;
        private readonly Mock<ISendEventLogStateRepository> _mockSendEventLogStateRepository;
        private readonly Mock<IEventLogRepository> _mockEventLogRepository;
        private readonly Mock<ILogFileService> _mockLogFileService;
        private readonly Mock<AbsExposureNotificationApiService> _mockAbsExposureNotificationApiService;
        private readonly Mock<ICloseApplicationService> _mockCloseApplicationService;
        private readonly Mock<IEssentialsService> _mockEssentialsService;

        private readonly Mock<IUserDialogs> _mockUserDialogs;

        public SettingsPageViewModelTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockNavigationService = _mockRepository.Create<INavigationService>();
            _mockLoggerService = _mockRepository.Create<ILoggerService>();
            _mockUserDataRepository = _mockRepository.Create<IUserDataRepository>();
            _mockExposureDataRepository = _mockRepository.Create<IExposureDataRepository>();
            _mockExposureConfigurationRepository = _mockRepository.Create<IExposureConfigurationRepository>();
            _mockSendEventLogStateRepository = _mockRepository.Create<ISendEventLogStateRepository>();
            _mockEventLogRepository = _mockRepository.Create<IEventLogRepository>();
            _mockLogFileService = _mockRepository.Create<ILogFileService>();
            _mockAbsExposureNotificationApiService = new Mock<AbsExposureNotificationApiService>(_mockLoggerService.Object);
            _mockCloseApplicationService = _mockRepository.Create<ICloseApplicationService>();
            _mockEssentialsService = _mockRepository.Create<IEssentialsService>();

            _mockUserDialogs = _mockRepository.Create<IUserDialogs>();
            UserDialogs.Instance = _mockUserDialogs.Object;

            Xamarin.Forms.Mocks.MockForms.Init();
            Application.Current = new Application();
        }

        public void Dispose()
        {
            UserDialogs.Instance = null;
            Application.Current = null;
        }

        private SettingsPageViewModel CreateViewModel()
        {
            return new SettingsPageViewModel(
                _mockNavigationService.Object,
                _mockLoggerService.Object,
                _mockUserDataRepository.Object,
                _mockExposureDataRepository.Object,
                _mockExposureConfigurationRepository.Object,
                _mockSendEventLogStateRepository.Object,
                _mockEventLogRepository.Object,
                _mockLogFileService.Object,
                _mockAbsExposureNotificationApiService.Object,
                _mockCloseApplicationService.Object,
                _mockEssentialsService.Object
                );
        }

        [Fact]
        public void AppVerTest()
        {
            _mockEssentialsService.SetupGet(x => x.AppVersion).Returns("1.2.3");
            SettingsPageViewModel unitUnderTest = CreateViewModel();
            Assert.Equal("1.2.3", unitUnderTest.AppVer);
        }

        [Fact]
        public void InitializeTest()
        {
            _mockEventLogRepository.Setup(x => x.IsExist()).ReturnsAsync(true);

            SettingsPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            _mockLoggerService.Verify(x => x.Info("isExistEventLogs: True", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task OnChangeResetDataTest_Ok()
        {
            _mockUserDialogs.Setup(x =>
                x.ConfirmAsync(
                    AppResources.SettingsPageDialogResetText,
                    AppResources.SettingsPageDialogResetTitle,
                    AppResources.ButtonOk,
                    AppResources.ButtonCancel,
                    null)
            ).ReturnsAsync(true);
            //_mockAbsExposureNotificationApiService.Setup(x => x.StopExposureNotificationAsync()).ReturnsAsync(true);

            SettingsPageViewModel unitUnderTest = CreateViewModel();
            await unitUnderTest.OnChangeResetData.ExecuteAsync();

            _mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            _mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());

            //_mockAbsExposureNotificationApiService.Verify(x => x.StopExposureNotificationAsync(), Times.Once());
            _mockExposureDataRepository.Verify(x => x.RemoveDailySummariesAsync(), Times.Once());
            _mockExposureDataRepository.Verify(x => x.RemoveExposureWindowsAsync(), Times.Once());
            _mockExposureDataRepository.Verify(x => x.RemoveExposureInformation(), Times.Once());
            _mockUserDataRepository.Verify(x => x.RemoveLastProcessDiagnosisKeyTimestampAsync(), Times.Once());
            _mockUserDataRepository.Verify(x => x.RemoveStartDate(), Times.Once());
            _mockUserDataRepository.Verify(x => x.RemoveAllUpdateDate(), Times.Once());
            _mockUserDataRepository.Verify(x => x.RemoveAllExposureNotificationStatus(), Times.Once());
            _mockExposureConfigurationRepository.Verify(x => x.RemoveExposureConfigurationAsync(), Times.Once());
            _mockSendEventLogStateRepository.Verify(x => x.RemoveAll(), Times.Once());
            _mockEventLogRepository.Verify(x => x.RemoveAllAsync(), Times.Once());
            _mockLogFileService.Verify(x => x.DeleteLogsDir(), Times.Once());
            _mockUserDialogs.Verify(x =>
                x.AlertAsync(
                    AppResources.SettingsPageDialogResetCompletedText,
                    AppResources.SettingsPageDialogResetCompletedTitle,
                    AppResources.ButtonOk, null),
                Times.Once());
            _mockCloseApplicationService.Verify(x => x.CloseApplication(), Times.Once());
        }

        [Fact]
        public async Task OnChangeResetDataTest_Cancel()
        {
            _mockUserDialogs.Setup(x =>
                x.ConfirmAsync(
                    AppResources.SettingsPageDialogResetText,
                    AppResources.SettingsPageDialogResetTitle,
                    AppResources.ButtonOk,
                    AppResources.ButtonCancel,
                    null)
            ).ReturnsAsync(false);
            //_mockAbsExposureNotificationApiService.Setup(x => x.StopExposureNotificationAsync()).ReturnsAsync(true);

            SettingsPageViewModel unitUnderTest = CreateViewModel();
            await unitUnderTest.OnChangeResetData.ExecuteAsync();

            _mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Never());
            _mockUserDialogs.Verify(x => x.HideLoading(), Times.Never());

            //_mockAbsExposureNotificationApiService.Verify(x => x.StopExposureNotificationAsync(), Times.Never());
            _mockExposureDataRepository.Verify(x => x.RemoveDailySummariesAsync(), Times.Never());
            _mockExposureDataRepository.Verify(x => x.RemoveExposureWindowsAsync(), Times.Never());
            _mockExposureDataRepository.Verify(x => x.RemoveExposureInformation(), Times.Never());
            _mockUserDataRepository.Verify(x => x.RemoveLastProcessDiagnosisKeyTimestampAsync(), Times.Never());
            _mockUserDataRepository.Verify(x => x.RemoveStartDate(), Times.Never());
            _mockUserDataRepository.Verify(x => x.RemoveAllUpdateDate(), Times.Never());
            _mockUserDataRepository.Verify(x => x.RemoveAllExposureNotificationStatus(), Times.Never());
            _mockExposureConfigurationRepository.Verify(x => x.RemoveExposureConfigurationAsync(), Times.Never());
            _mockSendEventLogStateRepository.Verify(x => x.RemoveAll(), Times.Never());
            _mockEventLogRepository.Verify(x => x.RemoveAllAsync(), Times.Never());
            _mockLogFileService.Verify(x => x.DeleteLogsDir(), Times.Never());
            _mockUserDialogs.Verify(x =>
                x.AlertAsync(
                    AppResources.SettingsPageDialogResetCompletedText,
                    AppResources.SettingsPageDialogResetCompletedTitle,
                    AppResources.ButtonOk, null),
                Times.Never());
            _mockCloseApplicationService.Verify(x => x.CloseApplication(), Times.Never());
        }

        [Fact]
        public async Task OnEventLogSendTest()
        {
            SettingsPageViewModel unitUnderTest = CreateViewModel();
            await unitUnderTest.OnEventLogSend.ExecuteAsync();

            _mockNavigationService.Verify(x =>
                x.NavigateAsync(nameof(EventLogSettingPage),
                It.Is<INavigationParameters>(x =>
                    x.ContainsKey(EventLogSettingPage.TransitionReasonKey) &&
                    x.GetValue<EventLogSettingPage.TransitionReason>(EventLogSettingPage.TransitionReasonKey) == EventLogSettingPage.TransitionReason.Setting
                ))
            );
        }
    }
}

