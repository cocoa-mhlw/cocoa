// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels.EndOfService;
using Covid19Radar.Views.EndOfService;
using Moq;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels.EndOfService
{
    public class TerminationOfUsePageViewModelTests : IDisposable
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<INavigationService> _mockNavigationService;
        private readonly Mock<ILoggerService> _mockLoggerService;
        private readonly Mock<ILogFileService> _mockLogFileService;
        private readonly Mock<ISurveyService> _mockSurveyService;
        private readonly Mock<IDialogService> _mockDialogService;

        private readonly Mock<IUserDataRepository> _mockUserDataRepository;
        private readonly Mock<IEventLogRepository> _mockEventLogRepository;
        private readonly Mock<ISendEventLogStateRepository> _mockSendEventLogStateRepository;
        private readonly Mock<IExposureDataRepository> _mockExposureDataRepository;
        private readonly Mock<IExposureConfigurationRepository> _mockExposureConfigurationRepository;

        private readonly Mock<AbsExposureNotificationApiService> _mockAbsExposureNotificationApiService;
        private readonly Mock<AbsExposureDetectionBackgroundService> _mockAbsExposureDetectionBackgroundService;
        private readonly Mock<AbsDataMaintainanceBackgroundService> _mockAbsDataMaintainanceBackgroundService;

        private readonly Mock<IUserDialogs> _mockUserDialogs;

        public TerminationOfUsePageViewModelTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockNavigationService = _mockRepository.Create<INavigationService>();
            _mockLoggerService = _mockRepository.Create<ILoggerService>();
            _mockLogFileService = _mockRepository.Create<ILogFileService>();
            _mockSurveyService = _mockRepository.Create<ISurveyService>();
            _mockDialogService = _mockRepository.Create<IDialogService>();

            _mockUserDataRepository = _mockRepository.Create<IUserDataRepository>();
            _mockEventLogRepository = _mockRepository.Create<IEventLogRepository>();
            _mockSendEventLogStateRepository = _mockRepository.Create<ISendEventLogStateRepository>();
            _mockExposureDataRepository = _mockRepository.Create<IExposureDataRepository>();
            _mockExposureConfigurationRepository = _mockRepository.Create<IExposureConfigurationRepository>();

            _mockAbsExposureNotificationApiService = new Mock<AbsExposureNotificationApiService>(_mockLoggerService.Object);
            _mockAbsExposureDetectionBackgroundService = new Mock<AbsExposureDetectionBackgroundService>(
                _mockRepository.Create<IDiagnosisKeyRepository>().Object,
                _mockAbsExposureNotificationApiService.Object,
                _mockExposureConfigurationRepository.Object,
                _mockLoggerService.Object,
                _mockUserDataRepository.Object,
                _mockRepository.Create<IServerConfigurationRepository>().Object,
                _mockRepository.Create<ILocalPathService>().Object,
                _mockRepository.Create<IDateTimeUtility>().Object,
                _mockRepository.Create<ILocalNotificationService>().Object,
                _mockRepository.Create<IEndOfServiceNotificationService>().Object);
            _mockAbsDataMaintainanceBackgroundService = new Mock<AbsDataMaintainanceBackgroundService>(
                _mockLoggerService.Object,
                _mockLogFileService.Object);

            _mockUserDialogs = _mockRepository.Create<IUserDialogs>();
            UserDialogs.Instance = _mockUserDialogs.Object;
        }

        public void Dispose()
        {
            UserDialogs.Instance = null;
        }

        private TerminationOfUsePageViewModel CreateViewModel()
        {
            return new TerminationOfUsePageViewModel(
                _mockNavigationService.Object,
                _mockLoggerService.Object,
                _mockLogFileService.Object,
                _mockSurveyService.Object,
                _mockDialogService.Object,
                _mockUserDataRepository.Object,
                _mockEventLogRepository.Object,
                _mockSendEventLogStateRepository.Object,
                _mockExposureDataRepository.Object,
                _mockExposureConfigurationRepository.Object,
                _mockAbsExposureNotificationApiService.Object,
                _mockAbsExposureDetectionBackgroundService.Object,
                _mockAbsDataMaintainanceBackgroundService.Object
                );
        }

        [Fact]
        public async Task OnTerminationButtonTests_NoSurveyContent()
        {
            TerminationOfUsePageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());
            await unitUnderTest.OnTerminationButton.ExecuteAsync();

            _mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            _mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());

            _mockSurveyService.Verify(x => x.SubmitSurvey(It.IsAny<SurveyContent>()), Times.Never());

            //_mockAbsExposureNotificationApiService.Verify(x => x.StopExposureNotificationAsync(), Times.Once());
            _mockAbsExposureDetectionBackgroundService.Verify(x => x.Cancel(), Times.Once());
            _mockAbsDataMaintainanceBackgroundService.Verify(x => x.Cancel(), Times.Once());

            _mockExposureDataRepository.Verify(x => x.RemoveDailySummariesAsync(), Times.Once());
            _mockExposureDataRepository.Verify(x => x.RemoveExposureWindowsAsync(), Times.Once());
            _mockExposureDataRepository.Verify(x => x.RemoveExposureInformation(), Times.Once());
            _mockUserDataRepository.Verify(x => x.RemoveAll(), Times.Once());
            _mockExposureConfigurationRepository.Verify(x => x.RemoveExposureConfigurationAsync(), Times.Once());
            _mockSendEventLogStateRepository.Verify(x => x.RemoveAll(), Times.Once());
            _mockEventLogRepository.Verify(x => x.RemoveAllAsync(), Times.Once());
            _mockLogFileService.Verify(x => x.DeleteLogsDir(), Times.Once());
            _mockNavigationService.Verify(x => x.NavigateAsync("/TerminationOfUseCompletePage"), Times.Once());
        }

        [Fact]
        public async Task OnTerminationButtonTests_ExistSurveyContent_SubmitSuccess()
        {
            var testSurveyContent = new SurveyContent { Q1 = 1, Q2 = 2, StartDate = 1640962800, ExposureData = null };

            var testNavigationParameters = new NavigationParameters();
            testNavigationParameters.Add(TerminationOfUsePage.NavigationParameterNameSurveyContent, testSurveyContent);

            _mockSurveyService.Setup(x => x.SubmitSurvey(It.IsAny<SurveyContent>())).ReturnsAsync(true);

            TerminationOfUsePageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(testNavigationParameters);
            await unitUnderTest.OnTerminationButton.ExecuteAsync();

            _mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            _mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());

            _mockSurveyService.Verify(x => x.SubmitSurvey(It.Is<SurveyContent>(x => x.Q1 == 1 && x.Q2 == 2 && x.StartDate == 1640962800 && x.ExposureData == null)), Times.Once());
            _mockDialogService.Verify(x => x.ShowNetworkConnectionErrorAsync(), Times.Never());

            //_mockAbsExposureNotificationApiService.Verify(x => x.StopExposureNotificationAsync(), Times.Once());
            _mockAbsExposureDetectionBackgroundService.Verify(x => x.Cancel(), Times.Once());
            _mockAbsDataMaintainanceBackgroundService.Verify(x => x.Cancel(), Times.Once());

            _mockExposureDataRepository.Verify(x => x.RemoveDailySummariesAsync(), Times.Once());
            _mockExposureDataRepository.Verify(x => x.RemoveExposureWindowsAsync(), Times.Once());
            _mockExposureDataRepository.Verify(x => x.RemoveExposureInformation(), Times.Once());
            _mockUserDataRepository.Verify(x => x.RemoveAll(), Times.Once());
            _mockExposureConfigurationRepository.Verify(x => x.RemoveExposureConfigurationAsync(), Times.Once());
            _mockSendEventLogStateRepository.Verify(x => x.RemoveAll(), Times.Once());
            _mockEventLogRepository.Verify(x => x.RemoveAllAsync(), Times.Once());
            _mockLogFileService.Verify(x => x.DeleteLogsDir(), Times.Once());
            _mockNavigationService.Verify(x => x.NavigateAsync("/TerminationOfUseCompletePage"), Times.Once());
        }

        [Fact]
        public async Task OnTerminationButtonTests_ExistSurveyContent_SubmitFailure()
        {
            var testSurveyContent = new SurveyContent { Q1 = 1, Q2 = 2, StartDate = 1640962800, ExposureData = null };

            var testNavigationParameters = new NavigationParameters();
            testNavigationParameters.Add(TerminationOfUsePage.NavigationParameterNameSurveyContent, testSurveyContent);

            _mockSurveyService.Setup(x => x.SubmitSurvey(It.IsAny<SurveyContent>())).ReturnsAsync(false);

            TerminationOfUsePageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(testNavigationParameters);
            await unitUnderTest.OnTerminationButton.ExecuteAsync();

            _mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            _mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());

            _mockSurveyService.Verify(x => x.SubmitSurvey(It.Is<SurveyContent>(x => x.Q1 == 1 && x.Q2 == 2 && x.StartDate == 1640962800 && x.ExposureData == null)), Times.Once());
            _mockDialogService.Verify(x => x.ShowNetworkConnectionErrorAsync(), Times.Once());

            //_mockAbsExposureNotificationApiService.Verify(x => x.StopExposureNotificationAsync(), Times.Never());
            _mockAbsExposureDetectionBackgroundService.Verify(x => x.Cancel(), Times.Never());
            _mockAbsDataMaintainanceBackgroundService.Verify(x => x.Cancel(), Times.Never());

            _mockExposureDataRepository.Verify(x => x.RemoveDailySummariesAsync(), Times.Never());
            _mockExposureDataRepository.Verify(x => x.RemoveExposureWindowsAsync(), Times.Never());
            _mockExposureDataRepository.Verify(x => x.RemoveExposureInformation(), Times.Never());
            _mockUserDataRepository.Verify(x => x.RemoveAll(), Times.Never());
            _mockExposureConfigurationRepository.Verify(x => x.RemoveExposureConfigurationAsync(), Times.Never());
            _mockSendEventLogStateRepository.Verify(x => x.RemoveAll(), Times.Never());
            _mockEventLogRepository.Verify(x => x.RemoveAllAsync(), Times.Never());
            _mockLogFileService.Verify(x => x.DeleteLogsDir(), Times.Never());
            _mockNavigationService.Verify(x => x.NavigateAsync("/TerminationOfUseCompletePage"), Times.Never());
        }
    }
}

