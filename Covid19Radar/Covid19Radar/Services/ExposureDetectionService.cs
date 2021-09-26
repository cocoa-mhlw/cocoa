/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Xamarin.Essentials;

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

        private readonly AbsExposureNotificationClient _exposureNotificationClient;
        private readonly IExposureDataCollectServer _exposureDataCollectServer;

        public ExposureDetectionService
        (
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            ILocalNotificationService localNotificationService,
            IExposureRiskCalculationService exposureRiskCalculationService,
            IExposureConfigurationRepository exposureConfigurationRepository,
            AbsExposureNotificationClient exposureNotificationClient,
            IExposureDataCollectServer exposureDataCollectServer
            )
        {
            _loggerService = loggerService;
            _userDataRepository = userDataRepository;
            _localNotificationService = localNotificationService;
            _exposureRiskCalculationService = exposureRiskCalculationService;
            _exposureConfigurationRepository = exposureConfigurationRepository;
            _exposureNotificationClient = exposureNotificationClient;
            _exposureDataCollectServer = exposureDataCollectServer;
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

            _ = Task.Run(async () =>
            {
                var enVersion = (await _exposureNotificationClient.GetVersionAsync()).ToString();
                ExposureConfiguration exposureConfiguration = await _exposureConfigurationRepository.GetExposureConfigurationAsync();

                _ = await _exposureDataCollectServer.UploadExposureDataAsync(
                    exposureConfiguration,
                    DeviceInfo.Model,
                    enVersion,
                    dailySummaries, exposureWindows
                    );
            });
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

            _ = Task.Run(async () =>
            {
                var enVersion = (await _exposureNotificationClient.GetVersionAsync()).ToString();
                ExposureConfiguration exposureConfiguration = await _exposureConfigurationRepository.GetExposureConfigurationAsync();

                _ = await _exposureDataCollectServer.UploadExposureDataAsync(
                    exposureConfiguration,
                    DeviceInfo.Model,
                    enVersion,
                    exposureSummary, exposureInformations
                    );
            });

            _loggerService.EndMethod();
        }

        public void ExposureNotDetected()
        {
            _loggerService.Info("ExposureNotDetected");

            _ = Task.Run(async () =>
            {
                var enVersion = (await _exposureNotificationClient.GetVersionAsync()).ToString();
                ExposureConfiguration exposureConfiguration = await _exposureConfigurationRepository.GetExposureConfigurationAsync();

                _ = await _exposureDataCollectServer.UploadExposureDataAsync(
                    exposureConfiguration,
                    DeviceInfo.Model,
                    enVersion
                    );
            });
        }
    }
}
