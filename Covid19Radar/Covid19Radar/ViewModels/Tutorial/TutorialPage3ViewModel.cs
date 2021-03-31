/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Acr.UserDialogs;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage3ViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly IUserDataService userDataService;
        private readonly ITermsUpdateService termsUpdateService;

        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public TutorialPage3ViewModel(INavigationService navigationService, ILoggerService loggerService, IUserDataService userDataService, ITermsUpdateService termsUpdateService) : base(navigationService)
        {
            this.loggerService = loggerService;
            this.userDataService = userDataService;
            this.termsUpdateService = termsUpdateService;
        }
        public Command OnClickAgree => new Command(async () =>
        {
            loggerService.StartMethod();

            UserDialogs.Instance.ShowLoading(Resources.AppResources.LoadingTextRegistering);

            var registerResult = await userDataService.RegisterUserAsync();
            if (!registerResult)
            {
                loggerService.Error("Failed register");
                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync(Resources.AppResources.DialogNetworkConnectionError, Resources.AppResources.DialogNetworkConnectionErrorTitle, Resources.AppResources.ButtonOk);
                loggerService.EndMethod();
                return;
            }

            termsUpdateService.SaveLastUpdateDate(TermsType.TermsOfService, DateTime.Now);
            UserDialogs.Instance.HideLoading();
            await NavigationService.NavigateAsync(nameof(PrivacyPolicyPage));
            loggerService.EndMethod();
        });
    }
}