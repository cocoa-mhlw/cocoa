/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class DebugPageViewModel : ViewModelBase
    {
        private class LastProcessTekTimestamp
        {
            internal string Region;

            internal long Ticks;

            internal DateTimeOffset DateTime
                => DateTimeOffset.FromUnixTimeMilliseconds(Ticks).ToLocalTime();

            public override string ToString()
                => $"{Region} - {DateTime:F}({Ticks})";
        }

        private readonly IUserDataService userDataService;
        private readonly ITermsUpdateService termsUpdateService;
        private readonly IExposureNotificationService exposureNotificationService;

        private string _debugInfo;
        public string DebugInfo
        {
            get { return _debugInfo; }
            set { SetProperty(ref _debugInfo, value); }
        }
        public async void Info(string ex = "")
        {
            string os = Device.RuntimePlatform;
#if DEBUG
            os += ",DEBUG";
#endif
#if USE_MOCK
            os += ",USE_MOCK";
#endif

            // debug info for ./SplashPageViewModel.cs
            var termsUpdateInfo = await termsUpdateService.GetTermsUpdateInfo();

            IList<string> lastProcessTekTimestampList = AppSettings.Instance.SupportedRegions.Select(region =>
                             new LastProcessTekTimestamp()
                             {
                                 Region = region,
                                 Ticks = exposureNotificationService.GetLastProcessTekTimestamp(region)
                             }.ToString()
            ).ToList();
            string lastProcessTekTimestampsStr = string.Join("\n  ", lastProcessTekTimestampList);

            var exposureNotificationStatus = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();
            var exposureNotificationMessage = await exposureNotificationService.UpdateStatusMessageAsync();
            var regionString = string.Join(",", AppSettings.Instance.SupportedRegions);

            // ../../settings.json
            var str = new[] {
                $"Build: {os}",
                $"Version: {AppSettings.Instance.AppVersion}",
                $"Region: {regionString}",
                $"CdnUrl: {AppSettings.Instance.CdnUrlBase}",
                $"ApiUrl: {AppSettings.Instance.ApiUrlBase}",
                $"TermsOfServiceUpdatedDateTime: {termsUpdateInfo.TermsOfService.UpdateDateTime}",
                $"PrivacyPolicyUpdatedDateTime: {termsUpdateInfo.PrivacyPolicy.UpdateDateTime}",
                $"StartDate: {userDataService.GetStartDate().ToLocalTime().ToString("F")}",
                $"DaysOfUse: {userDataService.GetDaysOfUse()}",
                $"ExposureCount: {exposureNotificationService.GetExposureCountToDisplay()}",
                $"LastProcessTekTimestamp: {lastProcessTekTimestampsStr}",
                $"ENstatus: {exposureNotificationStatus}",
                $"ENmessage: {exposureNotificationMessage}",
                $"Now: {DateTime.Now.ToLocalTime().ToString("F")}",
                ex
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
            this.userDataService = userDataService;
            this.termsUpdateService = termsUpdateService;
            this.exposureNotificationService = exposureNotificationService;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            Info("Initialize");
        }
        public Command OnClickReload => new Command(async () =>
        {
            Info("Reload");
        });

        public Command OnClickStartExposureNotification => new Command(async () =>
        {
            UserDialogs.Instance.ShowLoading("Starting ExposureNotification...");
            var result = await exposureNotificationService.StartExposureNotification();
            var message = $"Result: {result}";
            UserDialogs.Instance.HideLoading();
            await UserDialogs.Instance.AlertAsync(message, "StartExposureNotification", Resources.AppResources.ButtonOk);
            Info("StartExposureNotification");
        });
        public Command OnClickFetchExposureKeyAsync => new Command(async () =>
        {
            var exception = "FetchExposureKeyAsync";
            try {
                await exposureNotificationService.FetchExposureKeyAsync();
            }
            catch (Exception ex)
            {
                exception += $":Exception: {ex}";
            }
            Info(exception);
        });

        // see ../Settings/SettingsPageViewModel.cs
        public Command OnClickStopExposureNotification => new Command(async () =>
        {
            UserDialogs.Instance.ShowLoading("Stopping ExposureNotification...");
            var result = await exposureNotificationService.StopExposureNotification();
            string message = $"Result: {result}";
            UserDialogs.Instance.HideLoading();
            await UserDialogs.Instance.AlertAsync(message, "StopExposureNotification", Resources.AppResources.ButtonOk);
            Info("StopExposureNotification");
        });

        public Command OnClickRemoveStartDate => new Command(async () =>
        {
            userDataService.RemoveStartDate();
            Info("RemoveStartDate");
        });
        public Command OnClickRemoveExposureInformation => new Command(async () =>
        {
            exposureNotificationService.RemoveExposureInformation();
            Info("RemoveExposureInformation");
        });
        public Command OnClickRemoveConfiguration => new Command(async () =>
        {
            exposureNotificationService.RemoveConfiguration();
            Info("RemoveConfiguration");
        });
        public Command OnClickRemoveLastProcessTekTimestamp => new Command(async () =>
        {
            exposureNotificationService.RemoveLastProcessTekTimestamp();
            Info("RemoveLastProcessTekTimestamp");
        });
        public Command OnClickRemoveAllUpdateDate => new Command(async () =>
        {
            termsUpdateService.RemoveAllUpdateDate();
            Info("RemoveAllUpdateDate");
        });
        public Command OnClickQuit => new Command(async () =>
        {
            Application.Current.Quit();
            DependencyService.Get<ICloseApplication>().closeApplication();
        });
    }
}
