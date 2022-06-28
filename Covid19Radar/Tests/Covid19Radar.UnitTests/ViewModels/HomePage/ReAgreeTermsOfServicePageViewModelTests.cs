/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Covid19Radar.Views;
using Moq;
using Prism.Navigation;
using Xamarin.Essentials;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class ReAgreeTermsOfServicePageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IUserDataRepository> mockUserDataRepository;
        private readonly Mock<ISplashNavigationService> mockSplashNavigationService;

        public ReAgreeTermsOfServicePageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockUserDataRepository = mockRepository.Create<IUserDataRepository>();
            mockSplashNavigationService = mockRepository.Create<ISplashNavigationService>();
        }

        private ReAgreeTermsOfServicePageViewModel CreateViewModel()
        {
            var vm = new ReAgreeTermsOfServicePageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                mockUserDataRepository.Object,
                mockSplashNavigationService.Object
                );
            return vm;
        }

        [Fact]
        public void InitializeTests()
        {
            var reAgreeTermsOfServicePageViewModel = CreateViewModel();
            var termsOfServiceDetail = new TermsUpdateInfoModel.Detail { Text = "利用規約テキスト", UpdateDateTimeJst = new DateTime(2020, 11, 01) };

            var param = ReAgreeTermsOfServicePage.BuildNavigationParams(termsOfServiceDetail);
            reAgreeTermsOfServicePageViewModel.Initialize(param);

            Assert.Equal(termsOfServiceDetail.Text, reAgreeTermsOfServicePageViewModel.UpdateText);
        }

        [Fact]
        public void OpenWebViewCommandTests()
        {
            var reAgreeTermsOfServicePageViewModel = CreateViewModel();
            var termsOfServiceDetail = new TermsUpdateInfoModel.Detail { Text = "利用規約テキスト", UpdateDateTimeJst = new DateTime(2020, 11, 01) };

            var param = ReAgreeTermsOfServicePage.BuildNavigationParams(termsOfServiceDetail);
            reAgreeTermsOfServicePageViewModel.Initialize(param);

            var actualCalls = 0;
            string actualUri = default;
            BrowserLaunchMode actualLaunchMode = default;
            reAgreeTermsOfServicePageViewModel.BrowserOpenAsync = (uri, launchMode) =>
            {
                actualCalls++;
                actualUri = uri;
                actualLaunchMode = launchMode;
                return Task.CompletedTask;
            };

            reAgreeTermsOfServicePageViewModel.OpenWebView.Execute(null);

            Assert.Equal(1, actualCalls);
            Assert.Equal(AppResources.UrlTermOfUse, actualUri);
            Assert.Equal(BrowserLaunchMode.SystemPreferred, actualLaunchMode);
        }

        [Fact]
        public void OnClickReAgreeCommandTests()
        {
            var reAgreeTermsOfServicePageViewModel = CreateViewModel();
            var termsOfServiceDetail = new TermsUpdateInfoModel.Detail { Text = "利用規約テキスト", UpdateDateTimeJst = new DateTime(2020, 11, 01) };

            var param = ReAgreeTermsOfServicePage.BuildNavigationParams(termsOfServiceDetail);
            reAgreeTermsOfServicePageViewModel.Initialize(param);

            mockUserDataRepository.Setup(x => x.SaveLastUpdateDate(TermsType.TermsOfService, termsOfServiceDetail.UpdateDateTimeUtc));
            reAgreeTermsOfServicePageViewModel.OnClickReAgreeCommand.Execute(null);

            mockSplashNavigationService.Verify(x => x.NavigateNextAsync(), Times.Once());
        }
    }
}
