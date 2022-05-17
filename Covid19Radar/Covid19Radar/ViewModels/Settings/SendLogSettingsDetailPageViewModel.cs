// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SendLogSettingsDetailPageViewModel : ViewModelBase
    {
        private readonly ILoggerService _loggerService;

        private readonly ISendEventLogStateRepository _sendEventLogStateRepository;

        private Destination _destination = Destination.SettingsPage;
        private INavigationParameters _navigationParameters;

        private bool _enableExposureNotificationNotified = true;
        public bool EnableExposureNotificationNotified
        {
            get { return _enableExposureNotificationNotified; }
            set { SetProperty(ref _enableExposureNotificationNotified, value); }
        }

        private bool _enableExposureData = true;
        public bool EnableExposureData
        {
            get { return _enableExposureData; }
            set { SetProperty(ref _enableExposureData, value); }
        }

        public SendLogSettingsDetailPageViewModel(
            INavigationService navigationService,
            ISendEventLogStateRepository sendEventLogStateRepository,
            ILoggerService loggerService
            ) : base(navigationService)
        {
            _sendEventLogStateRepository = sendEventLogStateRepository;
            _loggerService = loggerService;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            _navigationParameters = parameters;

            if (parameters.ContainsKey(SendLogSettingsDetailPage.DestinationKey))
            {
                _destination = parameters.GetValue<Destination>(SendLogSettingsDetailPage.DestinationKey);
            }

        }

        public Command OnClickSave => new Command(async () =>
        {
            _loggerService.StartMethod();

            SendEventLogState exposureNotificationNotifiedState
                = EnableExposureNotificationNotified ? SendEventLogState.Enable : SendEventLogState.Disable;

            SendEventLogState exposureNotificationDataState
                = EnableExposureData ? SendEventLogState.Enable : SendEventLogState.Disable;

            _sendEventLogStateRepository.SetSendEventLogState(
                ISendEventLogStateRepository.EVENT_TYPE_EXPOSURE_NOTIFICATION_NOTIFIED,
                exposureNotificationNotifiedState
                );

            _sendEventLogStateRepository.SetSendEventLogState(
                ISendEventLogStateRepository.EVENT_TYPE_EXPOSURE_DATA,
                exposureNotificationDataState
                );

            _ = await NavigationService.NavigateAsync(_destination.ToPath(), _navigationParameters);

            _loggerService.EndMethod();

        });
    }
}
