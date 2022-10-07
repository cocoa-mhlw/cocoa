// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Chino;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views.EndOfService;
using Prism.Navigation;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Covid19Radar.ViewModels.EndOfService
{
    public class TerminationOfUsePageViewModel : ViewModelBase
    {
        private readonly ILoggerService _loggerService;
        private readonly ILogFileService _logFileService;
        private readonly ISurveyService _surveyService;
        private readonly IDialogService _dialogService;

        private readonly IUserDataRepository _userDataRepository;
        private readonly IEventLogRepository _eventLogRepository;
        private readonly ISendEventLogStateRepository _sendEventLogStateRepository;
        private readonly IExposureDataRepository _exposureDataRepository;
        private readonly IExposureConfigurationRepository _exposureConfigurationRepository;

        private readonly AbsExposureNotificationApiService _absExposureNotificationApiService;
        private readonly AbsExposureDetectionBackgroundService _absExposureDetectionBackgroundService;
        private readonly AbsDataMaintainanceBackgroundService _absDataMaintainanceBackgroundService;

        private SurveyContent _surveyContent = null;

        private string _description1;
        public string Description1
        {
            get { return _description1; }
            set { SetProperty(ref _description1, value); }
        }

        public TerminationOfUsePageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            ILogFileService logFileService,
            ISurveyService surveyService,
            IDialogService dialogService,
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
            _surveyService = surveyService;
            _dialogService = dialogService;

            _userDataRepository = userDataRepository;
            _eventLogRepository = eventLogRepository;
            _sendEventLogStateRepository = sendEventLogStateRepository;
            _exposureDataRepository = exposureDataRepository;
            _exposureConfigurationRepository = exposureConfigurationRepository;

            _absExposureNotificationApiService = absExposureNotificationApiService;
            _absExposureDetectionBackgroundService = absExposureDetectionBackgroundService;
            _absDataMaintainanceBackgroundService = absDataMaintainanceBackgroundService;

            var daysOfUse = _userDataRepository.GetDaysOfUse();
            Description1 = string.Format(
                AppResources.TerminationOfUsePageDescription1,
                daysOfUse
                );
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.ContainsKey(TerminationOfUsePage.NavigationParameterNameSurveyContent))
            {
                _surveyContent = parameters.GetValue<SurveyContent>(TerminationOfUsePage.NavigationParameterNameSurveyContent);
            }
        }

        public IAsyncCommand OnTerminationButton => new AsyncCommand(async () =>
        {
            UserDialogs.Instance.ShowLoading(AppResources.LoadingTextDeleting);

            try
            {
                // Submit survey content if needed
                if (_surveyContent != null && !await _surveyService.SubmitSurvey(_surveyContent))
                {
                    await _dialogService.ShowNetworkConnectionErrorAsync();
                    _loggerService.Error("Failed submit survey");
                    return;
                }

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
            }
            catch (Exception ex)
            {
                _loggerService.Exception("Failed termination of use", ex);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }

            // Navigate to complete page
            await NavigationService.NavigateAsync($"/{nameof(TerminationOfUseCompletePage)}");
        });

        private async Task StopExposureNotificationAsync()
        {
            _loggerService.StartMethod();

            try
            {
                _ = await _absExposureNotificationApiService.StopExposureNotificationAsync();
            }
            catch (Exception ex)
            {
                _loggerService.Exception("Failed to stop of exposure notification", ex);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }
}

