/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using System;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class PrivacyPolicyPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly ITermsUpdateService termsUpdateService;

        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public PrivacyPolicyPageViewModel(INavigationService navigationService, ILoggerService loggerService, ITermsUpdateService termsUpdateService) : base(navigationService)
        {
            Title = AppResources.PrivacyPolicyPageTitle;
            Url = Resources.AppResources.UrlPrivacyPolicy;

            this.loggerService = loggerService;
            this.termsUpdateService = termsUpdateService;
        }

        public Command OnClickAgree => new Command(async () =>
        {
            loggerService.StartMethod();

            termsUpdateService.SaveLastUpdateDate(TermsType.PrivacyPolicy, DateTime.Now);
            await NavigationService.NavigateAsync(nameof(TutorialPage4));

            loggerService.EndMethod();
        });
    }
}
