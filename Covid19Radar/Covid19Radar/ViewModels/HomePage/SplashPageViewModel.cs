/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Services.Migration;
using Covid19Radar.Views;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class SplashPageViewModel : ViewModelBase
    {
        private readonly ILoggerService _loggerService;
        private readonly IMigrationService _migrationService;
        private readonly ISplashNavigationService _splashNavigatoinService;

        public SplashPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IMigrationService migrationService,
            ISplashNavigationService splashNavigationService
            ) : base(navigationService)
        {
            _loggerService = loggerService;
            _migrationService = migrationService;
            _splashNavigatoinService = splashNavigationService;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            _loggerService.StartMethod();

            base.OnNavigatedTo(parameters);

            await _migrationService.MigrateAsync();

            Destination destination = Destination.EndOfServiceNotice;
            if (parameters.ContainsKey(SplashPage.DestinationKey))
            {
                destination = parameters.GetValue<Destination>(SplashPage.DestinationKey);
            }

            _loggerService.Info($"Destination is set {destination}");
            _splashNavigatoinService.Destination = destination;
            _splashNavigatoinService.DestinationPageParameters = parameters;

            await _splashNavigatoinService.NavigateNextAsync();

            _loggerService.EndMethod();
        }
    }
}
