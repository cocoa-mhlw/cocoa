// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Linq;
using System.Threading.Tasks;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Covid19Radar.Views;
using Moq;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels
{
    public class SettingsPageViewModelTests
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<INavigationService> _mockNavigationService;
        private readonly Mock<ILoggerService> _mockLoggerService;
        private readonly Mock<IUserDataRepository> _mockUserDataRepository;
        private readonly Mock<IExposureDataRepository> _mockExposureDataRepository;
        private readonly Mock<IExposureConfigurationRepository> _mockExposureConfigurationRepository;
        private readonly Mock<ILogFileService> _mockLogFileService;
        private readonly Mock<AbsExposureNotificationApiService> _mockAbsExposureNotificationApiService;
        private readonly Mock<ICloseApplicationService> _mockCloseApplicationService;
        private readonly Mock<IEssentialsService> _mockEssentialsService;

        public SettingsPageViewModelTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockNavigationService = _mockRepository.Create<INavigationService>();
            _mockLoggerService = _mockRepository.Create<ILoggerService>();
            _mockUserDataRepository = _mockRepository.Create<IUserDataRepository>();
            _mockExposureDataRepository = _mockRepository.Create<IExposureDataRepository>();
            _mockExposureConfigurationRepository = _mockRepository.Create<IExposureConfigurationRepository>();
            _mockLogFileService = _mockRepository.Create<ILogFileService>();
            _mockAbsExposureNotificationApiService = new Mock<AbsExposureNotificationApiService>(_mockLoggerService.Object);
            _mockCloseApplicationService = _mockRepository.Create<ICloseApplicationService>();
            _mockEssentialsService = _mockRepository.Create<IEssentialsService>();
        }

        private SettingsPageViewModel CreateViewModel()
        {
            return new SettingsPageViewModel(
                _mockNavigationService.Object,
                _mockLoggerService.Object,
                _mockUserDataRepository.Object,
                _mockExposureDataRepository.Object,
                _mockExposureConfigurationRepository.Object,
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

