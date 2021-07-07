/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Globalization;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Covid19Radar.Views;
using Moq;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class ReAgreePrivacyPolicyPageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<ITermsUpdateService> mockTermsUpdateService;

        public ReAgreePrivacyPolicyPageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockTermsUpdateService = mockRepository.Create<ITermsUpdateService>();
        }

        private ReAgreePrivacyPolicyPageViewModel CreateViewModel()
        {
            var vm = new ReAgreePrivacyPolicyPageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                mockTermsUpdateService.Object);
            return vm;
        }

        [Fact]
        public void InitializeTests()
        {
            var reAgreePrivacyPolicyPageViewModel = CreateViewModel();
            var updateInfo = new TermsUpdateInfoModel.Detail { Text = "test", UpdateDateTime = new DateTime(2020, 11, 02) };
            var param = new NavigationParameters
            {
                { "updatePrivacyPolicyInfo", updateInfo }
            };
            reAgreePrivacyPolicyPageViewModel.Initialize(param);

            Assert.Equal(updateInfo.Text, reAgreePrivacyPolicyPageViewModel.UpdateText);
        }

        [Fact]
        public void OpenWebViewCommandTests()
        {
            // The test is locale dependent
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

            var reAgreePrivacyPolicyPageViewModel = CreateViewModel();
            var param = new NavigationParameters
            {
                { "updatePrivacyPolicyInfo", new TermsUpdateInfoModel.Detail { Text = "", UpdateDateTime = DateTime.Now } }
            };
            reAgreePrivacyPolicyPageViewModel.Initialize(param);

            var actualCalls = 0;
            string actualUri = default;
            BrowserLaunchMode actualLaunchMode = default;
            reAgreePrivacyPolicyPageViewModel.BrowserOpenAsync = (uri, launchMode) =>
            {
                actualCalls++;
                actualUri = uri;
                actualLaunchMode = launchMode;
                return Task.CompletedTask;
            };

            reAgreePrivacyPolicyPageViewModel.OpenWebView.Execute(null);

            Assert.Equal(1, actualCalls);
            Assert.Equal("https://www.mhlw.go.jp/cocoa/privacypolicy_english.html", actualUri);
            Assert.Equal(BrowserLaunchMode.SystemPreferred, actualLaunchMode);
        }

        [Fact]
        public void OnClickReAgreeCommandTests()
        {
            var reAgreePrivacyPolicyPageViewModel = CreateViewModel();
            var updateInfo = new TermsUpdateInfoModel.Detail { Text = "", UpdateDateTime = DateTime.Now };
            var param = new NavigationParameters
            {
                { "updatePrivacyPolicyInfo", updateInfo }
            };
            reAgreePrivacyPolicyPageViewModel.Initialize(param);

            mockTermsUpdateService.Setup(x => x.SaveLastUpdateDate(TermsType.PrivacyPolicy, updateInfo.UpdateDateTime));
            reAgreePrivacyPolicyPageViewModel.OnClickReAgreeCommand.Execute(null);

            mockNavigationService.Verify(x => x.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage)), Times.Once());
        }
    }
}
