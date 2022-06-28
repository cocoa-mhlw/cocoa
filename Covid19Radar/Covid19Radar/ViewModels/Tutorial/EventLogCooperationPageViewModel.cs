// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Covid19Radar.ViewModels
{
    public class EventLogCooperationPageViewModel : ViewModelBase
    {
        public string SetupLaerLinkReadText => $"{Resources.AppResources.SetupLaerLink} {Resources.AppResources.Button}";

        private readonly ILoggerService _loggerService;
        private readonly ISplashNavigationService _splashNavigationService;
        private readonly ISendEventLogStateRepository _sendEventLogStateRepository;

        private EventLogCooperationPage.TransitionReason _transitionReason;

        public EventLogCooperationPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            ISplashNavigationService splashNavigationService,
            ISendEventLogStateRepository sendEventLogStateRepository            
            ) : base(navigationService)
        {
            _loggerService = loggerService;
            _splashNavigationService = splashNavigationService;
            _sendEventLogStateRepository = sendEventLogStateRepository;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            _loggerService.StartMethod();
            base.Initialize(parameters);

            try
            {
                if (parameters.ContainsKey(EventLogCooperationPage.TransitionReasonKey))
                {
                    _transitionReason = parameters.GetValue<EventLogCooperationPage.TransitionReason>(EventLogCooperationPage.TransitionReasonKey);
                }
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public IAsyncCommand OnClickSetting => new AsyncCommand(async () =>
        {
            _loggerService.StartMethod();

            try
            {
                EventLogSettingPage.TransitionReason transitionReason;
                switch (_transitionReason)
                {
                    case EventLogCooperationPage.TransitionReason.Tutorial:
                        transitionReason = EventLogSettingPage.TransitionReason.Tutorial;
                        break;
                    case EventLogCooperationPage.TransitionReason.Splash:
                        transitionReason = EventLogSettingPage.TransitionReason.Splash;
                        break;
                    default:
                        throw new ArgumentException($"EventLogCooperationPage.TransitionReason is invalid. ({_transitionReason})");
                }

                INavigationParameters navigatinParameters = EventLogSettingPage.BuildNavigationParams(transitionReason);
                _ = await NavigationService.NavigateAsync(nameof(EventLogSettingPage), navigatinParameters);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        });

        public IAsyncCommand OnClickSetLater => new AsyncCommand(async () =>
        {
            _loggerService.StartMethod();

            try
            {
                foreach (var eventType in EventType.All)
                {
                    if (_sendEventLogStateRepository.GetSendEventLogState(eventType) == SendEventLogState.NotSet)
                    {
                        _sendEventLogStateRepository.SetSendEventLogState(eventType, SendEventLogState.Disable);
                    }
                }

                switch (_transitionReason)
                {
                    case EventLogCooperationPage.TransitionReason.Tutorial:
                        _ = await NavigationService.NavigateAsync(nameof(TutorialPage6));
                        break;
                    case EventLogCooperationPage.TransitionReason.Splash:
                        _ = await _splashNavigationService.NavigateNextAsync();
                        break;
                    default:
                        throw new ArgumentException($"EventLogCooperationPage.TransitionReason is invalid. ({_transitionReason})");
                }
            }
            finally
            {
                _loggerService.EndMethod();
            }
        });
    }
}

