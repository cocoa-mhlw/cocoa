/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Moq;
using Prism.Navigation;
using Xamarin.Forms;
using Xunit;

namespace Covid19Radar.UnitTests.ViewModels
{
    public class SendLogConfirmationPageViewModelTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<IUserDialogs> mockUserDialogs;
        private readonly Mock<INavigationService> mockNavigationService;
        private readonly Mock<ILogFileService> mockLogFileService;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<ILogUploadService> mockLogUploadService;
        private readonly Mock<ILogPathService> mockLogPathService;

        public SendLogConfirmationPageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);

            mockUserDialogs = mockRepository.Create<IUserDialogs>();
            UserDialogs.Instance = mockUserDialogs.Object;

            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLogFileService = mockRepository.Create<ILogFileService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockLogUploadService = mockRepository.Create<ILogUploadService>();
            mockLogPathService = mockRepository.Create<ILogPathService>();
        }

        private SendLogConfirmationPageViewModel CreateViewModel()
        {
            var vm = new SendLogConfirmationPageViewModel(
                mockNavigationService.Object,
                mockLogFileService.Object,
                mockLoggerService.Object,
                mockLogUploadService.Object,
                mockLogPathService.Object)
            {
                BeginInvokeOnMainThread = new Action<Action>((a) => { a.Invoke(); }),
                TaskRun = new Func<Action, Task>((a) => { a.Invoke(); return Task.CompletedTask; })
            };
            return vm;
        }

        [Fact]
        public void InitializeTests_CreateZipSuccess()
        {
            var testLogId = "test-log-id";
            mockLogFileService.Setup(x => x.CreateLogId()).Returns(testLogId);

            var testZipFileName = "test-zip-file-name";
            mockLogFileService.Setup(x => x.LogUploadingFileName(testLogId)).Returns(testZipFileName);
            mockLogFileService.Setup(x => x.CreateLogUploadingFileToTmpPath(testZipFileName)).Returns(true);

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            mockLogFileService.Verify(x => x.CreateLogId(), Times.Once());
            mockLogFileService.Verify(x => x.LogUploadingFileName(testLogId), Times.Once());
            mockLogFileService.Verify(x => x.CreateLogUploadingFileToTmpPath(testZipFileName), Times.Once());

            mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());

            mockNavigationService.Verify(x => x.NavigateAsync(It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public void InitializeTests_CreateZipFailure()
        {
            var testLogId = "test-log-id";
            mockLogFileService.Setup(x => x.CreateLogId()).Returns(testLogId);

            var testZipFileName = "test-zip-file-name";
            mockLogFileService.Setup(x => x.LogUploadingFileName(testLogId)).Returns(testZipFileName);
            mockLogFileService.Setup(x => x.CreateLogUploadingFileToTmpPath(testZipFileName)).Returns(false);

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            mockLogFileService.Verify(x => x.CreateLogId(), Times.Once());
            mockLogFileService.Verify(x => x.LogUploadingFileName(testLogId), Times.Once());
            mockLogFileService.Verify(x => x.CreateLogUploadingFileToTmpPath(testZipFileName), Times.Once());

            mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), "OK", null), Times.Once());

            mockNavigationService.Verify(x => x.GoBackAsync(), Times.Once());
        }

        [Fact]
        public void OnClickConfirmLogCommandTests_Success()
        {
            var testLogId = "test-log-id";
            mockLogFileService.Setup(x => x.CreateLogId()).Returns(testLogId);

            var testZipFileName = "test-zip-file-name";
            mockLogFileService.Setup(x => x.LogUploadingFileName(testLogId)).Returns(testZipFileName);
            mockLogFileService.Setup(x => x.CreateLogUploadingFileToTmpPath(testZipFileName)).Returns(true);
            mockLogFileService.Setup(x => x.CopyLogUploadingFileToPublicPath(testZipFileName)).Returns(true);

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            mockUserDialogs.Invocations.Clear();
            mockLogFileService.Invocations.Clear();

            Xamarin.Forms.Mocks.MockForms.Init(Device.Android);
            unitUnderTest.OnClickConfirmLogCommand.Execute(null);

            mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), "OK", null), Times.Once());
        }

        [Fact]
        public void OnClickConfirmLogCommandTests_Failure()
        {
            var testLogId = "test-log-id";
            mockLogFileService.Setup(x => x.CreateLogId()).Returns(testLogId);

            var testZipFileName = "test-zip-file-name";
            mockLogFileService.Setup(x => x.LogUploadingFileName(testLogId)).Returns(testZipFileName);
            mockLogFileService.Setup(x => x.CreateLogUploadingFileToTmpPath(testZipFileName)).Returns(true);
            mockLogFileService.Setup(x => x.CopyLogUploadingFileToPublicPath(testZipFileName)).Returns(false);

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            mockUserDialogs.Invocations.Clear();
            mockLogFileService.Invocations.Clear();

            unitUnderTest.OnClickConfirmLogCommand.Execute(null);

            mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), "OK", null), Times.Once());
        }

        [Fact]
        public void OnClickSendLogCommandTests_Success()
        {
            var testLogId = "test-log-id";
            mockLogFileService.Setup(x => x.CreateLogId()).Returns(testLogId);

            var testZipFileName = "test-zip-file-name";
            mockLogFileService.Setup(x => x.LogUploadingFileName(testLogId)).Returns(testZipFileName);
            mockLogFileService.Setup(x => x.CreateLogUploadingFileToTmpPath(testZipFileName)).Returns(true);
            mockLogFileService.Setup(x => x.CopyLogUploadingFileToPublicPath(testZipFileName)).Returns(true);
            mockLogFileService.Setup(x => x.DeleteAllLogUploadingFiles()).Returns(true);

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            mockUserDialogs.Invocations.Clear();
            mockLogFileService.Invocations.Clear();

            mockLogUploadService.Setup(x => x.UploadAsync(testZipFileName)).ReturnsAsync(true);

            unitUnderTest.OnClickSendLogCommand.Execute(null);

            mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
            mockLogUploadService.Verify(x => x.UploadAsync(testZipFileName), Times.Once());
            mockLogFileService.Verify(x => x.DeleteAllLogUploadingFiles(), Times.Once());
            var expectedParameters = new NavigationParameters { { "logId", testLogId } };
            mockNavigationService.Verify(x => x.NavigateAsync("SendLogCompletePage?useModalNavigation=true/", expectedParameters), Times.Once());
        }

        [Fact]
        public void OnClickSendLogCommandTests_Failure()
        {
            var testLogId = "test-log-id";
            mockLogFileService.Setup(x => x.CreateLogId()).Returns(testLogId);

            var testZipFileName = "test-zip-file-name";
            mockLogFileService.Setup(x => x.LogUploadingFileName(testLogId)).Returns(testZipFileName);
            mockLogFileService.Setup(x => x.CreateLogUploadingFileToTmpPath(testZipFileName)).Returns(true);
            mockLogFileService.Setup(x => x.CopyLogUploadingFileToPublicPath(testZipFileName)).Returns(true);

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            mockUserDialogs.Invocations.Clear();
            mockLogFileService.Invocations.Clear();

            mockLogUploadService.Setup(x => x.UploadAsync(testZipFileName)).ReturnsAsync(false);

            unitUnderTest.OnClickSendLogCommand.Execute(null);

            mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), "OK", null), Times.Once());
            mockLogUploadService.Verify(x => x.UploadAsync(testZipFileName), Times.Once());
            mockLogFileService.Verify(x => x.DeleteAllLogUploadingFiles(), Times.Never());
            mockNavigationService.Verify(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>()), Times.Never());
        }

        [Fact]
        public void OnClickSendLogCommandTests_DeleteLogFalure()
        {
            var testLogId = "test-log-id";
            mockLogFileService.Setup(x => x.CreateLogId()).Returns(testLogId);

            var testZipFileName = "test-zip-file-name";
            mockLogFileService.Setup(x => x.LogUploadingFileName(testLogId)).Returns(testZipFileName);
            mockLogFileService.Setup(x => x.CreateLogUploadingFileToTmpPath(testZipFileName)).Returns(true);
            mockLogFileService.Setup(x => x.CopyLogUploadingFileToPublicPath(testZipFileName)).Returns(true);
            mockLogFileService.Setup(x => x.DeleteAllLogUploadingFiles()).Returns(false);

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            mockUserDialogs.Invocations.Clear();
            mockLogFileService.Invocations.Clear();

            mockLogUploadService.Setup(x => x.UploadAsync(testZipFileName)).ReturnsAsync(true);

            unitUnderTest.OnClickSendLogCommand.Execute(null);

            mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
            mockLogUploadService.Verify(x => x.UploadAsync(testZipFileName), Times.Once());
            mockLogFileService.Verify(x => x.DeleteAllLogUploadingFiles(), Times.Once());
            var expectedParameters = new NavigationParameters { { "logId", testLogId } };
            mockNavigationService.Verify(x => x.NavigateAsync("SendLogCompletePage?useModalNavigation=true/", expectedParameters), Times.Once());
        }
    }
}
