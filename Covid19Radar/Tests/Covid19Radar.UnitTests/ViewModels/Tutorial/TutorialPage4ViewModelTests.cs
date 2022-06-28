// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Threading.Tasks;
using Chino;
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
    public class TutorialPage4ViewModelTests
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<INavigationService> _mockNavigationService;
        private readonly Mock<IDialogService> _mockDialogService;
        private readonly Mock<ILoggerService> _mockLoggerService;
        private readonly Mock<AbsExposureNotificationApiService> _mockAbsExposureNotificationApiService;

        public TutorialPage4ViewModelTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockNavigationService = _mockRepository.Create<INavigationService>();
            _mockDialogService = _mockRepository.Create<IDialogService>();
            _mockLoggerService = _mockRepository.Create<ILoggerService>();
            _mockAbsExposureNotificationApiService = new Mock<AbsExposureNotificationApiService>(_mockLoggerService.Object);
        }

        private TutorialPage4ViewModel CreateViewModel()
        {
            var vm = new TutorialPage4ViewModel(
                _mockNavigationService.Object,
                _mockDialogService.Object,
                _mockLoggerService.Object,
                _mockAbsExposureNotificationApiService.Object
            );
            return vm;
        }

        [Fact]
        public void SetupLaerLinkReadTextTest()
        {
            var unitUnderTest = CreateViewModel();
            Assert.Equal($"{AppResources.SetupLaerLink} {AppResources.Button}", unitUnderTest.SetupLaerLinkReadText);
        }

        [Fact]
        public async Task OnClickEnableTest()
        {
            _mockAbsExposureNotificationApiService.Setup(x => x.StartExposureNotificationAsync()).ReturnsAsync(true);

            TutorialPage4ViewModel unitUnderTest = CreateViewModel();
            await unitUnderTest.OnClickEnable.ExecuteAsync();

            _mockAbsExposureNotificationApiService.Verify(x => x.StartExposureNotificationAsync(), Times.Once());
            _mockNavigationService.Verify(x =>
                x.NavigateAsync(nameof(EventLogCooperationPage),
                It.Is<INavigationParameters>(x =>
                    x.ContainsKey(EventLogCooperationPage.TransitionReasonKey) &&
                    x.GetValue<EventLogCooperationPage.TransitionReason>(EventLogCooperationPage.TransitionReasonKey) == EventLogCooperationPage.TransitionReason.Tutorial)
                ),
                Times.Once()
            );
        }

        [Fact]
        public async Task OnClickEnableTest_NotSupported()
        {
            _mockAbsExposureNotificationApiService.Setup(x => x.StartExposureNotificationAsync()).ThrowsAsync(new ENException(ENException.Code_Android.FAILED_NOT_SUPPORTED, "Unit test"));
            _mockAbsExposureNotificationApiService.Setup(x => x.GetStatusCodesAsync()).ReturnsAsync(new List<int> { { ExposureNotificationStatus.Code_Android.USER_PROFILE_NOT_SUPPORT } });

            TutorialPage4ViewModel unitUnderTest = CreateViewModel();
            await unitUnderTest.OnClickEnable.ExecuteAsync();

            _mockAbsExposureNotificationApiService.Verify(x => x.StartExposureNotificationAsync(), Times.Once());
            _mockDialogService.Verify(x => x.ShowUserProfileNotSupportAsync(), Times.Once());
            _mockNavigationService.Verify(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<INavigationParameters>()), Times.Never());
        }

        [Fact]
        public async Task OnClickEnableTest_ENException()
        {
            _mockAbsExposureNotificationApiService.Setup(x => x.StartExposureNotificationAsync()).ThrowsAsync(new ENException(ENException.Code_Android.FAILED, "Unit test"));

            TutorialPage4ViewModel unitUnderTest = CreateViewModel();
            await unitUnderTest.OnClickEnable.ExecuteAsync();

            _mockAbsExposureNotificationApiService.Verify(x => x.StartExposureNotificationAsync(), Times.Once());
            _mockDialogService.Verify(x => x.ShowUserProfileNotSupportAsync(), Times.Never());
            _mockNavigationService.Verify(x =>
                x.NavigateAsync(nameof(EventLogCooperationPage),
                It.Is<INavigationParameters>(x =>
                    x.ContainsKey(EventLogCooperationPage.TransitionReasonKey) &&
                    x.GetValue<EventLogCooperationPage.TransitionReason>(EventLogCooperationPage.TransitionReasonKey) == EventLogCooperationPage.TransitionReason.Tutorial)
                ),
                Times.Once()
            );
        }

        [Fact]
        public async Task OnClickDisableTest()
        {
            _mockAbsExposureNotificationApiService.Setup(x => x.StartExposureNotificationAsync()).ReturnsAsync(true);

            TutorialPage4ViewModel unitUnderTest = CreateViewModel();
            await unitUnderTest.OnClickDisable.ExecuteAsync();

            _mockAbsExposureNotificationApiService.Verify(x => x.StartExposureNotificationAsync(), Times.Never());
            _mockNavigationService.Verify(x =>
                x.NavigateAsync(nameof(EventLogCooperationPage),
                It.Is<INavigationParameters>(x =>
                    x.ContainsKey(EventLogCooperationPage.TransitionReasonKey) &&
                    x.GetValue<EventLogCooperationPage.TransitionReason>(EventLogCooperationPage.TransitionReasonKey) == EventLogCooperationPage.TransitionReason.Tutorial)
                ),
                Times.Once()
            );
        }

        [Fact]
        public void OnEnabledTest()
        {
            _mockAbsExposureNotificationApiService.Setup(x => x.StartExposureNotificationAsync()).ReturnsAsync(true);

            TutorialPage4ViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.OnEnabled();

            _mockAbsExposureNotificationApiService.Verify(x => x.StartExposureNotificationAsync(), Times.Once());
            _mockNavigationService.Verify(x =>
                x.NavigateAsync(nameof(EventLogCooperationPage),
                It.Is<INavigationParameters>(x =>
                    x.ContainsKey(EventLogCooperationPage.TransitionReasonKey) &&
                    x.GetValue<EventLogCooperationPage.TransitionReason>(EventLogCooperationPage.TransitionReasonKey) == EventLogCooperationPage.TransitionReason.Tutorial)
                ),
                Times.Once()
            );
        }
    }
}

