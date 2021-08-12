/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Moq;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels.HomePage
{
    public class NotifyOtherPageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IExposureNotificationService> mockExposureNotificationService;
        private readonly Mock<ICloseApplication> mockCloseApplication;
        private readonly Mock<AbsExposureNotificationApiService> mockExposureNotificationApiService;
        private readonly Mock<DiagnosisKeyRegisterServer> mockDiagnosisKeyRegisterServer;
        private readonly Mock<IExposureNotificationEventSubject> mockExposureNotificationEventSubject;

        public NotifyOtherPageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockExposureNotificationService = mockRepository.Create<IExposureNotificationService>();
            mockCloseApplication = mockRepository.Create<ICloseApplication>();
            mockExposureNotificationApiService = mockRepository.Create<AbsExposureNotificationApiService>();
            mockDiagnosisKeyRegisterServer = mockRepository.Create<DiagnosisKeyRegisterServer>();
            mockExposureNotificationEventSubject = mockRepository.Create<IExposureNotificationEventSubject>();
        }

        private NotifyOtherPageViewModel CreateViewModel()
        {
            return new NotifyOtherPageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                mockExposureNotificationService.Object,
                mockCloseApplication.Object,
                mockExposureNotificationApiService.Object,
                mockDiagnosisKeyRegisterServer.Object,
                mockExposureNotificationEventSubject.Object
                );
        }

        [Theory]
        [InlineData("11111111", false, false, false)]
        [InlineData("11111111", true, false, true)]
        [InlineData("11111111", false, true, true)]
        [InlineData("1111111", true, false, false)]
        [InlineData("111111111", true, false, false)]
        public void CheckRegisterButtonEnableTest(string processNumber, bool isVisibleWithSymptomsLayout, bool isVisibleNoSymptomsLayout, bool expectResult)
        {
            var vm = CreateViewModel();
            vm.ProcessNumber = processNumber;
            vm.IsVisibleWithSymptomsLayout = isVisibleWithSymptomsLayout;
            vm.IsVisibleNoSymptomsLayout = isVisibleNoSymptomsLayout;

            var result = vm.CheckRegisterButtonEnable();

            Assert.Equal(expectResult, result);
        }
    }
}
