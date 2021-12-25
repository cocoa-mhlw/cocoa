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

        public void PreExposureDetected(ExposureConfiguration exposureConfiguration, string enVersion);

        public Task ExposureDetectedAsync(ExposureConfiguration exposureConfiguration, string enVersion, IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows);

        public Task ExposureDetectedAsync(ExposureConfiguration exposureConfiguration, string enVersion, ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations);

        public void ExposureNotDetected(ExposureConfiguration exposureConfiguration, string enVersion);
    }

    public class ExposureDetectionService : IExposureDetectionService
    {
        private readonly ILoggerService _loggerService;
        private readonly IUserDataRepository _userDataRepository;
        private readonly ILocalNotificationService _localNotificationService;
        private readonly IExposureRiskCalculationService _exposureRiskCalculationService;

        private readonly IExposureConfigurationRepository _exposureConfigurationRepository;

        private readonly IEventLogService _eventLogService;

        private readonly IExposureDataCollectServer _exposureDataCollectServer;
        private readonly IDateTimeUtility _dateTimeUtility;
        private readonly IDeviceInfoUtility _deviceInfoUtility;

        public ExposureDetectionService
        (
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            ILocalNotificationService localNotificationService,
            IExposureRiskCalculationService exposureRiskCalculationService,
            IExposureConfigurationRepository exposureConfigurationRepository,
            IEventLogService eventLogService,
            IExposureDataCollectServer exposureDataCollectServer,
            IDateTimeUtility dateTimeUtility,
            IDeviceInfoUtility deviceInfoUtility
            )
        {
            _loggerService = loggerService;
            _userDataRepository = userDataRepository;
            _localNotificationService = localNotificationService;
            _exposureRiskCalculationService = exposureRiskCalculationService;
            _exposureConfigurationRepository = exposureConfigurationRepository;
            _eventLogService = eventLogService;
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

        public void PreExposureDetected(ExposureConfiguration exposureConfiguration, string enVersion)
        {
            _loggerService.Debug("PreExposureDetected");
        }

        public async Task ExposureDetectedAsync(ExposureConfiguration exposureConfiguration, string enVersion, IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows)
        {
            _loggerService.Debug("ExposureDetected: ExposureWindows");

            await _userDataRepository.SetExposureDataAsync(
                dailySummaries.ToList(),
                exposureWindows.ToList()
                );

            bool isHighRiskExposureDetected = dailySummaries
                .Select(dailySummary => _exposureRiskCalculationService.CalcRiskLevel(dailySummary))
                .Any(riskLevel => riskLevel >= RiskLevel.High);

            if (isHighRiskExposureDetected)
            {
                _ = _localNotificationService.ShowExposureNotificationAsync();
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
                    enVersion,
                    dailySummaries, exposureWindows
                    );
            }
            catch (Exception e)
            {
                _loggerService.Exception("UploadExposureDataAsync", e);
            }

            string idempotencyKey = Guid.NewGuid().ToString();
            try
            {
                await _eventLogService.SendExposureDataAsync(
                    idempotencyKey,
                    exposureConfiguration,
                    _deviceInfoUtility.Model,
                    enVersion,
                    dailySummaries, exposureWindows
                    );
            }
            catch (Exception e)
            {
                _loggerService.Exception("SendExposureDataAsync", e);
            }
        }

        public async Task ExposureDetectedAsync(ExposureConfiguration exposureConfiguration, string enVersion, ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations)
        {
            _loggerService.Info("ExposureDetected: Legacy-V1");

            ExposureConfiguration.GoogleExposureConfiguration configurationV1 = exposureConfiguration.GoogleExposureConfig;

            bool isNewExposureDetected = _userDataRepository.AppendExposureData(
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
                    enVersion,
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
                await _eventLogService.SendExposureDataAsync(
                    idempotencyKey,
                    exposureConfiguration,
                    _deviceInfoUtility.Model,
                    enVersion,
                    exposureSummary, exposureInformations
                    );
            }
            catch (Exception e)
            {
                _loggerService.Exception("SendExposureDataAsync", e);
            }
        }

        public void ExposureNotDetected(ExposureConfiguration exposureConfiguration, string enVersion)
        {
            _loggerService.Info("ExposureNotDetected");

            _ = Task.Run(async () =>
            {
                try
                {
                    await _exposureDataCollectServer.UploadExposureDataAsync(
                        exposureConfiguration,
                        _deviceInfoUtility.Model,
                        enVersion
                        );
                }
                catch (Exception e)
                {
                    _loggerService.Exception("UploadExposureDataAsync", e);
                }

                string idempotencyKey = Guid.NewGuid().ToString();
                try
                {
                    await _eventLogService.SendExposureDataAsync(
                        idempotencyKey,
                        exposureConfiguration,
                        _deviceInfoUtility.Model,
                        enVersion
                        );
                }
                catch (Exception e)
                {
                    _loggerService.Exception("SendExposureDataAsync", e);
                }
            });
        }
    }
}
