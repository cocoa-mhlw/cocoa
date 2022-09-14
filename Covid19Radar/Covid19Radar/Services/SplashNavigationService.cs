// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;

namespace Covid19Radar.Services
{
    public interface ISplashNavigationService
    {
        Destination Destination { get; set; }
        INavigationParameters DestinationPageParameters { get; set; }
        Task<INavigationResult> NavigateNextAsync(bool isSetupLaterEventLog = false);
    }

    public class SplashNavigationService : ISplashNavigationService
    {
        private readonly INavigationService _navigationService;
        private readonly IUserDataRepository _userDataRepository;
        private readonly ILoggerService _loggerService;

        public Destination Destination { get; set; }
        public INavigationParameters DestinationPageParameters { get; set; }

        public SplashNavigationService(
            INavigationService navigationService,
            IUserDataRepository userDataRepository,
            ILoggerService loggerService)
        {
            _navigationService = navigationService;
            _userDataRepository = userDataRepository;
            _loggerService = loggerService;
        }

        public async Task<INavigationResult> NavigateNextAsync(bool isSetupLaterEventLog = false)
        {
            _loggerService.StartMethod();

            try
            {
                string name;
                INavigationParameters parameters;

                if (_userDataRepository.IsAllAgreed())
                {
                    _loggerService.Info("Is all agreed");

                    name = Destination.ToPath();
                    parameters = DestinationPageParameters;
                }
                else
                {
                    name = $"/{nameof(TutorialPage1)}";
                    parameters = null;
                }

                _loggerService.Info($"Transition to {name}");
                return await _navigationService.NavigateAsync(name, parameters);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }
}

