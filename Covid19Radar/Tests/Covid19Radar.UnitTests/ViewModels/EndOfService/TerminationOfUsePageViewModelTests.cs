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
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels.EndOfService;
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
                _mockRepository.Create<ILocalNotificationService>().Object);
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
        public async Task OnTerminationButtonTests()
        {
            TerminationOfUsePageViewModel unitUnderTest = CreateViewModel();
            await unitUnderTest.OnTerminationButton.ExecuteAsync();

            _mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            _mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());

            //_mockAbsExposureNotificationApiService.Verify(x => x.StopExposureNotificationAsync(), Times.Once());
            _mockAbsExposureDetectionBackgroundService.Verify(x => x.Cancel(), Times.Once());
            _mockAbsDataMaintainanceBackgroundService.Verify(x => x.Cancel(), Times.Once());

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
            _mockNavigationService.Verify(x => x.NavigateAsync("/TerminationOfUseCompletePage"), Times.Once());
        }
    }
}

