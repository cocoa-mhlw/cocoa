/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private string _AppVersion;

        public string AppVer
        {
            get { return _AppVersion; }
            set { SetProperty(ref _AppVersion, value); }
        }

        private readonly IExposureNotificationService exposureNotificationService;
        private readonly IUserDataRepository userDataRepository;
        private readonly ILogFileService logFileService;

        public SettingsPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            IExposureNotificationService exposureNotificationService,
            ILogFileService logFileService
            ) : base(navigationService)
        {
            Title = AppResources.SettingsPageTitle;
            AppVer = AppInfo.VersionString;
            this.loggerService = loggerService;
            this.userDataRepository = userDataRepository;
            this.exposureNotificationService = exposureNotificationService;
            this.logFileService = logFileService;
        }

        public ICommand OnChangeResetData => new Command(async () =>
        {
            loggerService.StartMethod();

            var check = await UserDialogs.Instance.ConfirmAsync(
                AppResources.SettingsPageDialogResetText,
                AppResources.SettingsPageDialogResetTitle,
                AppResources.ButtonOk,
                AppResources.ButtonCancel
            );
            if (check)
            {
                UserDialogs.Instance.ShowLoading(AppResources.LoadingTextDeleting);

                if (await ExposureNotification.IsEnabledAsync())
                {
                    await ExposureNotification.StopAsync();
                }

                // Reset All Data and Optout
                userDataRepository.RemoveStartDate();
                exposureNotificationService.RemoveExposureInformation();
                exposureNotificationService.RemoveConfiguration();
                userDataRepository.RemoveLastProcessTekTimestamp();
                userDataRepository.RemoveAllUpdateDate();

                _ = logFileService.DeleteLogsDir();

                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync(
                    AppResources.SettingsPageDialogResetCompletedText,
                    AppResources.SettingsPageDialogResetCompletedTitle,
                    AppResources.ButtonOk
                    );
                Application.Current.Quit();
                // Application close
                DependencyService.Get<ICloseApplication>().closeApplication();

                loggerService.EndMethod();
                return;
            }

            loggerService.EndMethod();
        });
    }
}
