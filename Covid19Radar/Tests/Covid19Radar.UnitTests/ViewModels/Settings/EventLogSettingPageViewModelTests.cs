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
    public class EventLogSettingPageViewModelTests
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<INavigationService> _mockNavigationService;
        private readonly Mock<ILoggerService> _mockLoggerService;
        private readonly Mock<IDialogService> _mockDialogService;
        private readonly Mock<ISplashNavigationService> _mockSplashNavigationService;
        private readonly Mock<ISendEventLogStateRepository> _mockSendEventLogStateRepository;

        public EventLogSettingPageViewModelTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockNavigationService = _mockRepository.Create<INavigationService>();
            _mockLoggerService = _mockRepository.Create<ILoggerService>();
            _mockDialogService = _mockRepository.Create<IDialogService>();
            _mockSplashNavigationService = _mockRepository.Create<ISplashNavigationService>();
            _mockSendEventLogStateRepository = _mockRepository.Create<ISendEventLogStateRepository>();
        }

        private EventLogSettingPageViewModel CreateViewModel()
        {
            return new EventLogSettingPageViewModel(
                _mockNavigationService.Object,
                _mockLoggerService.Object,
                _mockDialogService.Object,
                _mockSplashNavigationService.Object,
                _mockSendEventLogStateRepository.Object
                );
        }

        [Fact]
        public void BackButtonEnabledTest_Tutorial()
        {
            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Tutorial));
            Assert.False(unitUnderTest.BackButtonEnabled);
        }

        [Fact]
        public void BackButtonEnabledTest_Splash()
        {
            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Splash));
            Assert.False(unitUnderTest.BackButtonEnabled);
        }

        [Fact]
        public void BackButtonEnabledTest_Setting()
        {
            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Setting));
            Assert.True(unitUnderTest.BackButtonEnabled);
        }

        [Fact]
        public void IsVisibleTitleInContentTest_Tutorial()
        {
            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Tutorial));
            Assert.True(unitUnderTest.IsVisibleTitleInContent);
        }

        [Fact]
        public void IsVisibleTitleInContentTest_Splash()
        {
            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Splash));
            Assert.True(unitUnderTest.IsVisibleTitleInContent);
        }

        [Fact]
        public void IsVisibleTitleInContentTest_Setting()
        {
            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Setting));
            Assert.False(unitUnderTest.IsVisibleTitleInContent);
        }

        [Fact]
        public void InitializeTest_ExposureNotified_NotSet()
        {
            _mockSendEventLogStateRepository.Setup(x => x.GetSendEventLogState(EventType.ExposureNotified)).Returns(SendEventLogState.NotSet);

            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Tutorial));

            Assert.False(unitUnderTest.ExposureNotifyIsToggled);
        }

        [Fact]
        public void InitializeTest_ExposureNotified_Disable()
        {
            _mockSendEventLogStateRepository.Setup(x => x.GetSendEventLogState(EventType.ExposureNotified)).Returns(SendEventLogState.Disable);

            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Tutorial));

            Assert.False(unitUnderTest.ExposureNotifyIsToggled);
        }

        [Fact]
        public void InitializeTest_ExposureNotified_Enable()
        {
            _mockSendEventLogStateRepository.Setup(x => x.GetSendEventLogState(EventType.ExposureNotified)).Returns(SendEventLogState.Enable);

            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Tutorial));

            Assert.True(unitUnderTest.ExposureNotifyIsToggled);
        }

        [Fact]
        public async Task OnClickSaveTest_ExposureNotified_Disable()
        {
            _mockSendEventLogStateRepository.Setup(x => x.GetSendEventLogState(EventType.ExposureNotified)).Returns(SendEventLogState.Enable);

            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Tutorial));

            await unitUnderTest.OnClickSave.ExecuteAsync();

            Assert.False(unitUnderTest.ExposureNotifyIsToggled);

            _mockSendEventLogStateRepository.Verify(x => x.SetSendEventLogState(EventType.ExposureNotified, SendEventLogState.Disable), Times.Once());

            foreach (var eventType in EventType.All.Where(x => x != EventType.ExposureNotified))
            {
                _mockSendEventLogStateRepository.Verify(x => x.SetSendEventLogState(eventType, SendEventLogState.Disable), Times.Once());
            }
        }

        [Fact]
        public async Task OnClickSaveTest_ExposureNotified_Enable()
        {
            _mockSendEventLogStateRepository.Setup(x => x.GetSendEventLogState(EventType.ExposureNotified)).Returns(SendEventLogState.Disable);

            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Tutorial));

            await unitUnderTest.OnClickSave.ExecuteAsync();

            Assert.True(unitUnderTest.ExposureNotifyIsToggled);

            _mockSendEventLogStateRepository.Verify(x => x.SetSendEventLogState(EventType.ExposureNotified, SendEventLogState.Enable), Times.Once());

            foreach (var eventType in EventType.All.Where(x => x != EventType.ExposureNotified))
            {
                _mockSendEventLogStateRepository.Verify(x => x.SetSendEventLogState(eventType, SendEventLogState.Disable), Times.Once());
            }
        }

        [Fact]
        public async Task OnClickSaveTest_Tutorial()
        {
            _mockSendEventLogStateRepository.Setup(x => x.GetSendEventLogState(EventType.ExposureNotified)).Returns(SendEventLogState.NotSet);

            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Tutorial));

            await unitUnderTest.OnClickSave.ExecuteAsync();

            _mockNavigationService.Verify(x => x.NavigateAsync(nameof(TutorialPage6)), Times.Once());
            _mockSplashNavigationService.Verify(x => x.NavigateNextAsync(false), Times.Never());
            _mockNavigationService.Verify(x => x.GoBackAsync(), Times.Never());
        }

        [Fact]
        public async Task OnClickSaveTest_Splash()
        {
            _mockSendEventLogStateRepository.Setup(x => x.GetSendEventLogState(EventType.ExposureNotified)).Returns(SendEventLogState.NotSet);

            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Splash));

            await unitUnderTest.OnClickSave.ExecuteAsync();

            _mockNavigationService.Verify(x => x.NavigateAsync(nameof(TutorialPage6)), Times.Never());
            _mockSplashNavigationService.Verify(x => x.NavigateNextAsync(false), Times.Once());
            _mockNavigationService.Verify(x => x.GoBackAsync(), Times.Never());
        }

        [Fact]
        public async Task OnClickSaveTest_Setting()
        {
            _mockSendEventLogStateRepository.Setup(x => x.GetSendEventLogState(EventType.ExposureNotified)).Returns(SendEventLogState.NotSet);

            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Setting));

            await unitUnderTest.OnClickSave.ExecuteAsync();

            _mockNavigationService.Verify(x => x.NavigateAsync(nameof(TutorialPage6)), Times.Never());
            _mockSplashNavigationService.Verify(x => x.NavigateNextAsync(false), Times.Never());
            _mockNavigationService.Verify(x => x.GoBackAsync(), Times.Once());
        }

        [Fact]
        public async Task OnClickSaveTest_Exception()
        {
            _mockSendEventLogStateRepository.Setup(x => x.GetSendEventLogState(EventType.ExposureNotified)).Returns(SendEventLogState.NotSet);

            EventLogSettingPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogSettingPage.BuildNavigationParams((EventLogSettingPage.TransitionReason)99));

            await Assert.ThrowsAsync<ArgumentException>(() => unitUnderTest.OnClickSave.ExecuteAsync());

            _mockNavigationService.Verify(x => x.NavigateAsync(nameof(TutorialPage6)), Times.Never());
            _mockSplashNavigationService.Verify(x => x.NavigateNextAsync(false), Times.Never());
            _mockNavigationService.Verify(x => x.GoBackAsync(), Times.Never());
        }
    }
}
