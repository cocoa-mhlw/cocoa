// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Covid19Radar.Views;
using Moq;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels
{
    public class EventLogCooperationPageViewModelTests
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<INavigationService> _mockNavigationService;
        private readonly Mock<ILoggerService> _mockLoggerService;
        private readonly Mock<ISplashNavigationService> _mockSplashNavigationService;
        private readonly Mock<ISendEventLogStateRepository> _mockSendEventLogStateRepository;

        public EventLogCooperationPageViewModelTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockNavigationService = _mockRepository.Create<INavigationService>();
            _mockLoggerService = _mockRepository.Create<ILoggerService>();
            _mockSplashNavigationService = _mockRepository.Create<ISplashNavigationService>();
            _mockSendEventLogStateRepository = _mockRepository.Create<ISendEventLogStateRepository>();
        }

        private EventLogCooperationPageViewModel CreateViewModel()
        {
            return new EventLogCooperationPageViewModel(
                _mockNavigationService.Object,
                _mockLoggerService.Object,
                _mockSplashNavigationService.Object,
                _mockSendEventLogStateRepository.Object
                );
        }

        [Fact]
        public void SetupLaterLinkReadTextTest()
        {
            var unitUnderTest = CreateViewModel();
            Assert.Equal($"{AppResources.SetupLaerLink} {AppResources.Button}", unitUnderTest.SetupLaterLinkReadText);
        }

        [Fact]
        public async Task OnClickSettingTests_Tutorial()
        {
            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogCooperationPage.BuildNavigationParams(EventLogCooperationPage.TransitionReason.Tutorial));

            await unitUnderTest.OnClickSetting.ExecuteAsync();

            _mockNavigationService.Verify(x =>
                x.NavigateAsync($"{nameof(EventLogSettingPage)}",
                It.Is<INavigationParameters>(x =>
                    x.ContainsKey(EventLogSettingPage.TransitionReasonKey) &&
                    x.GetValue<EventLogSettingPage.TransitionReason>(EventLogSettingPage.TransitionReasonKey) == EventLogSettingPage.TransitionReason.Tutorial)
                ),
                Times.Once()
            );
        }

        [Fact]
        public async Task OnClickSettingTests_Splash()
        {
            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogCooperationPage.BuildNavigationParams(EventLogCooperationPage.TransitionReason.Splash));

            await unitUnderTest.OnClickSetting.ExecuteAsync();

            _mockNavigationService.Verify(x =>
                x.NavigateAsync($"{nameof(EventLogSettingPage)}",
                It.Is<INavigationParameters>(x =>
                    x.ContainsKey(EventLogSettingPage.TransitionReasonKey) &&
                    x.GetValue<EventLogSettingPage.TransitionReason>(EventLogSettingPage.TransitionReasonKey) == EventLogSettingPage.TransitionReason.Splash)
                ),
                Times.Once()
            );
        }

        [Fact]
        public async Task OnClickSettingTests_Exception()
        {
            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogCooperationPage.BuildNavigationParams((EventLogCooperationPage.TransitionReason)99));

            await Assert.ThrowsAsync<ArgumentException>(() => unitUnderTest.OnClickSetting.ExecuteAsync());

            _mockNavigationService.Verify(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<INavigationParameters>()), Times.Never());
        }

        [Fact]
        public async Task OnClickSetLaterTests_Tutorial()
        {
            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogCooperationPage.BuildNavigationParams(EventLogCooperationPage.TransitionReason.Tutorial));

            await unitUnderTest.OnClickSetLater.ExecuteAsync();

            _mockNavigationService.Verify(x => x.NavigateAsync(nameof(TutorialPage6)), Times.Once());
            _mockSplashNavigationService.Verify(x => x.NavigateNextAsync(It.IsAny<bool>()), Times.Never());
        }

        [Fact]
        public async Task OnClickSetLaterTests_Splash()
        {
            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogCooperationPage.BuildNavigationParams(EventLogCooperationPage.TransitionReason.Splash));

            await unitUnderTest.OnClickSetLater.ExecuteAsync();

            _mockNavigationService.Verify(x => x.NavigateAsync(It.IsAny<string>()), Times.Never());
            _mockSplashNavigationService.Verify(x => x.NavigateNextAsync(true), Times.Once());
        }

        [Fact]
        public async Task OnClickSetLaterTests_Exception()
        {
            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(EventLogCooperationPage.BuildNavigationParams((EventLogCooperationPage.TransitionReason)99));

            await Assert.ThrowsAsync<ArgumentException>(() => unitUnderTest.OnClickSetLater.ExecuteAsync());

            _mockNavigationService.Verify(x => x.NavigateAsync(It.IsAny<string>()), Times.Never());
            _mockSplashNavigationService.Verify(x => x.NavigateNextAsync(It.IsAny<bool>()), Times.Never());
        }
    }
}
