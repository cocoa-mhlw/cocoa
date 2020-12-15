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
        private readonly Mock<IUserDataService> mockUserDataService;
        private readonly Mock<ExposureNotificationService> mockExposureNotificationService;

        public NotifyOtherPageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockUserDataService = mockRepository.Create<IUserDataService>();
            mockExposureNotificationService = mockRepository.Create<ExposureNotificationService>();
        }

        private NotifyOtherPageViewModel CreateViewModel()
        {
            var mockHttpClientService = mockRepository.Create<IHttpClientService>();
            var exposureNotificationService = new ExposureNotificationService(null, mockLoggerService.Object, mockUserDataService.Object, null, mockHttpClientService.Object);

            return new NotifyOtherPageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                mockUserDataService.Object,
                exposureNotificationService);
        }

        [Theory]
        [InlineData("11111111", false, false, false)]
        [InlineData("11111111", true, false, true)]
        [InlineData("11111111", false, true, true)]
        [InlineData("1111111", true, false, false)]
        [InlineData("111111111", true, false, false)]
        public void CheckRegisterButtonEnableTest(string diagnosisUid, bool isVisibleWithSymptomsLayout, bool isVisibleNoSymptomsLayout, bool expectResult)
        {
            var vm = CreateViewModel();
            vm.DiagnosisUid = diagnosisUid;
            vm.IsVisibleWithSymptomsLayout = isVisibleWithSymptomsLayout;
            vm.IsVisibleNoSymptomsLayout = isVisibleNoSymptomsLayout;

            var result = vm.CheckRegisterButtonEnable();

            Assert.Equal(expectResult, result);
        }
    }
}
