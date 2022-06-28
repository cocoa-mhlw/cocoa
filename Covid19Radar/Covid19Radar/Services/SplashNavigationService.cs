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
        Task Prepare();
        Task<INavigationResult> NavigateNextAsync();
    }

    public class SplashNavigationService : ISplashNavigationService
    {
        private readonly INavigationService _navigationService;
        private readonly IUserDataRepository _userDataRepository;
        private readonly ILoggerService _loggerService;
        private readonly ITermsUpdateService _termsUpdateService;
        private readonly ISendEventLogStateRepository _sendEventLogStateRepository;

        public Destination Destination { get; set; }
        public INavigationParameters DestinationPageParameters { get; set; }

        private TermsUpdateInfoModel _termsUpdateInfoModel;

        public SplashNavigationService(
            INavigationService navigationService,
            IUserDataRepository userDataRepository,
            ILoggerService loggerService,
            ITermsUpdateService termsUpdateService,
            ISendEventLogStateRepository sendEventLogStateRepository)
        {
            _navigationService = navigationService;
            _userDataRepository = userDataRepository;
            _termsUpdateService = termsUpdateService;
            _loggerService = loggerService;
            _sendEventLogStateRepository = sendEventLogStateRepository;
        }

        public async Task Prepare()
        {
            _loggerService.StartMethod();
            try
            {
                _termsUpdateInfoModel = await _termsUpdateService.GetTermsUpdateInfo();
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public async Task<INavigationResult> NavigateNextAsync()
        {
            _loggerService.StartMethod();

            try
            {
                string name;
                INavigationParameters parameters;

                if (_userDataRepository.IsAllAgreed())
                {
                    _loggerService.Info("Is all agreed");

                    if (_termsUpdateService.IsUpdated(TermsType.TermsOfService, _termsUpdateInfoModel))
                    {
                        name = $"/{nameof(ReAgreeTermsOfServicePage)}";
                        parameters = ReAgreeTermsOfServicePage.BuildNavigationParams(_termsUpdateInfoModel.TermsOfService);
                    }
                    else if (_termsUpdateService.IsUpdated(TermsType.PrivacyPolicy, _termsUpdateInfoModel))
                    {
                        name = $"/{nameof(ReAgreePrivacyPolicyPage)}";
                        parameters = ReAgreePrivacyPolicyPage.BuildNavigationParams(_termsUpdateInfoModel.PrivacyPolicy);
                    }
                    else if (_sendEventLogStateRepository.IsExistNotSetEventType())
                    {
                        name = $"/{nameof(EventLogCooperationPage)}";
                        parameters = EventLogCooperationPage.BuildNavigationParams(EventLogCooperationPage.TransitionReason.Splash);
                    }
                    else
                    {
                        name = Destination.ToPath();
                        parameters = DestinationPageParameters;
                    }
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

