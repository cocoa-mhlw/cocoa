/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Acr.UserDialogs;
using Covid19Radar.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class DebugPageViewModel : ViewModelBase
    {
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
            string os;
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    os = "Android";
                    break;
                case Device.iOS:
                    os = "iOS";
                    break;
                default:
                    os = "unknown";
                    break;
            }
#if DEBUG
            os += ",DEBUG";
#endif
#if USE_MOCK
            os += ",USE_MOCK";
#endif

            // debug info for ./SplashPageViewModel.cs
            string agree;
            if (termsUpdateService.IsAllAgreed())
            {
                agree = "exists";// (mainly) navigate from SplashPage to HomePage
                var termsUpdateInfo = await termsUpdateService.GetTermsUpdateInfo();
                if (termsUpdateService.IsReAgree(TermsType.TermsOfService, termsUpdateInfo))
                {
                    agree += "-TermsOfService";
                }
                else if (termsUpdateService.IsReAgree(TermsType.PrivacyPolicy, termsUpdateInfo))
                {
                    agree += "-PrivacyPolicy";
                }
            }
            else
            {
                agree = "not exists"; // navigate from SplashPage to TutorialPage1
            }

            long ticks = exposureNotificationService.GetLastProcessTekTimestamp(AppSettings.Instance.SupportedRegions[0]);
            DateTimeOffset dt = DateTimeOffset.FromUnixTimeMilliseconds(ticks).ToOffset(new TimeSpan(9, 0, 0));
            //please check : offset is correct or not
            //cf: ../../../Covid19Radar.Android/Services/Logs/LogPeriodicDeleteServiceAndroid.cs
            string lastProcessTekTimestamp = dt.ToLocalTime().ToString("F");

            var exposureNotificationStatus = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();
            var exposureNotificationMessage = await exposureNotificationService.UpdateStatusMessageAsync();
            // ../../settings.json
            var str = new[] { "Build: " + os, "Ver: " + AppSettings.Instance.AppVersion,
                "Region: " + string.Join(",", AppSettings.Instance.SupportedRegions), "CdnUrl: " + AppSettings.Instance.CdnUrlBase,
                "ApiUrl: " + AppSettings.Instance.ApiUrlBase, "Agree: " + agree, "StartDate: " + userDataService.GetStartDate().ToLocalTime().ToString("F"),
                "DaysOfUse: " + userDataService.GetDaysOfUse(), "ExposureCount: " + exposureNotificationService.GetExposureCount(),
                "LastProcessTek: " + lastProcessTekTimestamp, " (long): " + ticks, "ENstatus: " + exposureNotificationStatus,
                "ENmessage: " + exposureNotificationMessage, "Now: " + DateTime.Now.ToLocalTime().ToString("F"), ex};
            DebugInfo = string.Join(Environment.NewLine, str);
        }
        public DebugPageViewModel(INavigationService navigationService, IUserDataService userDataService, ITermsUpdateService termsUpdateService, IExposureNotificationService exposureNotificationService) : base(navigationService)
        {
            Title = "Title:DebugPage";
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
            var str = $"StartExposureNotification: {result}";
            UserDialogs.Instance.HideLoading();
            await UserDialogs.Instance.AlertAsync(str, str, Resources.AppResources.ButtonOk);
            Info("StartExposureNotification");
        });
        public Command OnClickFetchExposureKeyAsync => new Command(async () =>
        {
            var exLog = "FetchExposureKeyAsync";
            try { await exposureNotificationService.FetchExposureKeyAsync(); }
            catch (Exception ex) { exLog += $":Exception: {ex}"; }
            Info(exLog);
        });

        // see ../Settings/SettingsPageViewModel.cs
        public Command OnClickStopExposureNotification => new Command(async () =>
        {
            UserDialogs.Instance.ShowLoading("Stopping ExposureNotification...");
            var result = await exposureNotificationService.StopExposureNotification();
            string str = "StopExposureNotification: " + result.ToString();
            UserDialogs.Instance.HideLoading();
            await UserDialogs.Instance.AlertAsync(str, str, Resources.AppResources.ButtonOk);
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
