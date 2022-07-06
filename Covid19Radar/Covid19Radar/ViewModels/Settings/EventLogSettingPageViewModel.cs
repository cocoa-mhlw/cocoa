// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class EventLogSettingPageViewModel : ViewModelBase
    {
        private const bool ExposureNotifyIsCheckedDefault = false;

        private bool _backButtonEnabled;
        public bool BackButtonEnabled
        {
            get => _backButtonEnabled;
            private set => SetProperty(ref _backButtonEnabled, value);
        }

        private bool _isVisibleTitleInContent = true;
        public bool IsVisibleTitleInContent
        {
            get => _isVisibleTitleInContent;
            private set => SetProperty(ref _isVisibleTitleInContent, value);
        }

        private bool _exposureNotifyIsChecked;
        public bool ExposureNotifyIsChecked
        {
            get => _exposureNotifyIsChecked;
            set => SetProperty(ref _exposureNotifyIsChecked, value);
        }

        private readonly ILoggerService _loggerService;
        private readonly IDialogService _dialogService;
        private readonly ISplashNavigationService _splashNavigationService;
        private readonly ISendEventLogStateRepository _sendEventLogStateRepository;

        private EventLogSettingPage.TransitionReason _transitionReason;

        public EventLogSettingPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IDialogService dialogService,
            ISplashNavigationService splashNavigationService,
            ISendEventLogStateRepository sendEventLogStateRepository
            ) : base(navigationService)
        {
            _loggerService = loggerService;
            _dialogService = dialogService;
            _splashNavigationService = splashNavigationService;
            _sendEventLogStateRepository = sendEventLogStateRepository;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            _loggerService.StartMethod();
            base.Initialize(parameters);

            try
            {
                if (parameters.ContainsKey(EventLogSettingPage.TransitionReasonKey))
                {
                    _transitionReason = parameters.GetValue<EventLogSettingPage.TransitionReason>(EventLogSettingPage.TransitionReasonKey);
                }

                BackButtonEnabled = _transitionReason == EventLogSettingPage.TransitionReason.Setting;
                IsVisibleTitleInContent = _transitionReason != EventLogSettingPage.TransitionReason.Setting;

                SendEventLogState exposureNotifiedState = _sendEventLogStateRepository.GetSendEventLogState(EventType.ExposureNotified);
                if (exposureNotifiedState == SendEventLogState.NotSet)
                {
                    ExposureNotifyIsChecked = ExposureNotifyIsCheckedDefault;
                }
                else
                {
                    ExposureNotifyIsChecked = exposureNotifiedState == SendEventLogState.Enable;
                }
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public Command OnExposureNotifyCheckedChanged => new Command(() => {
            _ = Task.Run(async () =>
            {
                await Task.Delay(100);
                SemanticExtensions.Announce(ExposureNotifyIsChecked ? AppResources.On : AppResources.Off);
            });
        });

        public Command OnExposureNotifyAccessibilityFocused => new Command(() => {
            var annouceText = new List<string> {
                ExposureNotifyIsChecked ? AppResources.On : AppResources.Off,
                AppResources.EventLogSettingPageSettingTitle1,
                AppResources.CheckBox };
            SemanticExtensions.Announce(string.Join(Environment.NewLine, annouceText));
        });

        public Command OnExposureNotifyTapped => new Command(() => {
            ExposureNotifyIsChecked = !ExposureNotifyIsChecked;
        });

        public IAsyncCommand OnClickSave => new AsyncCommand(async () =>
        {
            _loggerService.StartMethod();

            try
            {
                _sendEventLogStateRepository.SetSendEventLogState(EventType.ExposureNotified, ExposureNotifyIsChecked ? SendEventLogState.Enable : SendEventLogState.Disable);

                await _dialogService.ShowEventLogSaveCompletedAsync();

                switch (_transitionReason)
                {
                    case EventLogSettingPage.TransitionReason.Tutorial:
                        _ = await NavigationService.NavigateAsync(nameof(TutorialPage6));
                        break;
                    case EventLogSettingPage.TransitionReason.Splash:
                        _ = await _splashNavigationService.NavigateNextAsync();
                        break;
                    case EventLogSettingPage.TransitionReason.Setting:
                        _ = await NavigationService.GoBackAsync();
                        break;
                    default:
                        throw new ArgumentException($"EventLogSettingPage.TransitionReason is invalid. ({_transitionReason})");
                }
            }
            finally
            {
                _loggerService.EndMethod();
            }
        });
    }
}
