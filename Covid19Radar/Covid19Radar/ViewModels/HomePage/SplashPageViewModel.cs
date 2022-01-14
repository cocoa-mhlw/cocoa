/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Services.Migration;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SplashPageViewModel : ViewModelBase
    {
        private readonly ITermsUpdateService _termsUpdateService;
        private readonly ILoggerService _loggerService;
        private readonly IUserDataService _userDataService;
        private readonly IMigrationService _migrationService;
        private readonly IEssentialsService _essentialsService;
        private readonly IUserDataRepository _userDataRepository;

        private bool _enableContinue = false;
        public bool EnableContinue
        {
            get { return _enableContinue; }
            set
            {
                SetProperty(ref _enableContinue, value);
            }
        }

        private INavigationParameters _navigationParameters;
        private TermsUpdateInfoModel _termsUpdateInfo;

        public SplashPageViewModel(
            INavigationService navigationService,
            ITermsUpdateService termsUpdateService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            IUserDataService userDataService,
            IMigrationService migrationService,
            IEssentialsService essentialsService
            ) : base(navigationService)
        {
            _termsUpdateService = termsUpdateService;
            _loggerService = loggerService;
            _userDataRepository = userDataRepository;
            _userDataService = userDataService;
            _migrationService = migrationService;
            _essentialsService = essentialsService;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            _loggerService.StartMethod();

            _navigationParameters = parameters;

            _ = Task.Run(async () => {
                _termsUpdateInfo = await _termsUpdateService.GetTermsUpdateInfo();
                EnableContinue = true;
            });

            _loggerService.EndMethod();

        }

        public Func<string, BrowserLaunchMode, Task> BrowserOpenAsync = Browser.OpenAsync;

        public Command OpenGitHub => new Command(async () =>
        {
            var url = AppResources.UrlGitHubRepository;
            await BrowserOpenAsync(url, BrowserLaunchMode.External);
        });

        public Command OnClickContinue => new Command(() =>
        {
            _loggerService.StartMethod();

            NavigateToNextPage(_navigationParameters);

            _loggerService.EndMethod();
        });

        public Command OnClickUseStable => new Command(async () =>
        {
            _loggerService.StartMethod();

            var url = _essentialsService.StoreUrl;
            await BrowserOpenAsync(url, BrowserLaunchMode.External);

            _loggerService.EndMethod();
        });

        private async void NavigateToNextPage(INavigationParameters parameters)
        {
            _loggerService.StartMethod();

            base.OnNavigatedTo(parameters);

            await _migrationService.MigrateAsync();

            var destination = Destination.HomePage;
            if (parameters.ContainsKey(SplashPage.DestinationKey))
            {
                _loggerService.Info($"Destination is set {destination}");
                destination = parameters.GetValue<Destination>(SplashPage.DestinationKey);
            }

            if (_userDataRepository.IsAllAgreed())
            {
                _loggerService.Info("User data exists");

                if (_termsUpdateService.IsUpdated(TermsType.TermsOfService, _termsUpdateInfo))
                {
                    _loggerService.Info($"Transition to ReAgreeTermsOfServicePage");

                    var navigationParams = ReAgreeTermsOfServicePage.BuildNavigationParams(_termsUpdateInfo, destination, parameters);
                    _ = await NavigationService.NavigateAsync("/" + nameof(ReAgreeTermsOfServicePage), navigationParams);
                }
                else if (_termsUpdateService.IsUpdated(TermsType.PrivacyPolicy, _termsUpdateInfo))
                {
                    _loggerService.Info($"Transition to ReAgreePrivacyPolicyPage");

                    var navigationParams = ReAgreePrivacyPolicyPage.BuildNavigationParams(_termsUpdateInfo.PrivacyPolicy, destination, parameters);
                    _ = await NavigationService.NavigateAsync("/" + nameof(ReAgreePrivacyPolicyPage), navigationParams);
                }
                else
                {
                    _loggerService.Info($"Transition to {destination}");
                    _ = await NavigationService.NavigateAsync(destination.ToPath(), parameters);
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
