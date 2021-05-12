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

        public string StringAppSettings
    {
        get {
        string os = null;
        switch (Device.RuntimePlatform)
        {
            case Device.Android:
            os = "Android";
            break;
            case Device.iOS:
            os = "iOS";
            break;
        }
        string userData; // debug info for SplashPage
        if (termsUpdateService.IsAllAgreed()){
            userData = "exists";// (mainly) navigate from SplashPage to HomePage
        }else{
            userData = "not exists"; // navigate from SplashPage to TutorialPage1
        }
        long ticks =  exposureNotificationService.GetLastProcessTekTimestamp(AppSettings.Instance.SupportedRegions[0]);
        DateTimeOffset dt = DateTimeOffset.FromUnixTimeMilliseconds(ticks).ToOffset(new TimeSpan(9, 0, 0));
        //please check : offset is correct or not
        //cf: ../../../Covid19Radar.Android/Services/Logs/LogPeriodicDeleteServiceAndroid.cs
        string LastProcessTekTimestamp = dt.ToLocalTime().ToString("F");

        var str = new string[]
        {"build: "+os
#if DEBUG
         +",DEBUG"
#endif
#if USE_MOCK
         +",USE_MOCK"
#endif
         // ../../settings.json
         ,"ver: "+AppSettings.Instance.AppVersion
         ,"region: "+AppSettings.Instance.SupportedRegions[0]
         ,"cdnurl: "+AppSettings.Instance.CdnUrlBase
         ,"GetStart: "+userDataService.GetStartDate().ToLocalTime().ToString("F")
         ,"DaysOfUse: "+userDataService.GetDaysOfUse().ToString()
         ,"UserData: "+userData
         ,"Now: "+DateTime.Now.ToLocalTime().ToString("F")
         ,"LastProcessTek: "+LastProcessTekTimestamp
         ,"ExposureCount: "+exposureNotificationService.GetExposureCount().ToString()
        };
        return string.Join(Environment.NewLine,str);
        }
        set{}
    }
        public DebugPageViewModel(IHttpDataService httpDataService,INavigationService navigationService, ILoggerService loggerService, IUserDataService userDataService, ITermsUpdateService termsUpdateService,IExposureNotificationService exposureNotificationService) : base(navigationService)
        {
            Title = "Title:DebugPage";
            this.loggerService = loggerService;
            this.userDataService = userDataService;
            this.termsUpdateService = termsUpdateService;
            this.exposureNotificationService = exposureNotificationService;
        }
        public override async void Initialize(INavigationParameters parameters)
        {
            loggerService.StartMethod();
            try
            {
                await exposureNotificationService.StartExposureNotification();
                await exposureNotificationService.FetchExposureKeyAsync();
                var statusMessage = await exposureNotificationService.UpdateStatusMessageAsync();
                loggerService.Info($"Exposure notification status: {statusMessage}");
                base.Initialize(parameters);
                loggerService.EndMethod();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                loggerService.Exception("Failed to exposure notification status.", ex);
                loggerService.EndMethod();
            }
        }
    public Command OnClickReload => new Command(async () =>
       {
       await NavigationService.NavigateAsync(nameof(DebugPage));
       });
    public Command OnClickRemove => new Command(async () =>
       {
           loggerService.StartMethod();

       exposureNotificationService.RemoveLastProcessTekTimestamp();
       await NavigationService.NavigateAsync(nameof(DebugPage));

           loggerService.EndMethod();
       });
    }
}
