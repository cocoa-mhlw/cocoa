/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Moq;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels
{
    public class TutorialPage3ViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<IUserDialogs> mockUserDialogs;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IUserDataService> mockUserDataService;
        private readonly Mock<ITermsUpdateService> mockTermsUpdateService;


        public TutorialPage3ViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);

            mockUserDialogs = mockRepository.Create<IUserDialogs>();
            UserDialogs.Instance = mockUserDialogs.Object;

            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockUserDataService = mockRepository.Create<IUserDataService>();
            mockTermsUpdateService = mockRepository.Create<ITermsUpdateService>();
        }

        [Fact]
        public void Dispose()
        {
            mockUserDialogs.Reset();
            mockNavigationService.Reset();
            mockLoggerService.Reset();
            mockUserDataService.Reset();
            mockTermsUpdateService.Reset();

        }

        private TutorialPage3ViewModel CreateViewModel()
        {
            var vm = new TutorialPage3ViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                mockUserDataService.Object,
                mockTermsUpdateService.Object
            );
            return vm;
        }

        [Fact]
        public void OnClickAgreeSuccessTests()
        {
            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            mockUserDataService
                .Setup(product => product.RegisterUserAsync())
                .Returns(Task.Run(() => { return true; }));

            unitUnderTest.OnClickAgree.Execute(null);

            mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), "OK", null), Times.Never());

            mockNavigationService.Verify(x => x.NavigateAsync("PrivacyPolicyPage"), Times.Once);
        }

        [Fact]
        public void OnClickAgreeFailedTests()
        {
            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            mockUserDataService
                .Setup(product => product.RegisterUserAsync())
                .Returns(Task.Run(() =>　{　return false; }));

            unitUnderTest.OnClickAgree.Execute(null);

            mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), "OK", null), Times.Once());

            mockNavigationService.Verify(x => x.NavigateAsync("PrivacyPolicyPage"), Times.Never);
        }

    }
}
