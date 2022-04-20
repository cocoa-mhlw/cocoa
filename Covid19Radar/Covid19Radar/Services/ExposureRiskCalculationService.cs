/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Chino;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;

using Threshold = Covid19Radar.Model.V1ExposureRiskCalculationConfiguration.Threshold;

namespace Covid19Radar.Services
{
    public interface IExposureRiskCalculationService
    {
        RiskLevel CalcRiskLevel(
            DailySummary dailySummary,
            List<ExposureWindow> exposureWindowList,
            V1ExposureRiskCalculationConfiguration configuration
            );
    }

    public class ExposureRiskCalculationService : IExposureRiskCalculationService
    {
        private readonly ILoggerService _loggerService;


        public ExposureRiskCalculationService(
            ILoggerService loggerService
            )
        {
            _loggerService = loggerService;
        }

        public RiskLevel CalcRiskLevel(
            DailySummary dailySummary,
            List<ExposureWindow> exposureWindowList,
            V1ExposureRiskCalculationConfiguration configuration
            )
        {
            if (
                configuration.DailySummary_DaySummary_ScoreSum.Op == Threshold.OPERATION_NOP
                && configuration.DailySummary_WeightedDurationAverage.Op == Threshold.OPERATION_NOP
                && configuration.ExposureWindow_ScanInstance_SecondsSinceLastScanSum.Op == Threshold.OPERATION_NOP
                && configuration.ExposureWindow_ScanInstance_TypicalAttenuationDb_Max.Op == Threshold.OPERATION_NOP
                && configuration.ExposureWindow_ScanInstance_TypicalAttenuationDb_Min.Op == Threshold.OPERATION_NOP
                )
            {
                _loggerService.Info("All conditions are NOP.");
                return RiskLevel.Low;
            }

            var allScanInstances = exposureWindowList
                .SelectMany(ew => ew.ScanInstances);

            double secondsSinceLastScanSum = allScanInstances
                .Sum(si => si.SecondsSinceLastScan);

            double weightedDurationAverage = 0;
            if (secondsSinceLastScanSum > 0)
            {
                weightedDurationAverage = dailySummary.DaySummary.WeightedDurationSum / secondsSinceLastScanSum;
            }

            double typicalAttenuationDbMax = 0;
            if (allScanInstances.Count() > 0)
            {
                typicalAttenuationDbMax = allScanInstances.Max(si => si.TypicalAttenuationDb);
            }

            double typicalAttenuationDbMin = 0;
            if (allScanInstances.Count() > 0)
            {
                typicalAttenuationDbMin = allScanInstances.Min(si => si.TypicalAttenuationDb);
            }

            // AND
            if (configuration.DailySummary_DaySummary_ScoreSum.Cond(dailySummary.DaySummary.ScoreSum)
                && configuration.DailySummary_WeightedDurationAverage.Cond(weightedDurationAverage)
                && configuration.ExposureWindow_ScanInstance_SecondsSinceLastScanSum.Cond(secondsSinceLastScanSum)
                && configuration.ExposureWindow_ScanInstance_TypicalAttenuationDb_Max.Cond(typicalAttenuationDbMax)
                && configuration.ExposureWindow_ScanInstance_TypicalAttenuationDb_Min.Cond(typicalAttenuationDbMin)
                )
            {
                return RiskLevel.High;
            }

            return RiskLevel.Low;
        }
    }
}
