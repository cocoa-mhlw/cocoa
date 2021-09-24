/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Chino;

namespace Covid19Radar.Services
{
    public interface IExposureRiskCalculationService
    {
        RiskLevel CalcRiskLevel(DailySummary dailySummary);
    }

    public class ExposureRiskCalculationService : IExposureRiskCalculationService
    {
        // TODO:  We should make consideration later.
        public RiskLevel CalcRiskLevel(DailySummary dailySummary)
            => RiskLevel.High;
    }
}
