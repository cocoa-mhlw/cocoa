/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class InqueryPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly ILogFileService logFileService;
        private readonly ILogPathService logPathService;

        private readonly IEssentialsService essentialsService;

        public Func<string, BrowserLaunchMode, Task> BrowserOpenAsync = Browser.OpenAsync;
        public Func<string, string, string[], Task> ComposeEmailAsync { get; set; } = Email.ComposeAsync;

        public InqueryPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            ILogFileService logFileService,
            ILogPathService logPathService,
            IEssentialsService essentialsService
            ) : base(navigationService)
        {
            Title = AppResources.InqueryPageTitle;
            this.loggerService = loggerService;
            this.logFileService = logFileService;
            this.logPathService = logPathService;
            this.essentialsService = essentialsService;
        }

        public Command OnClickQuestionCommand => new Command(async () =>
        {
            loggerService.StartMethod();

            var uri = "https://www.mhlw.go.jp/stf/seisakunitsuite/bunya/kenkou_iryou/covid19_qa_kanrenkigyou_00009.html";
            await BrowserOpenAsync(uri, BrowserLaunchMode.SystemPreferred);

            loggerService.EndMethod();
        });

        public Command OnClickSendLogCommand => new Command(async () =>
        {
            loggerService.StartMethod();

            try
            {
                UserDialogs.Instance.ShowLoading(AppResources.Processing);

                var (logId, zipFilePath) = CreateZipFile();

                UserDialogs.Instance.HideLoading();

                if (zipFilePath is null)
                {
                    // Failed to create ZIP file
                    await UserDialogs.Instance.AlertAsync(
                        AppResources.FailedMessageToGetOperatingInformation,
                        AppResources.Error,
                        AppResources.ButtonOk);
                    return;
                }

                loggerService.Info($"zipFilePath: {zipFilePath}");

                INavigationParameters navigationParameters
                    = SendLogConfirmationPage.BuildNavigationParams(logId, zipFilePath);

                _ = await NavigationService.NavigateAsync(nameof(SendLogConfirmationPage), navigationParameters);
            }
            finally
            {
                loggerService.EndMethod();
            }
        });

        public Command OnClickShareLogCommand => new Command(async () =>
        {
            loggerService.StartMethod();

            try
            {
                UserDialogs.Instance.ShowLoading(AppResources.Processing);

                var (logId, zipFilePath) = CreateZipFile();

                UserDialogs.Instance.HideLoading();

                if (zipFilePath is null)
                {
                    // Failed to create ZIP file
                    await UserDialogs.Instance.AlertAsync(
                        AppResources.FailedMessageToGetOperatingInformation,
                        AppResources.Error,
                        AppResources.ButtonOk);
                    return;
                }

                loggerService.Info($"zipFilePath: {zipFilePath}");

                string sharePath = logFileService.CopyLogUploadingFileToPublicPath(zipFilePath);

                try
                {
                    await Share.RequestAsync(new ShareFileRequest
                    {
                        File = new ShareFile(sharePath)
                    });
                }
                catch (NotImplementedInReferenceAssemblyException exception)
                {
                    loggerService.Exception("NotImplementedInReferenceAssemblyException", exception);
                }
            }
            finally
            {
                loggerService.EndMethod();
            }
        });

        public Command OnClickEmailCommand => new Command(async () =>
        {
            loggerService.StartMethod();
            try
            {
                await ComposeEmailAsync(
                    AppResources.InquiryMailSubject,
                    CreateInquiryMailBody(),
                    new string[] { AppSettings.Instance.SupportEmail });

                loggerService.EndMethod();
            }
            catch (Exception ex)
            {
                loggerService.Exception("Exception", ex);
                loggerService.EndMethod();
            }
        });

        public Command OnClickAboutAppCommand => new Command(async () =>
        {
            loggerService.StartMethod();

            var uri = "https://www.mhlw.go.jp/stf/seisakunitsuite/bunya/cocoa_00138.html";
            await BrowserOpenAsync(uri, BrowserLaunchMode.SystemPreferred);

            loggerService.EndMethod();
        });

        private string CreateInquiryMailBody()
        {
            return AppResources.InquiryMailBody.Replace("\\r\\n", "\r\n")
                + AppResources.InquiryMailModelTitle + essentialsService.Model + "\r\n"
                + AppResources.InquiryMailOSTitle + essentialsService.Platform + "\r\n"
                + AppResources.InquiryMailOSVersionTitle + essentialsService.PlatformVersion + "\r\n"
                + AppResources.InquiryMailAppVersionTitle + essentialsService.AppVersion;
        }

        private (string, string) CreateZipFile()
        {
            string logId = logFileService.CreateLogId();
            string zipFileName = logFileService.CreateZipFileName(logId);

            logFileService.Rotate();

            var zipFilePath = logFileService.CreateZipFile(zipFileName);

            return (logId, zipFilePath);
        }
    }
}
