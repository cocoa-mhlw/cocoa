/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Linq;
using Acr.UserDialogs;
using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class DebugPageViewModel : ViewModelBase
    {
        private readonly IUserDataService _userDataService;
        private readonly ITermsUpdateService _termsUpdateService＿;
        private readonly IExposureNotificationService _exposureNotificationService;

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

            var lastProcessTekTimestampList = AppSettings.Instance.SupportedRegions.Select(region =>
                             new LastProcessTekTimestamp()
                             {
                                 Region = region,
                                 Ticks = _exposureNotificationService.GetLastProcessTekTimestamp(region)
                             }.ToString()
                );

            string regionString = string.Join(",", AppSettings.Instance.SupportedRegions);
            string lastProcessTekTimestampsStr = string.Join("\n  ", lastProcessTekTimestampList);

            var exposureNotificationStatus = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();
            var exposureNotificationMessage = await _exposureNotificationService.UpdateStatusMessageAsync();

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
                $"ExposureCount: {_exposureNotificationService.GetExposureCountToDisplay()}",
                $"LastProcessTekTimestamp: {lastProcessTekTimestampsStr}",
                $"ENstatus: {exposureNotificationStatus}",
                $"ENmessage: {exposureNotificationMessage}",
                $"Now: {DateTime.Now.ToLocalTime().ToString("F")}",
                exception
            };
            DebugInfo = string.Join(Environment.NewLine, str);
        }

        public DebugPageViewModel(
            INavigationService navigationService,
            IUserDataService userDataService,
            ITermsUpdateService termsUpdateService,
            IExposureNotificationService exposureNotificationService
            ) : base(navigationService)
        {
            Title = "Title:Debug";
            _userDataService = userDataService;
            _termsUpdateService＿ = termsUpdateService;
            _exposureNotificationService = exposureNotificationService;
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
            var result = await _exposureNotificationService.StartExposureNotification();
            var message = $"Result: {result}";
            UserDialogs.Instance.HideLoading();
            await UserDialogs.Instance.AlertAsync(message, "StartExposureNotification", Resources.AppResources.ButtonOk);
            UpdateInfo("StartExposureNotification");
        });

        public Command OnClickFetchExposureKeyAsync => new Command(async () =>
        {
            var exception = "FetchExposureKeyAsync";
            try
            {
                await _exposureNotificationService.FetchExposureKeyAsync();
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
            var result = await _exposureNotificationService.StopExposureNotification();
            string message = $"Result: {result}";
            UserDialogs.Instance.HideLoading();
            await UserDialogs.Instance.AlertAsync(message, "StopExposureNotification", Resources.AppResources.ButtonOk);
            UpdateInfo("StopExposureNotification");
        });

        public Command OnClickRemoveStartDate => new Command(() =>
        {
            _userDataService.RemoveStartDate();
            UpdateInfo("RemoveStartDate");
        });

        public Command OnClickRemoveExposureInformation => new Command(() =>
        {
            _exposureNotificationService.RemoveExposureInformation();
            UpdateInfo("RemoveExposureInformation");
        });

        public Command OnClickRemoveConfiguration => new Command(() =>
        {
            _exposureNotificationService.RemoveConfiguration();
            UpdateInfo("RemoveConfiguration");
        });

        public Command OnClickRemoveLastProcessTekTimestamp => new Command(() =>
        {
            _exposureNotificationService.RemoveLastProcessTekTimestamp();
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
