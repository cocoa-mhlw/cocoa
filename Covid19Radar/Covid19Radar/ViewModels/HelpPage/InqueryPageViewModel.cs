/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class InqueryPageViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly IEssentialsService essentialsService;

        public Func<string, BrowserLaunchMode, Task> BrowserOpenAsync = Browser.OpenAsync;
        public Func<string, string, string[], Task> ComposeEmailAsync { get; set; } = Email.ComposeAsync;

        public InqueryPageViewModel(INavigationService navigationService, ILoggerService loggerService, IEssentialsService essentialsService) : base(navigationService)
        {
            Title = AppResources.InqueryPageTitle;
            this.loggerService = loggerService;
            this.essentialsService = essentialsService;
        }

        public Command OnClickQuestionCommand => new Command(async () =>
        {
            loggerService.StartMethod();

            var uri = "https://www.mhlw.go.jp/stf/seisakunitsuite/bunya/kenkou_iryou/covid19_qa_kanrenkigyou_00009.html";
            await BrowserOpenAsync(uri, BrowserLaunchMode.SystemPreferred);

            loggerService.EndMethod();
        });

        public Command OnClickSendLogCommand => new Command(async () =>
        {
            loggerService.StartMethod();

            _ = await NavigationService.NavigateAsync(nameof(SendLogConfirmationPage));

            loggerService.EndMethod();
        });

        public Command OnClickEmailCommand => new Command(async () =>
        {
            loggerService.StartMethod();
            try
            {
                await ComposeEmailAsync(
                    AppResources.InquiryMailSubject,
                    CreateInquiryMailBody(),
                    new string[] { AppSettings.Instance.SupportEmail });

                loggerService.EndMethod();
            }
            catch (Exception ex)
            {
                loggerService.Exception("Exception", ex);
                loggerService.EndMethod();
            }
        });

        public Command OnClickAboutAppCommand => new Command(async () =>
        {
            loggerService.StartMethod();

            var uri = "https://www.mhlw.go.jp/stf/seisakunitsuite/bunya/cocoa_00138.html";
            await BrowserOpenAsync(uri, BrowserLaunchMode.SystemPreferred);

            loggerService.EndMethod();
        });

        private string CreateInquiryMailBody()
        {
            return AppResources.InquiryMailBody.Replace("\\r\\n", "\r\n")
                + "モデル名：（" + essentialsService.Model + "）\r\n"
                + "OS：（" + essentialsService.Platform + "）\r\n"
                + "OSバージョン：（" + essentialsService.PlatformVersion + "）\r\n"
                + "アプリバージョン：（" + essentialsService.AppVersion + "）";
        }
    }
}
