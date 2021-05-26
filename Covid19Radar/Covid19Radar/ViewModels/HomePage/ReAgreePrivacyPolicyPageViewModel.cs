/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ReAgreePrivacyPolicyPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private ITermsUpdateService _termsUpdateService;

        private DateTime UpdateDateTime { get; set; }
        private string _updateText;
        public string UpdateText
        {
            get { return _updateText; }
            set { SetProperty(ref _updateText, value); }
        }

        public Func<string, BrowserLaunchMode, Task> BrowserOpenAsync = Browser.OpenAsync;

        public ReAgreePrivacyPolicyPageViewModel(INavigationService navigationService, ILoggerService loggerService, ITermsUpdateService termsUpdateService) : base(navigationService)
        {
            _termsUpdateService = termsUpdateService;
            this.loggerService = loggerService;
        }

        public Command OpenWebView => new Command(async () =>
        {
            loggerService.StartMethod();

            var url = Resources.AppResources.UrlPrivacyPolicy;
            await BrowserOpenAsync(url, BrowserLaunchMode.SystemPreferred);

            loggerService.EndMethod();
        });

        public Command OnClickReAgreeCommand => new Command(async () =>
        {
            loggerService.StartMethod();

            _termsUpdateService.SaveLastUpdateDate(TermsType.PrivacyPolicy, UpdateDateTime);
            _ = await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));

            loggerService.EndMethod();
        });

        public override void Initialize(INavigationParameters parameters)
        {
            loggerService.StartMethod();

            base.Initialize(parameters);
            TermsUpdateInfoModel.Detail updateInfo = (TermsUpdateInfoModel.Detail) parameters["updatePrivacyPolicyInfo"];
            UpdateDateTime = updateInfo.UpdateDateTime;
            UpdateText = updateInfo.Text;

            loggerService.EndMethod();
        }
    }
}
