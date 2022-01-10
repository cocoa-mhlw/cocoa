/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class PrivacyPolicyPageViewModel : ViewModelBase
    {
        private readonly ILoggerService _loggerService;
        private readonly IUserDataRepository _userDataRepository;

        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public PrivacyPolicyPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository
            ) : base(navigationService)
        {
            Title = AppResources.PrivacyPolicyPageTitle;
            Url = AppResources.UrlPrivacyPolicy;

            _loggerService = loggerService;
            _userDataRepository = userDataRepository;
        }

        public Command OnClickAgree => new Command(async () =>
        {
            _loggerService.StartMethod();

            _userDataRepository.SaveLastUpdateDate(TermsType.PrivacyPolicy, DateTime.Now);

            INavigationParameters navigationParameters
                = SendLogSettingsPage.BuildNavigationParams(Destination.TutorialPage4_EnableExposureNotification);
            await NavigationService.NavigateAsync(
                Destination.SendLogSettingsPage.ToPath(),
                navigationParameters
                );

            _loggerService.EndMethod();
        });
    }
}
