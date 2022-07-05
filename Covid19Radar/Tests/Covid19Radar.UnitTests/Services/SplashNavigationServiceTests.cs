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
        private readonly Mock<ITermsUpdateService> mockTermsUpdateService;
        private readonly Mock<ISendEventLogStateRepository> mockSendEventLogStateRepository;

        private readonly TermsUpdateInfoModel _termsUpdateInfoDefault = new TermsUpdateInfoModel
        {
            TermsOfService = new TermsUpdateInfoModel.Detail { Text = "Test: Terms of service", UpdateDateTimeJst = DateTime.Now },
            PrivacyPolicy = new TermsUpdateInfoModel.Detail { Text = "Test: Privacy policy", UpdateDateTimeJst = DateTime.Now }
        };

        public SplashNavigationServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigatoinService = mockRepository.Create<INavigationService>();
            mockUserDataRepository = mockRepository.Create<IUserDataRepository>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockTermsUpdateService = mockRepository.Create<ITermsUpdateService>();
            mockSendEventLogStateRepository = mockRepository.Create<ISendEventLogStateRepository>();
        }

        public SplashNavigationService CreateService()
        {
            return new SplashNavigationService(
                mockNavigatoinService.Object,
                mockUserDataRepository.Object,
                mockLoggerService.Object,
                mockTermsUpdateService.Object,
                mockSendEventLogStateRepository.Object);
        }

        [Fact]
        public async Task PrepareTestAsync()
        {
            SplashNavigationService unitUnderTest = CreateService();

            mockTermsUpdateService.Setup(x => x.GetTermsUpdateInfo()).ReturnsAsync(_termsUpdateInfoDefault);

            await unitUnderTest.Prepare();

            mockTermsUpdateService.Verify(x => x.GetTermsUpdateInfo(), Times.Once());
        }

        [Fact]
        public async Task NavigateNextAsyncTest_Tutorial()
        {
            SplashNavigationService unitUnderTest = CreateService();

            unitUnderTest.Destination = Destination.HomePage;
            unitUnderTest.DestinationPageParameters = new NavigationParameters();

            mockTermsUpdateService.Setup(x => x.GetTermsUpdateInfo()).ReturnsAsync(_termsUpdateInfoDefault);

            await unitUnderTest.Prepare();
            mockTermsUpdateService.Verify(x => x.GetTermsUpdateInfo(), Times.Once());

            mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(false);
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.TermsOfService, _termsUpdateInfoDefault)).Returns(false);
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.PrivacyPolicy, _termsUpdateInfoDefault)).Returns(false);
            mockSendEventLogStateRepository.Setup(x => x.IsExistNotSetEventType()).Returns(false);

            await unitUnderTest.NavigateNextAsync();

            mockUserDataRepository.Verify(x => x.IsAllAgreed(), Times.Once());
            mockTermsUpdateService.Verify(x => x.IsUpdated(TermsType.TermsOfService, _termsUpdateInfoDefault), Times.Never());
            mockTermsUpdateService.Verify(x => x.IsUpdated(TermsType.PrivacyPolicy, _termsUpdateInfoDefault), Times.Never());
            mockSendEventLogStateRepository.Verify(x => x.IsExistNotSetEventType(), Times.Never());
            mockNavigatoinService.Verify(x => x.NavigateAsync("/TutorialPage1", It.IsAny<INavigationParameters>()), Times.Once());
        }

        [Fact]
        public async Task NavigateNextAsyncTest_TermsOfService()
        {
            SplashNavigationService unitUnderTest = CreateService();

            unitUnderTest.Destination = Destination.HomePage;
            unitUnderTest.DestinationPageParameters = new NavigationParameters();

            mockTermsUpdateService.Setup(x => x.GetTermsUpdateInfo()).ReturnsAsync(_termsUpdateInfoDefault);

            await unitUnderTest.Prepare();
            mockTermsUpdateService.Verify(x => x.GetTermsUpdateInfo(), Times.Once());

            mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.TermsOfService, _termsUpdateInfoDefault)).Returns(true);
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.PrivacyPolicy, _termsUpdateInfoDefault)).Returns(true);
            mockSendEventLogStateRepository.Setup(x => x.IsExistNotSetEventType()).Returns(true);

            await unitUnderTest.NavigateNextAsync();

            mockUserDataRepository.Verify(x => x.IsAllAgreed(), Times.Once());
            mockTermsUpdateService.Verify(x => x.IsUpdated(TermsType.TermsOfService, _termsUpdateInfoDefault), Times.Once());
            mockTermsUpdateService.Verify(x => x.IsUpdated(TermsType.PrivacyPolicy, _termsUpdateInfoDefault), Times.Never());
            mockSendEventLogStateRepository.Verify(x => x.IsExistNotSetEventType(), Times.Never());
            mockNavigatoinService.Verify(x => x.NavigateAsync("/ReAgreeTermsOfServicePage",
                It.Is<INavigationParameters>(x =>
                    x.ContainsKey(ReAgreeTermsOfServicePage.TermsOfServiceDetailKey) &&
                    x.GetValue<TermsUpdateInfoModel.Detail>(ReAgreeTermsOfServicePage.TermsOfServiceDetailKey).Text == "Test: Terms of service")),
                Times.Once());
        }

        [Fact]
        public async Task NavigateNextAsyncTest_PrivacyPolicy()
        {
            SplashNavigationService unitUnderTest = CreateService();

            unitUnderTest.Destination = Destination.HomePage;
            unitUnderTest.DestinationPageParameters = new NavigationParameters();

            mockTermsUpdateService.Setup(x => x.GetTermsUpdateInfo()).ReturnsAsync(_termsUpdateInfoDefault);

            await unitUnderTest.Prepare();
            mockTermsUpdateService.Verify(x => x.GetTermsUpdateInfo(), Times.Once());

            mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.TermsOfService, _termsUpdateInfoDefault)).Returns(false);
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.PrivacyPolicy, _termsUpdateInfoDefault)).Returns(true);
            mockSendEventLogStateRepository.Setup(x => x.IsExistNotSetEventType()).Returns(true);

            await unitUnderTest.NavigateNextAsync();

            mockUserDataRepository.Verify(x => x.IsAllAgreed(), Times.Once());
            mockTermsUpdateService.Verify(x => x.IsUpdated(TermsType.TermsOfService, _termsUpdateInfoDefault), Times.Once());
            mockTermsUpdateService.Verify(x => x.IsUpdated(TermsType.PrivacyPolicy, _termsUpdateInfoDefault), Times.Once());
            mockSendEventLogStateRepository.Verify(x => x.IsExistNotSetEventType(), Times.Never());
            mockNavigatoinService.Verify(x => x.NavigateAsync("/ReAgreePrivacyPolicyPage",
                It.Is<INavigationParameters>(x =>
                    x.ContainsKey(ReAgreePrivacyPolicyPage.PrivacyPolicyDetailKey) &&
                    x.GetValue<TermsUpdateInfoModel.Detail>(ReAgreePrivacyPolicyPage.PrivacyPolicyDetailKey).Text == "Test: Privacy policy")),
                Times.Once());
        }

        [Fact]
        public async Task NavigateNextAsyncTest_EventLog()
        {
            SplashNavigationService unitUnderTest = CreateService();

            unitUnderTest.Destination = Destination.HomePage;
            unitUnderTest.DestinationPageParameters = new NavigationParameters();

            mockTermsUpdateService.Setup(x => x.GetTermsUpdateInfo()).ReturnsAsync(_termsUpdateInfoDefault);

            await unitUnderTest.Prepare();
            mockTermsUpdateService.Verify(x => x.GetTermsUpdateInfo(), Times.Once());

            mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.TermsOfService, _termsUpdateInfoDefault)).Returns(false);
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.PrivacyPolicy, _termsUpdateInfoDefault)).Returns(false);
            mockSendEventLogStateRepository.Setup(x => x.IsExistNotSetEventType()).Returns(true);

            await unitUnderTest.NavigateNextAsync();

            mockUserDataRepository.Verify(x => x.IsAllAgreed(), Times.Once());
            mockTermsUpdateService.Verify(x => x.IsUpdated(TermsType.TermsOfService, _termsUpdateInfoDefault), Times.Once());
            mockTermsUpdateService.Verify(x => x.IsUpdated(TermsType.PrivacyPolicy, _termsUpdateInfoDefault), Times.Once());
            mockSendEventLogStateRepository.Verify(x => x.IsExistNotSetEventType(), Times.Once());
            mockNavigatoinService.Verify(x => x.NavigateAsync("/EventLogCooperationPage", It.IsAny<INavigationParameters>()), Times.Once());
        }

        [Fact]
        public async Task NavigateNextAsyncTest_EventLogSetupLager()
        {
            SplashNavigationService unitUnderTest = CreateService();

            unitUnderTest.Destination = Destination.HomePage;
            unitUnderTest.DestinationPageParameters = new NavigationParameters();

            mockTermsUpdateService.Setup(x => x.GetTermsUpdateInfo()).ReturnsAsync(_termsUpdateInfoDefault);

            await unitUnderTest.Prepare();
            mockTermsUpdateService.Verify(x => x.GetTermsUpdateInfo(), Times.Once());

            mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.TermsOfService, _termsUpdateInfoDefault)).Returns(false);
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.PrivacyPolicy, _termsUpdateInfoDefault)).Returns(false);
            mockSendEventLogStateRepository.Setup(x => x.IsExistNotSetEventType()).Returns(true);

            await unitUnderTest.NavigateNextAsync(true);

            mockUserDataRepository.Verify(x => x.IsAllAgreed(), Times.Once());
            mockTermsUpdateService.Verify(x => x.IsUpdated(TermsType.TermsOfService, _termsUpdateInfoDefault), Times.Once());
            mockTermsUpdateService.Verify(x => x.IsUpdated(TermsType.PrivacyPolicy, _termsUpdateInfoDefault), Times.Once());
            mockSendEventLogStateRepository.Verify(x => x.IsExistNotSetEventType(), Times.Never());
            mockNavigatoinService.Verify(x => x.NavigateAsync(Destination.HomePage.ToPath(), It.IsAny<INavigationParameters>()), Times.Once());
        }

        [Fact]
        public async Task NavigateNextAsyncTest_Destination()
        {
            SplashNavigationService unitUnderTest = CreateService();

            unitUnderTest.Destination = Destination.HomePage;
            unitUnderTest.DestinationPageParameters = new NavigationParameters { { "test-key", "test-value" } };

            mockTermsUpdateService.Setup(x => x.GetTermsUpdateInfo()).ReturnsAsync(_termsUpdateInfoDefault);

            await unitUnderTest.Prepare();
            mockTermsUpdateService.Verify(x => x.GetTermsUpdateInfo(), Times.Once());

            mockUserDataRepository.Setup(x => x.IsAllAgreed()).Returns(true);
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.TermsOfService, _termsUpdateInfoDefault)).Returns(false);
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.PrivacyPolicy, _termsUpdateInfoDefault)).Returns(false);
            mockSendEventLogStateRepository.Setup(x => x.IsExistNotSetEventType()).Returns(false);

            await unitUnderTest.NavigateNextAsync();

            mockUserDataRepository.Verify(x => x.IsAllAgreed(), Times.Once());
            mockTermsUpdateService.Verify(x => x.IsUpdated(TermsType.TermsOfService, _termsUpdateInfoDefault), Times.Once());
            mockTermsUpdateService.Verify(x => x.IsUpdated(TermsType.PrivacyPolicy, _termsUpdateInfoDefault), Times.Once());
            mockSendEventLogStateRepository.Verify(x => x.IsExistNotSetEventType(), Times.Once());
            mockNavigatoinService.Verify(x => x.NavigateAsync(
                Destination.HomePage.ToPath(),
                It.Is<INavigationParameters>(x =>
                    x.ContainsKey("test-key") &&
                    x.GetValue<string>("test-key") == "test-value")),
                Times.Once());
        }

    }
}

