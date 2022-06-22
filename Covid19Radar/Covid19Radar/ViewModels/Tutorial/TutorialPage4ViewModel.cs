﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Chino;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage4ViewModel : ViewModelBase, IExposureNotificationEventCallback
    {
        public string TutorialPage4LinkReadText => $"{AppResources.TutorialPage4Link} {AppResources.Button}";

        private readonly IDialogService dialogService;
        private readonly ILoggerService loggerService;
        private readonly AbsExposureNotificationApiService exposureNotificationApiService;

        public TutorialPage4ViewModel(
            INavigationService navigationService,
            IDialogService dialogService,
            ILoggerService loggerService,
            AbsExposureNotificationApiService exposureNotificationApiService
            ) : base(navigationService)
        {
            this.dialogService = dialogService;
            this.loggerService = loggerService;
            this.exposureNotificationApiService = exposureNotificationApiService;
        }

        public Command OnClickEnable => new Command(async () =>
        {
            loggerService.StartMethod();

            try
            {
                var success = await exposureNotificationApiService.StartExposureNotificationAsync();
                if (success)
                {
                    await NavigationService.NavigateAsync(nameof(TutorialPage6));
                }
            }
            catch (ENException exception)
            {
                loggerService.Exception("ENException", exception);

                if (exception.Code == ENException.Code_Android.FAILED_NOT_SUPPORTED)
                {
                    ShowStatuses();
                    return;
                }
                await NavigationService.NavigateAsync(nameof(TutorialPage6));
            }
            finally
            {
                loggerService.EndMethod();
            }
        });

        private async void ShowStatuses()
        {
            var statusCodes = await exposureNotificationApiService.GetStatusCodesAsync();
            if (statusCodes.Contains(ExposureNotificationStatus.Code_Android.USER_PROFILE_NOT_SUPPORT))
            {
                await dialogService.ShowUserProfileNotSupportAsync();
            }
        }

        public Command OnClickDisable => new Command(async () =>
        {
            loggerService.StartMethod();
            await NavigationService.NavigateAsync(nameof(TutorialPage6));
            loggerService.EndMethod();
        });

        public async void OnEnabled()
        {
            loggerService.StartMethod();

            await exposureNotificationApiService.StartExposureNotificationAsync();
            await NavigationService.NavigateAsync(nameof(TutorialPage6));

            loggerService.EndMethod();
        }
    }
}
