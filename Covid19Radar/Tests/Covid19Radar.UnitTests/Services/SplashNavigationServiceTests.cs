// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Moq;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class SplashNavigationServiceTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigatoinService;
        private readonly Mock<IUserDataRepository> mockUserDataRepository;
        private readonly Mock<ILoggerService> mockLoggerService;

        public SplashNavigationServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigatoinService = mockRepository.Create<INavigationService>();
            mockUserDataRepository = mockRepository.Create<IUserDataRepository>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
        }

        public SplashNavigationService CreateService()
        {
            return new SplashNavigationService(
                mockNavigatoinService.Object,
                mockUserDataRepository.Object,
                mockLoggerService.Object);
        }

        [Fact]
        public async Task NavigateNextAsyncTest_EndOfServicePage()
        {
            SplashNavigationService unitUnderTest = CreateService();

            unitUnderTest.Destination = Destination.EndOfServiceNotice;
            unitUnderTest.DestinationPageParameters = new NavigationParameters();

            mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(false);

            await unitUnderTest.NavigateNextAsync();

            mockUserDataRepository.Verify(x => x.IsAllAgreed(), Times.Once());
            mockNavigatoinService.Verify(x => x.NavigateAsync("/EndOfServicePage", It.IsAny<INavigationParameters>()), Times.Once());
        }

        [Fact]
        public async Task NavigateNextAsyncTest_Destination()
        {
            SplashNavigationService unitUnderTest = CreateService();

            unitUnderTest.Destination = Destination.EndOfServiceNotice;
            unitUnderTest.DestinationPageParameters = new NavigationParameters { { "test-key", "test-value" } };

            mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);

            await unitUnderTest.NavigateNextAsync();

            mockUserDataRepository.Verify(x => x.IsAllAgreed(), Times.Once());
            mockNavigatoinService.Verify(x => x.NavigateAsync(
                Destination.EndOfServiceNotice.ToPath(),
                It.Is<INavigationParameters>(x =>
                    x.ContainsKey("test-key") &&
                    x.GetValue<string>("test-key") == "test-value")),
                Times.Once());
        }
    }
}

