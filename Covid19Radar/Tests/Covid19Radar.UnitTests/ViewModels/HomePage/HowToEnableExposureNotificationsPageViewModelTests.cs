// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Covid19Radar.Services;
using Covid19Radar.ViewModels;
using Moq;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels
{
    public class HowToEnableExposureNotificationsPageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<IExternalNavigationService> mockExternalNavigationService;

        public HowToEnableExposureNotificationsPageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockExternalNavigationService = mockRepository.Create<IExternalNavigationService>();
        }

        private HowToEnableExposureNotificationsPageViewModel CreateViewModel()
        {
            return new HowToEnableExposureNotificationsPageViewModel(
                mockNavigationService.Object,
                mockExternalNavigationService.Object
                );
        }

        [Fact]
        public void OnExposureNotificationSettingButtonTest()
        {
            var unitUnderTest = CreateViewModel();
            unitUnderTest.OnExposureNotificationSettingButton.Execute(null);
            mockExternalNavigationService.Verify(x => x.NavigateAppSettings(), Times.Once());
        }
    }
}
