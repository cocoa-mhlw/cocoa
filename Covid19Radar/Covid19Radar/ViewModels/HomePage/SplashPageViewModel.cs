/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Services.Migration;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SplashPageViewModel : ViewModelBase
    {
        private readonly ITermsUpdateService _termsUpdateService;
        private readonly ILoggerService _loggerService;
        private readonly IUserDataService _userDataService;
        private readonly IMigrationService _migrationService;
        private readonly IUserDataRepository _userDataRepository;

        public SplashPageViewModel(
            INavigationService navigationService,
            ITermsUpdateService termsUpdateService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            IUserDataService userDataService,
            IMigrationService migrationService
            ) : base(navigationService)
        {
            _termsUpdateService = termsUpdateService;
            _loggerService = loggerService;
            _userDataRepository = userDataRepository;
            _userDataService = userDataService;
            _migrationService = migrationService;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            _loggerService.StartMethod();

            base.OnNavigatedTo(parameters);

            await _migrationService.MigrateAsync();

            if (_userDataRepository.IsAllAgreed())
            {
                _loggerService.Info("User data exists");

                var termsUpdateInfo = await _termsUpdateService.GetTermsUpdateInfo();

                if (_userDataRepository.IsReAgree(TermsType.TermsOfService, termsUpdateInfo))
                {
                    var param = new NavigationParameters
                {
                    { "updateInfo", termsUpdateInfo }
                };
                    _loggerService.Info($"Transition to ReAgreeTermsOfServicePage");
                    _ = await NavigationService.NavigateAsync(nameof(ReAgreeTermsOfServicePage), param);
                }
                else if (_userDataRepository.IsReAgree(TermsType.PrivacyPolicy, termsUpdateInfo))
                {
                    var param = new NavigationParameters
                {
                    { "updatePrivacyPolicyInfo", termsUpdateInfo.PrivacyPolicy }
                };
                    _loggerService.Info($"Transition to ReAgreePrivacyPolicyPage");
                    _ = await NavigationService.NavigateAsync(nameof(ReAgreePrivacyPolicyPage), param);
                }
                else
                {
                    _loggerService.Info($"Transition to HomePage");
                    _ = await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
                }
            }
            else
            {
                _loggerService.Info("No user data exists");
                _loggerService.Info($"Transition to TutorialPage1");
                _ = await NavigationService.NavigateAsync("/" + nameof(TutorialPage1));
            }

            _loggerService.EndMethod();
        }
    }
}
