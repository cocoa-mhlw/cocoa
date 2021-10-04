/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Linq;
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
        private readonly IUserDataService _userDataService;
        private readonly ITermsUpdateService _termsUpdateService＿;
        private readonly IExposureConfigurationRepository _exposureConfigurationRepository;
        private readonly IUserDataRepository _userDataRepository;
        private readonly AbsExposureNotificationApiService _exposureNotificationApiService;
        private readonly AbsExposureDetectionBackgroundService _exposureDetectionBackgroundService;

        private string _debugInfo;
        public string DebugInfo
        {
            get { return _debugInfo; }
            set { SetProperty(ref _debugInfo, value); }
        }

        public async void UpdateInfo(string exception = "")
        {
            string os = Device.RuntimePlatform;
#if DEBUG
            os += ",DEBUG";
#endif
#if USE_MOCK
            os += ",USE_MOCK";
#endif

            // debug info for ./SplashPageViewModel.cs
            var termsUpdateInfo = await _termsUpdateService＿.GetTermsUpdateInfo() ?? new Model.TermsUpdateInfoModel();

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
            var str = new[] {
                $"Build: {os}",
                $"Version: {AppSettings.Instance.AppVersion}",
                $"Region: {regionString}",
                $"CdnUrl: {AppSettings.Instance.CdnUrlBase}",
                $"ApiUrl: {AppSettings.Instance.ApiUrlBase}",
                $"TermsOfServiceUpdatedDateTime: {termsOfServiceUpdateDateTime}",
                $"PrivacyPolicyUpdatedDateTime: {privacyPolicyUpdateDateTime}",
                $"StartDate: {_userDataService.GetStartDate().ToLocalTime().ToString("F")}",
                $"DaysOfUse: {_userDataService.GetDaysOfUse()}",
                $"Legacy-V1 ExposureCount: {_userDataRepository.GetV1ExposureCount(AppConstants.DaysOfExposureInformationToDisplay)}",
                $"DailySummaryCount: {dailySummaryCount}",
                $"ExposureWindowCount: {exposureWindowCount}",
                $"LastProcessTekTimestamp: {lastProcessTekTimestampsStr}",
                $"ENstatus: {exposureNotificationStatus}",
                $"Now: {DateTime.Now.ToLocalTime().ToString("F")}",
                exception
            };
            DebugInfo = string.Join(Environment.NewLine, str);
        }

        public DebugPageViewModel(
            INavigationService navigationService,
            IUserDataService userDataService,
            ITermsUpdateService termsUpdateService,
            IExposureConfigurationRepository exposureConfigurationRepository,
            IUserDataRepository userDataRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            AbsExposureDetectionBackgroundService exposureDetectionBackgroundService
            ) : base(navigationService)
        {
            Title = "Title:Debug";
            _userDataService = userDataService;
            _termsUpdateService＿ = termsUpdateService;
            _exposureConfigurationRepository = exposureConfigurationRepository;
            _userDataRepository = userDataRepository;
            _exposureNotificationApiService = exposureNotificationApiService;
            _exposureDetectionBackgroundService = exposureDetectionBackgroundService;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            UpdateInfo("Initialize");
        }

        public Command OnClickReload => new Command(() => UpdateInfo("Reload"));

        public Command OnClickStartExposureNotification => new Command(async () =>
        {
            UserDialogs.Instance.ShowLoading("Starting ExposureNotification...");
            await _exposureNotificationApiService.StartAsync();
            UserDialogs.Instance.HideLoading();
            UpdateInfo("StartExposureNotification");
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
            UpdateInfo(exception);
        });

        // see ../Settings/SettingsPageViewModel.cs
        public Command OnClickStopExposureNotification => new Command(async () =>
        {
            UserDialogs.Instance.ShowLoading("Stopping ExposureNotification...");
            await _exposureNotificationApiService.StopAsync();
            UserDialogs.Instance.HideLoading();
            UpdateInfo("StopExposureNotification");
        });

        public Command OnClickRemoveStartDate => new Command(() =>
        {
            _userDataService.RemoveStartDate();
            UpdateInfo("RemoveStartDate");
        });

        public Command OnClickRemoveExposureInformation => new Command(() =>
        {
            _userDataRepository.RemoveExposureInformation();
            UpdateInfo("RemoveExposureInformation");
        });

        public Command OnClickRemoveConfiguration => new Command(() =>
        {

            _exposureConfigurationRepository.RemoveExposureConfiguration();
            UpdateInfo("RemoveConfiguration");
        });

        public Command OnClickRemoveLastProcessTekTimestamp => new Command(async () =>
        {
            await _userDataRepository.RemoveLastProcessDiagnosisKeyTimestampAsync();
            UpdateInfo("RemoveLastProcessTekTimestamp");
        });

        public Command OnClickRemoveAllUpdateDate => new Command(() =>
        {
            _termsUpdateService＿.RemoveAllUpdateDate();
            UpdateInfo("RemoveAllUpdateDate");
        });

        public Command OnClickQuit => new Command(() =>
        {
            Application.Current.Quit();
            DependencyService.Get<ICloseApplication>().closeApplication();
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
