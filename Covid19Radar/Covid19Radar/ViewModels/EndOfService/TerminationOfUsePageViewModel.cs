// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Chino;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Prism.Navigation;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Covid19Radar.ViewModels.EndOfService
{
    public class TerminationOfUsePageViewModel : ViewModelBase
    {
        private readonly ILoggerService _loggerService;
        private readonly ILogFileService _logFileService;
        private readonly IUserDataRepository _userDataRepository;
        private readonly IEventLogRepository _eventLogRepository;
        private readonly ISendEventLogStateRepository _sendEventLogStateRepository;
        private readonly IExposureDataRepository _exposureDataRepository;
        private readonly IExposureConfigurationRepository _exposureConfigurationRepository;

        private readonly AbsExposureNotificationApiService _absExposureNotificationApiService;
        private readonly AbsExposureDetectionBackgroundService _absExposureDetectionBackgroundService;
        private readonly AbsDataMaintainanceBackgroundService _absDataMaintainanceBackgroundService;

        public TerminationOfUsePageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            ILogFileService logFileService,
            IUserDataRepository userDataRepository,
            IEventLogRepository eventLogRepository,
            ISendEventLogStateRepository sendEventLogStateRepository,
            IExposureDataRepository exposureDataRepository,
            IExposureConfigurationRepository exposureConfigurationRepository,
            AbsExposureNotificationApiService absExposureNotificationApiService,
            AbsExposureDetectionBackgroundService absExposureDetectionBackgroundService,
            AbsDataMaintainanceBackgroundService absDataMaintainanceBackgroundService
            ) : base(navigationService)
        {
            _loggerService = loggerService;
            _logFileService = logFileService;
            _userDataRepository = userDataRepository;
            _eventLogRepository = eventLogRepository;
            _sendEventLogStateRepository = sendEventLogStateRepository;
            _exposureDataRepository = exposureDataRepository;
            _exposureConfigurationRepository = exposureConfigurationRepository;

            _absExposureNotificationApiService = absExposureNotificationApiService;
            _absExposureDetectionBackgroundService = absExposureDetectionBackgroundService;
            _absDataMaintainanceBackgroundService = absDataMaintainanceBackgroundService;
        }

        public IAsyncCommand OnTerminationButton => new AsyncCommand(async () =>
        {
            try
            {
                UserDialogs.Instance.ShowLoading(AppResources.LoadingTextDeleting);

                // Stop exposure notifications
                await StopExposureNotificationAsync();

                // Cancel background tasks.
                _absExposureDetectionBackgroundService.Cancel();
                _absDataMaintainanceBackgroundService.Cancel();

                // Reset All Data and Optout
                await _exposureDataRepository.RemoveDailySummariesAsync();
                await _exposureDataRepository.RemoveExposureWindowsAsync();
                _exposureDataRepository.RemoveExposureInformation();
                await _userDataRepository.RemoveLastProcessDiagnosisKeyTimestampAsync();
                await _exposureConfigurationRepository.RemoveExposureConfigurationAsync();

                _userDataRepository.RemoveStartDate();
                _userDataRepository.RemoveAllUpdateDate();
                _userDataRepository.RemoveAllExposureNotificationStatus();

                _sendEventLogStateRepository.RemoveAll();
                await _eventLogRepository.RemoveAllAsync();

                _ = _logFileService.DeleteLogsDir();

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                _loggerService.Exception("Failed termination of use", ex);
            }
        });

        private async Task StopExposureNotificationAsync()
        {
            _loggerService.StartMethod();

            try
            {
                _ = await _absExposureNotificationApiService.StopExposureNotificationAsync();
            }
            catch (ENException exception)
            {
                _loggerService.Exception("ENException", exception);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }
}

