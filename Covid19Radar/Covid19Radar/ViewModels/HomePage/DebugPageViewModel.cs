/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Acr.UserDialogs;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class DebugPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly IUserDataService userDataService;
        private readonly ITermsUpdateService termsUpdateService;
        private readonly IExposureNotificationService exposureNotificationService;
        private readonly IHttpDataService httpDataService;

        private string _debugInfo;
        public string DebugInfo
        {
            get { return _debugInfo; }
            set { SetProperty(ref _debugInfo, value); }
        }
        public async void info(string ex = "")
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
            string LastProcessTekTimestamp = dt.ToLocalTime().ToString("F");

            var en = await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync();
            var status = await exposureNotificationService.UpdateStatusMessageAsync();
            // ../../settings.json
            var str = new string[] { "Build: " + os, "Ver: " + AppSettings.Instance.AppVersion, "Region: " + string.Join(",", AppSettings.Instance.SupportedRegions), "CdnUrl: " + AppSettings.Instance.CdnUrlBase, "ApiUrl: " + AppSettings.Instance.ApiUrlBase, "Agree: " + agree, "StartDate: " + userDataService.GetStartDate().ToLocalTime().ToString("F"), "DaysOfUse: " + userDataService.GetDaysOfUse().ToString(), "ExposureCount: " + exposureNotificationService.GetExposureCount().ToString(), "LastProcessTek: " + LastProcessTekTimestamp, " (long): " + ticks.ToString(), "EN: " + en, "ENS: " + status, "Now: " + DateTime.Now.ToLocalTime().ToString("F"), ex};
            DebugInfo = string.Join(Environment.NewLine, str);
        }
        public DebugPageViewModel(IHttpDataService httpDataService, INavigationService navigationService, ILoggerService loggerService, IUserDataService userDataService, ITermsUpdateService termsUpdateService, IExposureNotificationService exposureNotificationService) : base(navigationService)
        {
            Title = "Title:DebugPage";
            this.loggerService = loggerService;
            this.userDataService = userDataService;
            this.termsUpdateService = termsUpdateService;
            this.exposureNotificationService = exposureNotificationService;
        }
        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            info("Initialize");
        }
        public Command OnClickReload => new Command(async () =>
        {
            info("Reload");
        });

        public Command OnClickStartExposureNotification => new Command(async () =>
        {
            UserDialogs.Instance.ShowLoading("Starting ExposureNotification...");
            var result = await exposureNotificationService.StartExposureNotification();
            string str = "StartExposureNotification: " + result.ToString();
            UserDialogs.Instance.HideLoading();
            await UserDialogs.Instance.AlertAsync(str, str, Resources.AppResources.ButtonOk);
            info("StartExposureNotification");
        });
        public Command OnClickFetchExposureKeyAsync => new Command(async () =>
        {
            string exLog = "FetchExposureKeyAsync";
            try { await exposureNotificationService.FetchExposureKeyAsync(); }
            catch (Exception ex) { exLog += ":Exception: " + ex.ToString(); }
            info(exLog);
        });

        // see ../Settings/SettingsPageViewModel.cs
        public Command OnClickStopExposureNotification => new Command(async () =>
        {
            UserDialogs.Instance.ShowLoading("Stopping ExposureNotification...");
            var result = await exposureNotificationService.StopExposureNotification();
            string str = "StopExposureNotification: " + result.ToString();
            UserDialogs.Instance.HideLoading();
            await UserDialogs.Instance.AlertAsync(str, str, Resources.AppResources.ButtonOk);
            info("StopExposureNotification");
        });

        public Command OnClickRemoveStartDate => new Command(async () =>
        {
            userDataService.RemoveStartDate();
            info("RemoveStartDate");
        });
        public Command OnClickRemoveExposureInformation => new Command(async () =>
        {
            exposureNotificationService.RemoveExposureInformation();
            info("RemoveExposureInformation");
        });
        public Command OnClickRemoveConfiguration => new Command(async () =>
        {
            exposureNotificationService.RemoveConfiguration();
            info("RemoveConfiguration");
        });
        public Command OnClickRemoveLastProcessTekTimestamp => new Command(async () =>
        {
            exposureNotificationService.RemoveLastProcessTekTimestamp();
            info("RemoveLastProcessTekTimestamp");
        });
        public Command OnClickRemoveAllUpdateDate => new Command(async () =>
        {
            termsUpdateService.RemoveAllUpdateDate();
            info("RemoveAllUpdateDate");
        });
        public Command OnClickQuit => new Command(async () =>
        {
            Application.Current.Quit();
            DependencyService.Get<ICloseApplication>().closeApplication();
        });
    }
}
