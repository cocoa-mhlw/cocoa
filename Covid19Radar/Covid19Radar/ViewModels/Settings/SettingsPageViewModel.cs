/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Chino;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
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

        private readonly IUserDataRepository userDataRepository;
        private readonly IExposureDataRepository exposureDataRepository;
        private readonly IExposureConfigurationRepository exposureConfigurationRepository;
        private readonly ILogFileService logFileService;
        private readonly AbsExposureNotificationApiService exposureNotificationApiService;
        private readonly ICloseApplicationService closeApplicationService;

        public string TermsOfUseReadText => $"{AppResources.TermsofservicePageTitle} {AppResources.Button}";
        public string PrivacyPolicyReadText => $"{AppResources.PrivacyPolicyPageTitle} {AppResources.Button}";
        public string WebAccessibilityPolicyReadText => $"{AppResources.WebAccessibilityPolicyPageTitle} {AppResources.Button}";

        public SettingsPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            IExposureDataRepository exposureDataRepository,
            IExposureConfigurationRepository exposureConfigurationRepository,
            ILogFileService logFileService,
            AbsExposureNotificationApiService exposureNotificationApiService,
            ICloseApplicationService closeApplicationService,
            IEssentialsService essentialsService
            ) : base(navigationService)
        {
            Title = AppResources.SettingsPageTitle;
            AppVer = essentialsService.AppVersion;
            this.loggerService = loggerService;
            this.userDataRepository = userDataRepository;
            this.exposureDataRepository = exposureDataRepository;
            this.exposureConfigurationRepository = exposureConfigurationRepository;
            this.logFileService = logFileService;
            this.exposureNotificationApiService = exposureNotificationApiService;
            this.closeApplicationService = closeApplicationService;
        }

        public IAsyncCommand OnEventLogSend => new AsyncCommand(async () =>
        {
            loggerService.StartMethod();

            INavigationParameters navigatinParameters = EventLogSettingPage.BuildNavigationParams(EventLogSettingPage.TransitionReason.Setting);
            _ = await NavigationService.NavigateAsync(nameof(EventLogSettingPage), navigatinParameters);

            loggerService.EndMethod();
        });

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

                await StopExposureNotificationAsync();

                // Reset All Data and Optout
                await exposureDataRepository.RemoveDailySummariesAsync();
                await exposureDataRepository.RemoveExposureWindowsAsync();
                exposureDataRepository.RemoveExposureInformation();
                await userDataRepository.RemoveLastProcessDiagnosisKeyTimestampAsync();
                await exposureConfigurationRepository.RemoveExposureConfigurationAsync();

                userDataRepository.RemoveStartDate();
                userDataRepository.RemoveAllUpdateDate();
                userDataRepository.RemoveAllExposureNotificationStatus();

                _ = logFileService.DeleteLogsDir();

                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync(
                    AppResources.SettingsPageDialogResetCompletedText,
                    AppResources.SettingsPageDialogResetCompletedTitle,
                    AppResources.ButtonOk
                    );
                Application.Current.Quit();
                // Application close
                closeApplicationService.CloseApplication();

                loggerService.EndMethod();
                return;
            }

            loggerService.EndMethod();
        });

        private async Task StopExposureNotificationAsync()
        {
            loggerService.StartMethod();

            try
            {
                _ = await exposureNotificationApiService.StopExposureNotificationAsync();
            }
            catch (ENException exception)
            {
                loggerService.Exception("ENException", exception);
            }
            finally
            {
                loggerService.EndMethod();
            }
        }

        public ICommand OnClickObtainSourceCode => new Command<string>(async (uri) =>
        {
            loggerService.StartMethod();

            await Browser.OpenAsync(uri, BrowserLaunchMode.External);

            loggerService.EndMethod();
        });
    }
}
