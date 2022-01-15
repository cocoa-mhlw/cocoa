/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Acr.UserDialogs;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SendLogConfirmationPageViewModel : ViewModelBase
    {
        public string ConfirmingLogReadText => $"{AppResources.SendLogConfirmationPageTextLink1} {AppResources.Button}";

        private readonly ILoggerService loggerService;
        private readonly ILogFileService logFileService;
        private readonly ILogUploadService logUploadService;

        private string LogId { get; set; }
        private string ZipFilePath { get; set; }

        public SendLogConfirmationPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            ILogFileService logFileService,
            ILogUploadService logUploadService
            ) : base(navigationService)
        {
            this.loggerService = loggerService;
            this.logFileService = logFileService;
            this.logUploadService = logUploadService;
        }

        public Command OnClickConfirmLogCommand => new Command(async () =>
        {
            loggerService.StartMethod();

            string sharePath = logFileService.CopyLogUploadingFileToPublicPath(ZipFilePath);

            if (sharePath is null)
            {
                await UserDialogs.Instance.AlertAsync(
                    AppResources.FailedMessageToSaveOperatingInformation,
                    AppResources.Error,
                    AppResources.ButtonOk);
                return;
            }

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

            loggerService.EndMethod();
        });

        public Command OnClickSendLogCommand => new Command(async () =>
        {
            loggerService.StartMethod();
            try
            {
                // Upload log file.
                UserDialogs.Instance.ShowLoading(AppResources.Sending);

                var uploadResult = await logUploadService.UploadAsync(ZipFilePath);

                UserDialogs.Instance.HideLoading();

                if (!uploadResult)
                {
                    // Failed to create ZIP file
                    await UserDialogs.Instance.AlertAsync(
                        AppResources.FailedMessageToSendOperatingInformation,
                        AppResources.SendingError,
                        AppResources.ButtonOk);
                    return;
                }

                var deleteResult = logFileService.DeleteAllLogUploadingFiles();
                if (!deleteResult)
                {
                    // Failed to delete ZIP file (Processing can be continued)
                    loggerService.Warning("Failed to delete ZIP file.");
                }

                // Transition to log send completion page.
                var parameters = new NavigationParameters
                {
                    { "logId", LogId }
                };
                _ = await NavigationService.NavigateAsync($"{nameof(SendLogCompletePage)}?useModalNavigation=true/", parameters);
            }
            finally
            {
                loggerService.EndMethod();
            }
        });

        public override void Initialize(INavigationParameters parameters)
        {
            loggerService.StartMethod();

            base.Initialize(parameters);

            LogId = parameters.GetValue<string>(SendLogConfirmationPage.LogIdKey);
            ZipFilePath = parameters.GetValue<string>(SendLogConfirmationPage.ZipFilePathKey);

            loggerService.Info($"ZipFilePath: {ZipFilePath}");

            loggerService.EndMethod();
        }

        public override void Destroy()
        {
            loggerService.StartMethod();
            base.Destroy();
            logFileService.DeleteAllLogUploadingFiles();
            loggerService.EndMethod();
        }
    }
}
