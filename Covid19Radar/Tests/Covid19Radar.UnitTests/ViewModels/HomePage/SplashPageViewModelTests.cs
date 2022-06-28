// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Services.Migration;
using Covid19Radar.ViewModels;
using Covid19Radar.Views;
using Moq;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels
{
    public class SplashPageViewModelTests
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<INavigationService> _mockNavigationService;
        private readonly Mock<ILoggerService> _mockLoggerService;
        private readonly Mock<IMigrationService> _mockMigrationService;
        private readonly Mock<ISplashNavigationService> _mockSplashNavigationService;

        public SplashPageViewModelTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockNavigationService = _mockRepository.Create<INavigationService>();
            _mockLoggerService = _mockRepository.Create<ILoggerService>();
            _mockMigrationService = _mockRepository.Create<IMigrationService>();
            _mockSplashNavigationService = _mockRepository.Create<ISplashNavigationService>();
        }

        public SplashPageViewModel CreateViewModel()
        {
            return new SplashPageViewModel(
                _mockNavigationService.Object,
                _mockLoggerService.Object,
                _mockMigrationService.Object,
                _mockSplashNavigationService.Object
            );
        }

        [Fact]
        public void OnNavigatedToTest()
        {
            SplashPageViewModel unitUnderTest = CreateViewModel();
            unitUnderTest.OnNavigatedTo(SplashPage.BuildNavigationParams(Destination.ContactedNotifyPage, new NavigationParameters { { "test-key", "test-value" } }));

            _mockMigrationService.Verify(x => x.MigrateAsync(), Times.Once());

            _mockSplashNavigationService.VerifySet(x => x.Destination = Destination.ContactedNotifyPage, Times.Once());
            _mockSplashNavigationService.VerifySet(x =>
                x.DestinationPageParameters = It.Is<INavigationParameters>(x =>
                    x.ContainsKey("test-key") && x.GetValue<string>("test-key") == "test-value"),
                Times.Once()
            );

            _mockSplashNavigationService.Verify(x => x.Prepare(), Times.Once());
            _mockSplashNavigationService.Verify(x => x.NavigateNextAsync(), Times.Once());
        }
    }
}

