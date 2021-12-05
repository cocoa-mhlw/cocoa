/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Moq;
using Prism.Navigation;
using Acr.UserDialogs;
using Xunit;
using Covid19Radar.Resources;

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class NotifyOtherPageViewModelTests: IDisposable
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IDiagnosisKeyRegisterServer> mockDiagnosisKeyRegisterServer;
        private readonly Mock<IEssentialsService> mockEssentialsService;
        private readonly Mock<IUserDialogs> mockUserDialogs;
        private readonly Mock<IServerConfigurationRepository> mockServerConfigurationRepository;

        public NotifyOtherPageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockDiagnosisKeyRegisterServer = mockRepository.Create<IDiagnosisKeyRegisterServer>();
            mockEssentialsService = mockRepository.Create<IEssentialsService>();
            mockUserDialogs = mockRepository.Create<IUserDialogs>();
            UserDialogs.Instance = mockUserDialogs.Object;
            mockServerConfigurationRepository = mockRepository.Create<IServerConfigurationRepository>();
        }

        private NotifyOtherPageViewModel CreateViewModel()
        {
            return new NotifyOtherPageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                new MockExposureNotificationApiService(mockLoggerService.Object),
                mockDiagnosisKeyRegisterServer.Object,
                mockEssentialsService.Object,
                mockServerConfigurationRepository.Object,
                0
                );
        }

        public void Dispose()
        {
            UserDialogs.Instance = null;
        }

        [Theory]
        [InlineData("11111111", false, false, false)]
        [InlineData("11111111", true, false, true)]
        [InlineData("11111111", false, true, true)]
        [InlineData("1111111", true, false, false)]
        [InlineData("111111111", true, false, false)]
        public void CheckRegisterButtonEnableTest(string processingNumber, bool isVisibleWithSymptomsLayout, bool isVisibleNoSymptomsLayout, bool expectResult)
        {
            var vm = CreateViewModel();
            vm.ProcessingNumber = processingNumber;
            vm.IsVisibleWithSymptomsLayout = isVisibleWithSymptomsLayout;
            vm.IsVisibleNoSymptomsLayout = isVisibleNoSymptomsLayout;

            var result = vm.CheckRegisterButtonEnable();

            Assert.Equal(expectResult, result);
        }

        [Fact]
        public void CheckRegisterButtonMaxErrorCountReturnHomeTest()
        {
            mockUserDialogs
                .Setup(x => x.ConfirmAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    null)
                )
                .Returns(Task.FromResult(true));

            mockUserDialogs
                .Setup(x => x.AlertAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    null)
                )
                .Returns(Task.FromResult(true));

            var vm = CreateViewModel();
            vm.ProcessingNumber = "1";

            vm.OnClickRegister.Execute(null);
            vm.OnClickRegister.Execute(null);
            vm.OnClickRegister.Execute(null);

            mockUserDialogs.Verify(x => x.AlertAsync(
                AppResources.NotifyOtherPageDiag5Message,
                AppResources.ProcessingNumberErrorDiagTitle,
                AppResources.ButtonOk,
                null
            ), Times.Exactly(3));

            vm.OnClickRegister.Execute(null);

            mockUserDialogs.Verify(x => x.AlertAsync(
                AppResources.NotifyOtherPageDiagReturnHome,
                AppResources.NotifyOtherPageDiagReturnHomeTitle,
                AppResources.ButtonOk,
                null
            ), Times.Once());

            mockNavigationService.Verify(x => x.NavigateAsync("/MenuPage/NavigationPage/HomePage"), Times.Once());
        }
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    internal class MockExposureNotificationApiService : AbsExposureNotificationApiService
    {
        private const long DUMMY_EN_API_VERSION = 2;

        internal MockExposureNotificationApiService(
            ILoggerService loggerService
            ) : base(loggerService) {
        }

        public override async Task<IList<Chino.ExposureNotificationStatus>> GetStatusesAsync()
            => new List<Chino.ExposureNotificationStatus>();

        public override async Task<List<TemporaryExposureKey>> GetTemporaryExposureKeyHistoryAsync()
            => new List<TemporaryExposureKey>();

        public override async Task<long> GetVersionAsync()
            => DUMMY_EN_API_VERSION;

        public override async Task<bool> IsEnabledAsync()
            => true;

        public override async Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> keyFiles,
            CancellationTokenSource cancellationTokenSource = null
            )
            => ProvideDiagnosisKeysResult.Completed;

        public override async Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> keyFiles,
            ExposureConfiguration configuration,
            CancellationTokenSource cancellationTokenSource = null
            )
            => ProvideDiagnosisKeysResult.Completed;

        public override async Task<ProvideDiagnosisKeysResult> ProvideDiagnosisKeysAsync(
            List<string> keyFiles,
            ExposureConfiguration configuration,
            string token,
            CancellationTokenSource cancellationTokenSource = null
            )
            => ProvideDiagnosisKeysResult.Completed;

        public override async Task RequestPreAuthorizedTemporaryExposureKeyHistoryAsync()
        {
            // do nothing
        }

        public override async Task RequestPreAuthorizedTemporaryExposureKeyReleaseAsync()
        {
            // do nothing
        }

        public override async Task StartAsync()
        {
            // do nothing
        }

        public override async Task StopAsync()
        {
            // do nothing
        }
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}
