/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Chino;

namespace Covid19Radar.Services
{
    public interface IExposureRiskCalculationService
    {
        RiskLevel CalcRiskLevel(DailySummary dailySummary, List<ExposureWindow> exposureWindowList);
    }

    public class ExposureRiskCalculationService : IExposureRiskCalculationService
    {
        // TODO: refine
        private const double THRESHOLD_SCORE_SUM = 2000.0;

        public RiskLevel CalcRiskLevel(DailySummary dailySummary, List<ExposureWindow> exposureWindowList)
        {
            if (dailySummary.DaySummary.ScoreSum >= THRESHOLD_SCORE_SUM)
            {
                return RiskLevel.High;
            }
            return RiskLevel.Low;
        }

    }
}
