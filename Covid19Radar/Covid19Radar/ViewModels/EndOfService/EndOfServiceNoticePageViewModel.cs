// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using Covid19Radar.Common;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views.EndOfService;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels.EndOfService
{
    public class EndOfServiceNoticePageViewModel : ViewModelBase
    {
        public string CheckDetailsLinkReadText => $"{AppResources.EndOfServiceNoticePageTextLink} {AppResources.Button}";

        private readonly ILoggerService _loggerService;

        public EndOfServiceNoticePageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService
            ) : base(navigationService)
        {
            _loggerService = loggerService;
        }

        public Command CheckDetailsCommand => new Command(async () =>
        {
            _loggerService.StartMethod();

            await Browser.OpenAsync(AppConstants.EndOfServiceCheckDetailsUrl, BrowserLaunchMode.SystemPreferred);

            _loggerService.EndMethod();
        });

        public Command OnTerminationProcedureButton => new Command(async () =>
        {
            _loggerService.StartMethod();

            if (DateTime.UtcNow.ToUnixEpoch() <= AppConstants.SURVEY_END_DATE_UTC.ToUnixEpoch() && Utils.IsCurrentUICultureJaJp())
            {
                await NavigationService.NavigateAsync(nameof(SurveyRequestPage));
            }
          else
            {
                await NavigationService.NavigateAsync(nameof(TerminationOfUsePage));
            }

            _loggerService.EndMethod();
        });
    }
}

