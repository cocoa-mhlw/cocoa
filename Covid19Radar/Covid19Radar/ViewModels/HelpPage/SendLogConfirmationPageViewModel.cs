/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SendLogConfirmationPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly ILogFileService logFileService;
        private readonly ILogUploadService logUploadService;
        private readonly ILogPathService logPathService;

        public Action<Action> BeginInvokeOnMainThread { get; set; } = MainThread.BeginInvokeOnMainThread;
        public Func<Action, Task> TaskRun { get; set; } = Task.Run;

        private string LogId { get; set; }
        private string ZipFileName { get; set; }

        public SendLogConfirmationPageViewModel(
            INavigationService navigationService,
            ILogFileService logFileService,
            ILoggerService loggerService,
            ILogUploadService logUploadService,
            ILogPathService logPathService) : base(navigationService)
        {
            this.loggerService = loggerService;
            this.logFileService = logFileService;
            this.logUploadService = logUploadService;
            this.logPathService = logPathService;
        }

        public Command OnClickConfirmLogCommand => new Command(() =>
        {
            loggerService.StartMethod();

            CopyZipFileToPublicPath();

            loggerService.EndMethod();
        });

        public Command OnClickSendLogCommand => new Command(async () =>
        {
            loggerService.StartMethod();
            try
            {
                // Upload log file.
                UserDialogs.Instance.ShowLoading(Resources.AppResources.Sending);

                var uploadResult = await logUploadService.UploadAsync(ZipFileName);

                UserDialogs.Instance.HideLoading();

                if (!uploadResult)
                {
                    // Failed to create ZIP file
                    await UserDialogs.Instance.AlertAsync(
                        Resources.AppResources.FailedMessageToSendOperatingInformation,
                        Resources.AppResources.SendingError,
                        Resources.AppResources.ButtonOk);
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
            CreateZipFile();

            loggerService.EndMethod();
        }

        public override void Destroy()
        {
            loggerService.StartMethod();
            base.Destroy();
            logFileService.DeleteAllLogUploadingFiles();
            loggerService.EndMethod();
        }

        private void CreateZipFile()
        {
            LogId = logFileService.CreateLogId();
            ZipFileName = logFileService.LogUploadingFileName(LogId);

            UserDialogs.Instance.ShowLoading(Resources.AppResources.Processing);

            _ = TaskRun(() =>
            {
                logFileService.Rotate();

                var result = logFileService.CreateLogUploadingFileToTmpPath(ZipFileName);

                BeginInvokeOnMainThread(async () =>
                {
                    UserDialogs.Instance.HideLoading();

                    if (!result)
                    {
                        // Failed to create ZIP file
                        await UserDialogs.Instance.AlertAsync(
                            Resources.AppResources.FailedMessageToGetOperatingInformation,
                            Resources.AppResources.Error,
                            Resources.AppResources.ButtonOk);

                        _ = await NavigationService.GoBackAsync();
                    }
                });
            });
        }

        private void CopyZipFileToPublicPath()
        {
            UserDialogs.Instance.ShowLoading(Resources.AppResources.Saving);

            _ = TaskRun(() =>
            {
                var result = logFileService.CopyLogUploadingFileToPublicPath(ZipFileName);

                BeginInvokeOnMainThread(async () =>
                {
                    UserDialogs.Instance.HideLoading();

                    if (!result)
                    {
                        await UserDialogs.Instance.AlertAsync(
                            Resources.AppResources.FailedMessageToSaveOperatingInformation,
                            Resources.AppResources.Error,
                            Resources.AppResources.ButtonOk);
                    }
                    else
                    {
                        string message = null;
                        switch (Device.RuntimePlatform)
                        {
                            case Device.Android:
                                message = Resources.AppResources.SuccessMessageToSaveOperatingInformationForAndroid + logPathService.LogUploadingPublicPath + Resources.AppResources.SuccessMessageToSaveOperatingInformationForAndroid2;
                                break;
                            case Device.iOS:
                                message = Resources.AppResources.SuccessMessageToSaveOperatingInformationForIOS;
                                break;
                        }

                        await UserDialogs.Instance.AlertAsync(
                            message,
                            Resources.AppResources.SaveCompleted,
                            Resources.AppResources.ButtonOk);
                    }
                });
            });
        }
    }
}
