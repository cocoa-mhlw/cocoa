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
        private readonly Mock<ITermsUpdateService> mockTermsUpdateService;
        private readonly Mock<IUserDataRepository> mockUserDataRepository;
        private readonly Mock<ISendEventLogStateRepository> mockSendEventLogStateRepository;

        public ReAgreeTermsOfServicePageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockTermsUpdateService = mockRepository.Create<ITermsUpdateService>();
            mockUserDataRepository = mockRepository.Create<IUserDataRepository>();
            mockSendEventLogStateRepository = mockRepository.Create<ISendEventLogStateRepository>();
        }

        private ReAgreeTermsOfServicePageViewModel CreateViewModel()
        {
            var vm = new ReAgreeTermsOfServicePageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                mockTermsUpdateService.Object,
                mockUserDataRepository.Object,
                mockSendEventLogStateRepository.Object
                );
            return vm;
        }

        [Fact]
        public void InitializeTests()
        {
            var reAgreeTermsOfServicePageViewModel = CreateViewModel();
            var updateInfo = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { Text = "利用規約テキスト", UpdateDateTimeJst = new DateTime(2020, 11, 01) },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { Text = "プライバシーポリシーテキスト", UpdateDateTimeJst = new DateTime(2020, 11, 02) }
            };

            var param = ReAgreeTermsOfServicePage.BuildNavigationParams(updateInfo, Destination.HomePage);
            reAgreeTermsOfServicePageViewModel.Initialize(param);

            Assert.Equal(updateInfo.TermsOfService.Text, reAgreeTermsOfServicePageViewModel.UpdateText);
        }

        [Fact]
        public void OpenWebViewCommandTests()
        {
            var reAgreeTermsOfServicePageViewModel = CreateViewModel();
            var updateInfo = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { Text = "利用規約テキスト", UpdateDateTimeJst = new DateTime(2020, 11, 01) },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { Text = "プライバシーポリシーテキスト", UpdateDateTimeJst = new DateTime(2020, 11, 02) }
            };

            var param = ReAgreeTermsOfServicePage.BuildNavigationParams(updateInfo, Destination.HomePage);
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
        public void OnClickReAgreeCommandTests_NavigateReAgreePrivacyPolicyPageViewModel()
        {
            var reAgreeTermsOfServicePageViewModel = CreateViewModel();
            var updateInfo = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { Text = "利用規約テキスト", UpdateDateTimeJst = new DateTime(2020, 11, 01) },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { Text = "プライバシーポリシーテキスト", UpdateDateTimeJst = new DateTime(2020, 11, 02) }
            };

            var param = ReAgreeTermsOfServicePage.BuildNavigationParams(updateInfo, Destination.HomePage);
            reAgreeTermsOfServicePageViewModel.Initialize(param);

            mockUserDataRepository.Setup(x => x.SaveLastUpdateDate(TermsType.TermsOfService, updateInfo.TermsOfService.UpdateDateTimeUtc));
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.PrivacyPolicy, updateInfo)).Returns(true);
            mockSendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(It.IsAny<EventType>()))
                .Returns(SendEventLogState.Disable);

            reAgreeTermsOfServicePageViewModel.OnClickReAgreeCommand.Execute(null);

            param.Add("updatePrivacyPolicyInfo", updateInfo.PrivacyPolicy);
            mockNavigationService.Verify(x => x.NavigateAsync(nameof(ReAgreePrivacyPolicyPage), It.IsAny<NavigationParameters>()), Times.Once());
        }

        [Fact]
        public void OnClickReAgreeCommandTests()
        {
            var reAgreeTermsOfServicePageViewModel = CreateViewModel();
            var updateInfo = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { Text = "利用規約テキスト", UpdateDateTimeJst = new DateTime(2020, 11, 01) },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { Text = "プライバシーポリシーテキスト", UpdateDateTimeJst = new DateTime(2020, 11, 02) }
            };

            var param = ReAgreeTermsOfServicePage.BuildNavigationParams(updateInfo, Destination.HomePage);
            reAgreeTermsOfServicePageViewModel.Initialize(param);

            mockUserDataRepository.Setup(x => x.SaveLastUpdateDate(TermsType.TermsOfService, updateInfo.TermsOfService.UpdateDateTimeUtc));
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.PrivacyPolicy, updateInfo)).Returns(false);
            mockSendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(It.IsAny<EventType>()))
                .Returns(SendEventLogState.Disable);

            reAgreeTermsOfServicePageViewModel.OnClickReAgreeCommand.Execute(null);

            mockNavigationService.Verify(x => x.NavigateAsync(Destination.HomePage.ToPath(), param), Times.Once());
        }

        [Fact]
        public void OnClickReAgreeCommandWithDestinationTest()
        {
            var reAgreeTermsOfServicePageViewModel = CreateViewModel();
            var updateInfo = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { Text = "利用規約テキスト", UpdateDateTimeJst = new DateTime(2020, 11, 01) },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { Text = "プライバシーポリシーテキスト", UpdateDateTimeJst = new DateTime(2020, 11, 02) }
            };

            var param = ReAgreeTermsOfServicePage.BuildNavigationParams(updateInfo, Destination.NotifyOtherPage);
            reAgreeTermsOfServicePageViewModel.Initialize(param);

            mockUserDataRepository.Setup(x => x.SaveLastUpdateDate(TermsType.TermsOfService, updateInfo.TermsOfService.UpdateDateTimeUtc));
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.PrivacyPolicy, updateInfo)).Returns(false);
            mockSendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(It.IsAny<EventType>()))
                .Returns(SendEventLogState.Disable);

            reAgreeTermsOfServicePageViewModel.OnClickReAgreeCommand.Execute(null);

            mockNavigationService.Verify(x => x.NavigateAsync(Destination.NotifyOtherPage.ToPath(), param), Times.Once());
        }

        [Fact]
        public void OnClickReAgreeCommandTests_NavigateReAgreePrivacyPolicyPageViewModel_NeedSendLogSetting()
        {
            var reAgreeTermsOfServicePageViewModel = CreateViewModel();
            var updateInfo = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { Text = "利用規約テキスト", UpdateDateTimeJst = new DateTime(2020, 11, 01) },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { Text = "プライバシーポリシーテキスト", UpdateDateTimeJst = new DateTime(2020, 11, 02) }
            };

            var param = ReAgreeTermsOfServicePage.BuildNavigationParams(updateInfo, Destination.HomePage);
            reAgreeTermsOfServicePageViewModel.Initialize(param);

            mockUserDataRepository.Setup(x => x.SaveLastUpdateDate(TermsType.TermsOfService, updateInfo.TermsOfService.UpdateDateTimeUtc));
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.PrivacyPolicy, updateInfo)).Returns(true);
            mockSendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(It.IsAny<EventType>()))
                .Returns(SendEventLogState.NotSet);

            reAgreeTermsOfServicePageViewModel.OnClickReAgreeCommand.Execute(null);

            param.Add("updatePrivacyPolicyInfo", updateInfo.PrivacyPolicy);
            mockNavigationService.Verify(x => x.NavigateAsync(nameof(ReAgreePrivacyPolicyPage), It.IsAny<NavigationParameters>()), Times.Once());
        }

        [Fact]
        public void OnClickReAgreeCommandTests_NeedSendLogSetting()
        {
            var reAgreeTermsOfServicePageViewModel = CreateViewModel();
            var updateInfo = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { Text = "利用規約テキスト", UpdateDateTimeJst = new DateTime(2020, 11, 01) },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { Text = "プライバシーポリシーテキスト", UpdateDateTimeJst = new DateTime(2020, 11, 02) }
            };

            var param = ReAgreeTermsOfServicePage.BuildNavigationParams(updateInfo, Destination.HomePage);
            reAgreeTermsOfServicePageViewModel.Initialize(param);

            mockUserDataRepository.Setup(x => x.SaveLastUpdateDate(TermsType.TermsOfService, updateInfo.TermsOfService.UpdateDateTimeUtc));
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.PrivacyPolicy, updateInfo)).Returns(false);
            mockSendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(It.IsAny<EventType>()))
                .Returns(SendEventLogState.NotSet);

            reAgreeTermsOfServicePageViewModel.OnClickReAgreeCommand.Execute(null);

            mockNavigationService.Verify(x => x.NavigateAsync(Destination.SendLogSettingsPage.ToPath(), It.IsAny<NavigationParameters>()), Times.Once());
        }

        [Fact]
        public void OnClickReAgreeCommandWithDestinationTest_NeedSendLogSetting()
        {
            var reAgreeTermsOfServicePageViewModel = CreateViewModel();
            var updateInfo = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { Text = "利用規約テキスト", UpdateDateTimeJst = new DateTime(2020, 11, 01) },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { Text = "プライバシーポリシーテキスト", UpdateDateTimeJst = new DateTime(2020, 11, 02) }
            };

            var param = ReAgreeTermsOfServicePage.BuildNavigationParams(updateInfo, Destination.NotifyOtherPage);
            reAgreeTermsOfServicePageViewModel.Initialize(param);

            mockUserDataRepository.Setup(x => x.SaveLastUpdateDate(TermsType.TermsOfService, updateInfo.TermsOfService.UpdateDateTimeUtc));
            mockTermsUpdateService.Setup(x => x.IsUpdated(TermsType.PrivacyPolicy, updateInfo)).Returns(false);
            mockSendEventLogStateRepository
                .Setup(x => x.GetSendEventLogState(It.IsAny<EventType>()))
                .Returns(SendEventLogState.NotSet);

            reAgreeTermsOfServicePageViewModel.OnClickReAgreeCommand.Execute(null);

            mockNavigationService.Verify(x => x.NavigateAsync(Destination.SendLogSettingsPage.ToPath(), It.IsAny<NavigationParameters>()), Times.Once());
        }
    }
}
