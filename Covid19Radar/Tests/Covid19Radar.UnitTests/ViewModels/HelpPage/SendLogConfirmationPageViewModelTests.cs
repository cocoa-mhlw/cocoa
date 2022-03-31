/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Net;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.ViewModels;
using Covid19Radar.Views;
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
        private readonly Mock<IHttpDataService> mockHttpDataService;

        public SendLogConfirmationPageViewModelTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);

            mockUserDialogs = mockRepository.Create<IUserDialogs>();
            UserDialogs.Instance = mockUserDialogs.Object;

            mockNavigationService = mockRepository.Create<INavigationService>();
            mockLogFileService = mockRepository.Create<ILogFileService>();
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockLogUploadService = mockRepository.Create<ILogUploadService>();
            mockHttpDataService = mockRepository.Create<IHttpDataService>();
        }

        private SendLogConfirmationPageViewModel CreateViewModel()
        {
            var vm = new SendLogConfirmationPageViewModel(
                mockNavigationService.Object,
                mockLoggerService.Object,
                mockLogFileService.Object,
                mockLogUploadService.Object
                )
            {
                BeginInvokeOnMainThread = new Action<Action>((a) => { a.Invoke(); }),
                TaskRun = new Func<Action, Task>((a) => { a.Invoke(); return Task.CompletedTask; })
            };
            return vm;
        }

        [Fact]
        public void OnClickConfirmLogCommandTests_Success()
        {
            var testLogId = "test-log-id";
            mockLogFileService.Setup(x => x.CreateLogId()).Returns(testLogId);

            var testZipFileName = "test-zip-file-name";
            mockLogFileService.Setup(x => x.CreateZipFileName(testLogId)).Returns(testZipFileName);
            var testPublicZipFileName = "test-public-zip-file-name";
            mockLogFileService.Setup(x => x.CreateZipFile(testZipFileName)).Returns(testPublicZipFileName);
            var testPublicZipFilePath = "test-public-zip-file-path";
            mockLogFileService.Setup(x => x.CopyLogUploadingFileToPublicPath(testZipFileName)).Returns(testPublicZipFilePath);

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(SendLogConfirmationPage.BuildNavigationParams(testLogId, testPublicZipFilePath));

            mockUserDialogs.Invocations.Clear();
            mockLogFileService.Invocations.Clear();

            Xamarin.Forms.Mocks.MockForms.Init(Device.Android);
            unitUnderTest.OnClickConfirmLogCommand.Execute(null);
        }

        [Fact]
        public void OnClickConfirmLogCommandTests_Failure()
        {
            var testLogId = "test-log-id";
            mockLogFileService.Setup(x => x.CreateLogId()).Returns(testLogId);

            var testZipFileName = "test-zip-file-name";
            mockLogFileService.Setup(x => x.CreateZipFileName(testLogId)).Returns(testZipFileName);
            var testPublicZipFileName = "test-public-zip-file-name";
            mockLogFileService.Setup(x => x.CreateZipFile(testZipFileName)).Returns(testPublicZipFileName);
            var testPublicZipFilePath = "test-public-zip-file-path";
            mockLogFileService.Setup(x => x.CopyLogUploadingFileToPublicPath(testZipFileName)).Returns(testPublicZipFilePath);

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(new NavigationParameters());

            mockUserDialogs.Invocations.Clear();
            mockLogFileService.Invocations.Clear();

            unitUnderTest.OnClickConfirmLogCommand.Execute(null);

            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), "OK", null), Times.Once());
        }

        [Fact]
        public void OnClickSendLogCommandTests_Success()
        {
            var testLogId = "test-log-id";
            mockLogFileService.Setup(x => x.CreateLogId()).Returns(testLogId);

            var testZipFileName = "test-zip-file-name";
            mockLogFileService.Setup(x => x.CreateZipFileName(testLogId)).Returns(testZipFileName);
            var testPublicZipFileName = "test-public-zip-file-name";
            mockLogFileService.Setup(x => x.CreateZipFile(testZipFileName)).Returns(testPublicZipFileName);
            var testPublicZipFilePath = "test-public-zip-file-path";
            mockLogFileService.Setup(x => x.CopyLogUploadingFileToPublicPath(testZipFileName)).Returns(testPublicZipFilePath);
            mockLogFileService.Setup(x => x.DeleteAllLogUploadingFiles()).Returns(true);

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(SendLogConfirmationPage.BuildNavigationParams(testLogId, testPublicZipFilePath));
            
            mockUserDialogs.Invocations.Clear();
            mockLogFileService.Invocations.Clear();

            var testResponse = new ApiResponse<LogStorageSas>(200, new LogStorageSas() { SasToken = "test-sas-token" });
            mockLogUploadService.Setup(x => x.GetLogStorageSas()).ReturnsAsync(testResponse);
            mockLogUploadService.Setup(x => x.UploadAsync(testPublicZipFilePath, testResponse.Result.SasToken)).ReturnsAsync(HttpStatusCode.Created);

            unitUnderTest.OnClickSendLogCommand.Execute(null);

            mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
            mockLogUploadService.Verify(x => x.UploadAsync(testPublicZipFilePath, testResponse.Result.SasToken), Times.Once());
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
            mockLogFileService.Setup(x => x.CreateZipFileName(testLogId)).Returns(testZipFileName);
            var testPublicZipFileName = "test-public-zip-file-name";
            mockLogFileService.Setup(x => x.CreateZipFile(testZipFileName)).Returns(testPublicZipFileName);
            var testPublicZipFilePath = "test-public-zip-file-path";
            mockLogFileService.Setup(x => x.CopyLogUploadingFileToPublicPath(testZipFileName)).Returns(testPublicZipFilePath);

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(SendLogConfirmationPage.BuildNavigationParams(testLogId, testPublicZipFilePath));

            mockUserDialogs.Invocations.Clear();
            mockLogFileService.Invocations.Clear();

            var testResponse = new ApiResponse<LogStorageSas>(200, new LogStorageSas() { SasToken = "test-sas-token" });
            mockLogUploadService.Setup(x => x.GetLogStorageSas()).ReturnsAsync(testResponse);
            mockLogUploadService.Setup(x => x.UploadAsync(testPublicZipFilePath, testResponse.Result.SasToken)).ReturnsAsync(HttpStatusCode.Forbidden);

            unitUnderTest.OnClickSendLogCommand.Execute(null);

            mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), "OK", null), Times.Once());
            mockLogUploadService.Verify(x => x.UploadAsync(testPublicZipFilePath, testResponse.Result.SasToken), Times.Once());
            mockLogFileService.Verify(x => x.DeleteAllLogUploadingFiles(), Times.Never());
            mockNavigationService.Verify(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>()), Times.Never());
        }

        [Fact]
        public void OnClickSendLogCommandTests_DeleteLogFalure()
        {
            var testLogId = "test-log-id";
            mockLogFileService.Setup(x => x.CreateLogId()).Returns(testLogId);

            var testZipFileName = "test-zip-file-name";
            mockLogFileService.Setup(x => x.CreateZipFileName(testLogId)).Returns(testZipFileName);
            var testPublicZipFileName = "test-public-zip-file-name";
            mockLogFileService.Setup(x => x.CreateZipFile(testZipFileName)).Returns(testPublicZipFileName);
            var testPublicZipFilePath = "test-public-zip-file-path";
            mockLogFileService.Setup(x => x.CopyLogUploadingFileToPublicPath(testZipFileName)).Returns(testPublicZipFilePath);
            mockLogFileService.Setup(x => x.DeleteAllLogUploadingFiles()).Returns(false);

            var unitUnderTest = CreateViewModel();
            unitUnderTest.Initialize(
                SendLogConfirmationPage.BuildNavigationParams(testLogId, testPublicZipFilePath)
                );

            mockUserDialogs.Invocations.Clear();
            mockLogFileService.Invocations.Clear();

            var testResponse = new ApiResponse<LogStorageSas>(200, new LogStorageSas() { SasToken = "test-sas-token" });
            mockLogUploadService.Setup(x => x.GetLogStorageSas()).ReturnsAsync(testResponse);
            mockLogUploadService.Setup(x => x.UploadAsync(testPublicZipFilePath, testResponse.Result.SasToken)).ReturnsAsync(HttpStatusCode.Created);

            unitUnderTest.OnClickSendLogCommand.Execute(null);

            mockUserDialogs.Verify(x => x.ShowLoading(It.IsAny<string>(), null), Times.Once());
            mockUserDialogs.Verify(x => x.HideLoading(), Times.Once());
            mockUserDialogs.Verify(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Never());
            mockLogUploadService.Verify(x => x.UploadAsync(testPublicZipFilePath, testResponse.Result.SasToken), Times.Once());
            mockLogFileService.Verify(x => x.DeleteAllLogUploadingFiles(), Times.Once());
            var expectedParameters = new NavigationParameters { { "logId", testLogId } };
            mockNavigationService.Verify(x => x.NavigateAsync("SendLogCompletePage?useModalNavigation=true/", expectedParameters), Times.Once());
        }
    }
}
