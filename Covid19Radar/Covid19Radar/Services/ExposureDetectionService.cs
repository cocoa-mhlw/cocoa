/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public interface IExposureDetectionService
    {
        public void DiagnosisKeysDataMappingApplied();

        public void PreExposureDetected(ExposureConfiguration exposureConfiguration, long enVersion);

        public Task ExposureDetectedAsync(ExposureConfiguration exposureConfiguration, long enVersion, IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows);

        public Task ExposureDetectedAsync(ExposureConfiguration exposureConfiguration, long enVersion, ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations);

        public Task ExposureNotDetectedAsync(ExposureConfiguration exposureConfiguration, long enVersion);
    }

    public class ExposureDetectionService : IExposureDetectionService
    {
        private readonly ILoggerService _loggerService;
        private readonly IUserDataRepository _userDataRepository;
        private readonly ISendEventLogStateRepository _sendEventLogStateRepository;

        private readonly IExposureDataRepository _exposureDataRepository;

        private readonly ILocalNotificationService _localNotificationService;
        private readonly IExposureRiskCalculationService _exposureRiskCalculationService;
        private readonly IExposureRiskCalculationConfigurationRepository _exposureRiskCalculationConfigurationRepository;

        private readonly IExposureConfigurationRepository _exposureConfigurationRepository;

        private readonly IEventLogRepository _eventLogRepository;

        private readonly IDebugExposureDataCollectServer _exposureDataCollectServer;
        private readonly IDateTimeUtility _dateTimeUtility;
        private readonly IDeviceInfoUtility _deviceInfoUtility;

        public ExposureDetectionService
        (
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            ISendEventLogStateRepository sendEventLogStateRepository,
            IExposureDataRepository exposureDataRepository,
            ILocalNotificationService localNotificationService,
            IExposureRiskCalculationConfigurationRepository exposureRiskCalculationConfigurationRepository,
            IExposureRiskCalculationService exposureRiskCalculationService,
            IExposureConfigurationRepository exposureConfigurationRepository,
            IEventLogRepository eventLogRepository,
            IDebugExposureDataCollectServer exposureDataCollectServer,
            IDateTimeUtility dateTimeUtility,
            IDeviceInfoUtility deviceInfoUtility
            )
        {
            _loggerService = loggerService;
            _userDataRepository = userDataRepository;
            _sendEventLogStateRepository = sendEventLogStateRepository;
            _exposureDataRepository = exposureDataRepository;
            _localNotificationService = localNotificationService;
            _exposureRiskCalculationConfigurationRepository = exposureRiskCalculationConfigurationRepository;
            _exposureRiskCalculationService = exposureRiskCalculationService;
            _exposureConfigurationRepository = exposureConfigurationRepository;
            _eventLogRepository = eventLogRepository;
            _exposureDataCollectServer = exposureDataCollectServer;
            _dateTimeUtility = dateTimeUtility;
            _deviceInfoUtility = deviceInfoUtility;
        }

        public void DiagnosisKeysDataMappingApplied()
        {
            _loggerService.StartMethod();

            if (_exposureConfigurationRepository.IsDiagnosisKeysDataMappingConfigurationUpdated())
            {
                _exposureConfigurationRepository.SetDiagnosisKeysDataMappingAppliedDateTime(_dateTimeUtility.UtcNow);
                _exposureConfigurationRepository.SetIsDiagnosisKeysDataMappingConfigurationUpdated(false);
            }

            _loggerService.EndMethod();
        }

        public void PreExposureDetected(ExposureConfiguration exposureConfiguration, long enVersion)
        {
            _loggerService.Debug("PreExposureDetected");

            if (exposureConfiguration == null)
            {
                _loggerService.Error("PreExposureDetected is called but exposureConfiguration is null.");
            }
        }

        public async Task ExposureDetectedAsync(ExposureConfiguration exposureConfiguration, long enVersion, IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows)
        {
            _loggerService.Debug("ExposureDetected: ExposureWindows");

            var enVersionStr = enVersion.ToString();

            var (newDailySummaries, newExposureWindows) = await _exposureDataRepository.SetExposureDataAsync(
                dailySummaries.ToList(),
                exposureWindows.ToList()
                );

            var exposureRiskCalculationConfiguration = await _exposureRiskCalculationConfigurationRepository
                .GetExposureRiskCalculationConfigurationAsync(preferCache: false);
            _loggerService.Info(exposureRiskCalculationConfiguration.ToString());

            long expectOldestDateMillisSinceEpoch
                = _dateTimeUtility.UtcNow
                .AddDays(AppConstants.TermOfExposureRecordValidityInDays)
                .ToUnixEpochMillis();

            bool isHighRiskExposureDetected = newDailySummaries
                .Where(ds => ds.DateMillisSinceEpoch >= expectOldestDateMillisSinceEpoch)
                .Select(ds => _exposureRiskCalculationService.CalcRiskLevel(
                        ds,
                        newExposureWindows.Where(ew => ew.DateMillisSinceEpoch == ds.DateMillisSinceEpoch).ToList(),
                        exposureRiskCalculationConfiguration
                    )
                )
                .Any(riskLevel => riskLevel >= RiskLevel.High);

            if (isHighRiskExposureDetected)
            {
                _ = _localNotificationService.ShowExposureNotificationAsync();

                bool enableSendEventExposureNotificationNotified = _sendEventLogStateRepository
                    .GetSendEventLogState(ISendEventLogStateRepository.EVENT_TYPE_EXPOSURE_NOTIFICATION_NOTIFIED) == SendEventLogState.Enable;

                if (enableSendEventExposureNotificationNotified)
                {
                    // TODO: send event log.
                }
            }
            else
            {
                _loggerService.Info($"DailySummary: {dailySummaries.Count}, but no high-risk exposure detected");
            }

            try
            {
                await _exposureDataCollectServer.UploadExposureDataAsync(
                    exposureConfiguration,
                    _deviceInfoUtility.Model,
                    enVersionStr,
                    newDailySummaries, newExposureWindows
                    );
            }
            catch (Exception e)
            {
                _loggerService.Exception("UploadExposureDataAsync", e);
            }

            string idempotencyKey = Guid.NewGuid().ToString();
            try
            {
                await _eventLogRepository.AddAsync(
                    exposureConfiguration,
                    _deviceInfoUtility.Model,
                    enVersionStr,
                    newDailySummaries, newExposureWindows,
                    AppConstants.MAX_LOG_REQUEST_SIZE_IN_BYTES
                    );
            }
            catch (Exception e)
            {
                _loggerService.Exception("SendExposureDataAsync", e);
            }
        }

        public async Task ExposureDetectedAsync(ExposureConfiguration exposureConfiguration, long enVersion, ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations)
        {
            _loggerService.Info("ExposureDetected: Legacy-V1");

            var enVersionStr = enVersion.ToString();

            ExposureConfiguration.GoogleExposureConfiguration configurationV1 = exposureConfiguration.GoogleExposureConfig;

            bool isNewExposureDetected = _exposureDataRepository.AppendExposureData(
                exposureSummary,
                exposureInformations.ToList(),
                configurationV1.MinimumRiskScore
                );

            if (isNewExposureDetected)
            {
                _ = _localNotificationService.ShowExposureNotificationAsync();
            }
            else
            {
                _loggerService.Info($"MatchedKeyCount: {exposureSummary.MatchedKeyCount}, but no new exposure detected");
            }

            try
            {
                await _exposureDataCollectServer.UploadExposureDataAsync(
                    exposureConfiguration,
                    _deviceInfoUtility.Model,
                    enVersionStr,
                    exposureSummary, exposureInformations
                    );
            }
            catch (Exception e)
            {
                _loggerService.Exception("UploadExposureDataAsync", e);
            }

            string idempotencyKey = Guid.NewGuid().ToString();
            try
            {
                await _eventLogRepository.AddAsync(
                    exposureConfiguration,
                    _deviceInfoUtility.Model,
                    enVersionStr,
                    exposureSummary, exposureInformations,
                    AppConstants.MAX_LOG_REQUEST_SIZE_IN_BYTES
                    );
            }
            catch (Exception e)
            {
                _loggerService.Exception("SendExposureDataAsync", e);
            }
        }

        public async Task ExposureNotDetectedAsync(ExposureConfiguration exposureConfiguration, long enVersion)
        {
            _loggerService.Info("ExposureNotDetected");
        }
    }
}
