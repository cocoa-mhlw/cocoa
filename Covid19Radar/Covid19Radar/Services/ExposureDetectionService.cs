/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Chino;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public interface IExposureDetectionService
    {
        public void PreExposureDetected();

        public void ExposureDetected(IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows);

        public void ExposureDetected(ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations);

        public void ExposureNotDetected();
    }

    public class ExposureDetectionService : IExposureDetectionService
    {
        private readonly ILoggerService _loggerService;
        private readonly IUserDataRepository _userDataRepository;
        private readonly ILocalNotificationService _localNotificationService;
        private readonly IExposureRiskCalculationService _exposureRiskCalculationService;
        private readonly IExposureConfigurationRepository _exposureConfigurationRepository;

        public ExposureDetectionService
        (
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            ILocalNotificationService localNotificationService,
            IExposureRiskCalculationService exposureRiskCalculationService,
            IExposureConfigurationRepository exposureConfigurationRepository
            )
        {
            _loggerService = loggerService;
            _userDataRepository = userDataRepository;
            _localNotificationService = localNotificationService;
            _exposureRiskCalculationService = exposureRiskCalculationService;
            _exposureConfigurationRepository = exposureConfigurationRepository;
        }

        public void PreExposureDetected()
        {
            _loggerService.Debug("PreExposureDetected");
        }

        public void ExposureDetected(IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows)
        {
            _loggerService.Debug("ExposureDetected: ExposureWindows");

            _userDataRepository.SetExposureDataAsync(
                dailySummaries.ToList(),
                exposureWindows.ToList()
                );

            bool isHighRiskExposureDetected = dailySummaries
                .Select(dailySummary => _exposureRiskCalculationService.CalcRiskLevel(dailySummary))
                .Where(riskLevel => riskLevel >= RiskLevel.High)
                .Count() > 0;

            if (isHighRiskExposureDetected)
            {
                _localNotificationService.ShowExposureNotificationAsync()
                    .GetAwaiter().GetResult();
            }
            else
            {
                _loggerService.Info($"DailySummary: {dailySummaries.Count}, but no high-risk exposure detected");
            }
        }

        public void ExposureDetected(ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations)
        {
            _loggerService.Info("ExposureDetected: Legacy-V1");

            ExposureConfiguration exposureConfiguration = _exposureConfigurationRepository.GetExposureConfigurationAsync()
                .GetAwaiter().GetResult();
            ExposureConfiguration.GoogleExposureConfiguration configurationV1 = exposureConfiguration.GoogleExposureConfig;

            bool isNewExposureDetected = _userDataRepository.AppendExposureData(
                exposureSummary,
                exposureInformations.ToList(),
                configurationV1.MinimumRiskScore
                );

            if (isNewExposureDetected)
            {
                _localNotificationService.ShowExposureNotificationAsync()
                    .GetAwaiter().GetResult();
            }
            else
            {
                _loggerService.Info($"MatchedKeyCount: {exposureSummary.MatchedKeyCount}, but no new exposure detected");
            }

            _loggerService.EndMethod();
        }

        public void ExposureNotDetected()
        {
            _loggerService.Info("ExposureNotDetected");
        }
    }

}
