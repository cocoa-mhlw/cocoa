/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Linq;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Services.Migration;
using Covid19Radar.Views;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class SplashPageViewModel : ViewModelBase
    {
        private readonly ITermsUpdateService _termsUpdateService;
        private readonly ILoggerService _loggerService;
        private readonly IMigrationService _migrationService;
        private readonly IUserDataRepository _userDataRepository;
        private readonly ISendEventLogStateRepository _sendEventLogStateRepository;

        public SplashPageViewModel(
            INavigationService navigationService,
            ITermsUpdateService termsUpdateService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            ISendEventLogStateRepository sendEventLogStateRepository,
            IMigrationService migrationService
            ) : base(navigationService)
        {
            _termsUpdateService = termsUpdateService;
            _loggerService = loggerService;
            _userDataRepository = userDataRepository;
            _sendEventLogStateRepository = sendEventLogStateRepository;
            _migrationService = migrationService;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            _loggerService.StartMethod();

            try
            {
                await _migrationService.MigrateAsync();

                if (!_userDataRepository.IsAllAgreed())
                {
                    _loggerService.Info("No user data exists");
                    _loggerService.Info($"Transition to TutorialPage1");
                    _ = await NavigationService.NavigateAsync("/" + nameof(TutorialPage1));
                    return;
                }

                _loggerService.Info("User data exists");

                var destination = Destination.HomePage;
                if (parameters.ContainsKey(SplashPage.DestinationKey))
                {
                    _loggerService.Info($"Destination is set {destination}");
                    destination = parameters.GetValue<Destination>(SplashPage.DestinationKey);
                }

                var termsUpdateInfo = await _termsUpdateService.GetTermsUpdateInfo();

                if (_termsUpdateService.IsUpdated(TermsType.TermsOfService, termsUpdateInfo))
                {
                    _loggerService.Info($"Transition to ReAgreeTermsOfServicePage");

                    var navigationParams = ReAgreeTermsOfServicePage.BuildNavigationParams(termsUpdateInfo, destination, parameters);
                    _ = await NavigationService.NavigateAsync("/" + nameof(ReAgreeTermsOfServicePage), navigationParams);
                    return;
                }
                else if (_termsUpdateService.IsUpdated(TermsType.PrivacyPolicy, termsUpdateInfo))
                {
                    _loggerService.Info($"Transition to ReAgreePrivacyPolicyPage");

                    var navigationParams = ReAgreePrivacyPolicyPage.BuildNavigationParams(termsUpdateInfo.PrivacyPolicy, destination, parameters);
                    _ = await NavigationService.NavigateAsync("/" + nameof(ReAgreePrivacyPolicyPage), navigationParams);
                    return;
                }

                bool isExistNotSetEventType = ISendEventLogStateRepository.EVENT_TYPE_ALL
                    .Select(eventType => _sendEventLogStateRepository.GetSendEventLogState(eventType))
                    .Any(state => state == SendEventLogState.NotSet);

                if (isExistNotSetEventType)
                {
                    _loggerService.Info($"Transition to SendLogSettingsPage");

                    var navigationParams = SendLogSettingsPage.BuildNavigationParams(destination, parameters);
                    _ = await NavigationService.NavigateAsync(Destination.SendLogSettingsPage.ToPath(), navigationParams);
                    return;
                }

                _loggerService.Info($"Transition to {destination}");
                _ = await NavigationService.NavigateAsync(destination.ToPath(), parameters);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }
}
