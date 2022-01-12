// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using Chino;
using Covid19Radar.Services;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class ExposureRiskCalculationServiceTests
    {

        public ExposureRiskCalculationServiceTests()
        {
        }

        private IExposureRiskCalculationService CreateService()
        {
            return new ExposureRiskCalculationService();
        }

        [Theory]
        [InlineData(1999, RiskLevel.Low)]
        [InlineData(2000, RiskLevel.High)]
        public void RiskExposureTest(double scoreSum, RiskLevel expected)
        {

            var dailySummary = new DailySummary()
                {
                    DateMillisSinceEpoch = 0,
                    DaySummary = new ExposureSummaryData()
                    {
                        ScoreSum = scoreSum
                    },
                    ConfirmedClinicalDiagnosisSummary = new ExposureSummaryData(),
                    ConfirmedTestSummary = new ExposureSummaryData(),
                    RecursiveSummary = new ExposureSummaryData(),
                    SelfReportedSummary = new ExposureSummaryData()
            };

            var exposureWindows = new List<ExposureWindow>()
            {
                new ExposureWindow()
                {
                    CalibrationConfidence = CalibrationConfidence.High,
                    DateMillisSinceEpoch = 0,
                    Infectiousness = Infectiousness.High,
                    ReportType = ReportType.Unknown,
                    ScanInstances = new List<ScanInstance>()
                }
            };

            IExposureRiskCalculationService service = CreateService();

            RiskLevel result = service.CalcRiskLevel(dailySummary, exposureWindows);

            Assert.Equal(expected, result);
        }
    }
}
