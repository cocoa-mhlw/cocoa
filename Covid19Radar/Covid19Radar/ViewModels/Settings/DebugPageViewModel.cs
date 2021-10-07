/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class DebugPageViewModel : ViewModelBase
    {
        private readonly ITermsUpdateService _termsUpdateService;
        private readonly IExposureConfigurationRepository _exposureConfigurationRepository;
        private readonly IUserDataRepository _userDataRepository;
        private readonly AbsExposureNotificationApiService _exposureNotificationApiService;
        private readonly AbsExposureDetectionBackgroundService _exposureDetectionBackgroundService;
        private readonly ICloseApplicationService _closeApplicationService;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;

        private string _debugInfo;
        public string DebugInfo
        {
            get { return _debugInfo; }
            set { SetProperty(ref _debugInfo, value); }
        }

        private string _serverConfigurationInfo;
        public string ServerConfigurationInfo
        {
            get { return _serverConfigurationInfo; }
            set { SetProperty(ref _serverConfigurationInfo, value); }
        }

        public async override void OnAppearing()
        {
            base.OnAppearing();

            await UpdateInfo();
        }

        public async Task UpdateInfo(string exception = "")
        {
            string os = Device.RuntimePlatform;
#if DEBUG
            os += ",DEBUG";
#endif
#if USE_MOCK
            os += ",USE_MOCK";
#endif

            // debug info for ./SplashPageViewModel.cs
            var termsUpdateInfo = await _termsUpdateService.GetTermsUpdateInfo() ?? new Model.TermsUpdateInfoModel();

            var termsOfServiceUpdateDateTime = "Not Available";
            if (termsUpdateInfo.TermsOfService != null)
            {
                termsOfServiceUpdateDateTime = termsUpdateInfo.TermsOfService.UpdateDateTime.ToString();
            }

            var privacyPolicyUpdateDateTime = "Not Available";
            if (termsUpdateInfo.PrivacyPolicy != null)
            {
                privacyPolicyUpdateDateTime = termsUpdateInfo.PrivacyPolicy.UpdateDateTime.ToString();
            }

            var lastProcessTekTimestampList = AppSettings.Instance.SupportedRegions.Select(async region =>
            {
                var ticks = await _userDataRepository.GetLastProcessDiagnosisKeyTimestampAsync(region);
                new LastProcessTekTimestamp()
                {
                    Region = region,
                    Ticks = ticks
                }.ToString();
            });

            string regionString = string.Join(",", AppSettings.Instance.SupportedRegions);
            string lastProcessTekTimestampsStr = string.Join("\n  ", lastProcessTekTimestampList);

            var exposureNotificationStatus = await _exposureNotificationApiService.IsEnabledAsync();

            var dailySummaryCount = (await _userDataRepository.GetDailySummariesAsync(AppConstants.DaysOfExposureInformationToDisplay)).Count();
            var exposureWindowCount = (await _userDataRepository.GetExposureWindowsAsync(AppConstants.DaysOfExposureInformationToDisplay)).Count();

            // ../../settings.json
            var settings = new[] {
                $"Build: {os}",
                $"Version: {AppSettings.Instance.AppVersion}",
                $"Region: {regionString}",
                $"CdnUrl: {AppSettings.Instance.CdnUrlBase}",
                $"ApiUrl: {AppSettings.Instance.ApiUrlBase}",
                $"TermsOfServiceUpdatedDateTime: {termsOfServiceUpdateDateTime}",
                $"PrivacyPolicyUpdatedDateTime: {privacyPolicyUpdateDateTime}",
                $"StartDate: {_userDataRepository.GetStartDate().ToLocalTime().ToString("F")}",
                $"DaysOfUse: {_userDataRepository.GetDaysOfUse()}",
                $"Legacy-V1 ExposureCount: {_userDataRepository.GetV1ExposureCount(AppConstants.DaysOfExposureInformationToDisplay)}",
                $"DailySummaryCount: {dailySummaryCount}",
                $"ExposureWindowCount: {exposureWindowCount}",
                $"LastProcessTekTimestamp: {lastProcessTekTimestampsStr}",
                $"ENstatus: {exposureNotificationStatus}",
                $"Now: {DateTime.Now.ToLocalTime().ToString("F")}",
                exception
            };
            DebugInfo = string.Join(Environment.NewLine, settings);

            // ServerConfiguration
            var serverRegions = string.Join(",", _serverConfigurationRepository.Regions);
            var diagnosisKeyListProvideServerUrls = _serverConfigurationRepository.DiagnosisKeyListProvideServerUrls
                .Select(url => $"    * {url}");

            var serverConfiguration = new[]
            {
                $"Regions: {serverRegions}",
                $"DiagnosisKeyRegisterApiEndpoint: {_serverConfigurationRepository.DiagnosisKeyRegisterApiEndpoint}",
                $"DiagnosisKeyListProvideServerEndpoint: {_serverConfigurationRepository.DiagnosisKeyListProvideServerEndpoint}",
                $"ExposureConfigurationUrl: {_serverConfigurationRepository.ExposureConfigurationUrl}",
                $"ExposureDataCollectServerEndpoint: {_serverConfigurationRepository.ExposureDataCollectServerEndpoint}",
                $"UserRegisterApiEndpoint: {_serverConfigurationRepository.UserRegisterApiEndpoint}",
                $"InquiryLogApiUrl: {_serverConfigurationRepository.InquiryLogApiEndpoint}",
            };
            ServerConfigurationInfo = string.Join(Environment.NewLine, serverConfiguration);
        }

        public DebugPageViewModel(
            INavigationService navigationService,
            ITermsUpdateService termsUpdateService,
            IExposureConfigurationRepository exposureConfigurationRepository,
            IUserDataRepository userDataRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            AbsExposureDetectionBackgroundService exposureDetectionBackgroundService,
            ICloseApplicationService closeApplicationService,
            IServerConfigurationRepository serverConfigurationRepository
            ) : base(navigationService)
        {
            Title = "Title:Debug";
            _termsUpdateService = termsUpdateService;
            _exposureConfigurationRepository = exposureConfigurationRepository;
            _userDataRepository = userDataRepository;
            _exposureNotificationApiService = exposureNotificationApiService;
            _exposureDetectionBackgroundService = exposureDetectionBackgroundService;
            _closeApplicationService = closeApplicationService;
            _serverConfigurationRepository = serverConfigurationRepository;
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            await _serverConfigurationRepository.LoadAsync();

            _ = UpdateInfo("Initialize");
        }

        public Command OnClickReload => new Command(async () => await UpdateInfo("Reload"));

        public Command OnClickStartExposureNotification => new Command(async () =>
        {
            UserDialogs.Instance.ShowLoading("Starting ExposureNotification...");
            await _exposureNotificationApiService.StartAsync();
            UserDialogs.Instance.HideLoading();
            await UpdateInfo("StartExposureNotification");
        });

        public Command OnClickFetchExposureKeyAsync => new Command(async () =>
        {
            var exception = "FetchExposureKeyAsync";
            try
            {
                await _exposureDetectionBackgroundService.ExposureDetectionAsync();
            }
            catch (Exception ex)
            {
                exception += $":Exception: {ex}";
            }
            await UpdateInfo(exception);
        });

        // see ../Settings/SettingsPageViewModel.cs
        public Command OnClickStopExposureNotification => new Command(async () =>
        {
            UserDialogs.Instance.ShowLoading("Stopping ExposureNotification...");
            await _exposureNotificationApiService.StopAsync();
            UserDialogs.Instance.HideLoading();
            await UpdateInfo("StopExposureNotification");
        });

        public Command OnClickRemoveStartDate => new Command(async () =>
        {
            _userDataRepository.RemoveStartDate();
            await UpdateInfo("RemoveStartDate");
        });

        public Command OnClickRemoveExposureInformation => new Command(async () =>
        {
            _userDataRepository.RemoveExposureInformation();
            await UpdateInfo("RemoveExposureInformation");
        });

        public Command OnClickRemoveConfiguration => new Command(async () =>
        {

            _exposureConfigurationRepository.RemoveExposureConfiguration();
             await UpdateInfo("RemoveConfiguration");
        });

        public Command OnClickRemoveLastProcessTekTimestamp => new Command(async () =>
        {
            await _userDataRepository.RemoveLastProcessDiagnosisKeyTimestampAsync();
            await UpdateInfo("RemoveLastProcessTekTimestamp");
        });

        public Command OnClickRemoveAllUpdateDate => new Command(async () =>
        {
            _userDataRepository.RemoveAllUpdateDate();
            await UpdateInfo("RemoveAllUpdateDate");
        });

        public Command OnClickQuit => new Command(() =>
        {
            Application.Current.Quit();
            _closeApplicationService.CloseApplication();
        });

        private class LastProcessTekTimestamp
        {
            internal string Region;

            internal long Ticks;

            internal DateTimeOffset DateTime
                => DateTimeOffset.FromUnixTimeMilliseconds(Ticks).ToLocalTime();

            public override string ToString()
                => $"{Region} - {DateTime:F}({Ticks})";
        }
    }
}
