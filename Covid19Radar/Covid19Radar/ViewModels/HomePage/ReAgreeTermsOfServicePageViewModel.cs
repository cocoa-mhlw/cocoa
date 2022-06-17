﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ReAgreeTermsOfServicePageViewModel : ViewModelBase
    {
        private readonly ILoggerService _loggerService;
        private readonly ITermsUpdateService _termsUpdateService;
        private readonly IUserDataRepository _userDataRepository;
        private readonly ISendEventLogStateRepository _sendEventLogStateRepository;

        private TermsUpdateInfoModel UpdateInfo;
        private DateTime UpdateDateTimeUtc { get; set; }
        private string _updateText;
        public string UpdateText
        {
            get { return _updateText; }
            set { SetProperty(ref _updateText, value); }
        }

        private INavigationParameters _navigationParameters;

        public Func<string, BrowserLaunchMode, Task> BrowserOpenAsync = Browser.OpenAsync;

        public ReAgreeTermsOfServicePageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            ITermsUpdateService termsUpdateService,
            IUserDataRepository userDataRepository,
            ISendEventLogStateRepository sendEventLogStateRepository
            ) : base(navigationService)
        {
            _loggerService = loggerService;
            _termsUpdateService = termsUpdateService;
            _userDataRepository = userDataRepository;
            _sendEventLogStateRepository = sendEventLogStateRepository;
        }

        public Command OpenWebView => new Command(async () =>
        {
            _loggerService.StartMethod();

            var url = Resources.AppResources.UrlTermOfUse;
            await BrowserOpenAsync(url, BrowserLaunchMode.SystemPreferred);

            _loggerService.EndMethod();
        });

        public Command OnClickReAgreeCommand => new Command(async () =>
        {
            _loggerService.StartMethod();

            _userDataRepository.SaveLastUpdateDate(TermsType.TermsOfService, UpdateDateTimeUtc);

            Destination destination = Destination.HomePage;
            if (_navigationParameters.ContainsKey(ReAgreeTermsOfServicePage.DestinationKey))
            {
                destination = _navigationParameters.GetValue<Destination>(ReAgreeTermsOfServicePage.DestinationKey);
            }

            if (_termsUpdateService.IsUpdated(TermsType.PrivacyPolicy, UpdateInfo))
            {
                var navigationParams = ReAgreePrivacyPolicyPage.BuildNavigationParams(UpdateInfo.PrivacyPolicy, destination, _navigationParameters);
                _ = await NavigationService.NavigateAsync(nameof(ReAgreePrivacyPolicyPage), navigationParams);
            }
            else if (ISendEventLogStateRepository.IsExistNotSetEventType(_sendEventLogStateRepository))
            {
                _loggerService.Info($"Transition to SendLogSettingsPage");

                var navigationParams = SendLogSettingsPage.BuildNavigationParams(destination, _navigationParameters);
                _ = await NavigationService.NavigateAsync(Destination.SendLogSettingsPage.ToPath(), navigationParams);
            }
            else
            {
                _ = await NavigationService.NavigateAsync(destination.ToPath(), _navigationParameters);
            }

            _loggerService.EndMethod();
        });

        public override void Initialize(INavigationParameters parameters)
        {
            _loggerService.StartMethod();

            base.Initialize(parameters);
            UpdateInfo = (TermsUpdateInfoModel) parameters[ReAgreeTermsOfServicePage.UpdateInfoKey];
            UpdateDateTimeUtc = UpdateInfo.TermsOfService.UpdateDateTimeUtc;
            UpdateText = UpdateInfo.TermsOfService.Text;

            _navigationParameters = parameters;

            _loggerService.EndMethod();
        }
    }
}
