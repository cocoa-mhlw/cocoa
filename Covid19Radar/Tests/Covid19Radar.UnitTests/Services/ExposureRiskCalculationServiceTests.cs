// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using Chino;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class ExposureRiskCalculationServiceTests
    {

        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;

        public ExposureRiskCalculationServiceTests(
            )
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();

        }

        private IExposureRiskCalculationService CreateService()
        {
            return new ExposureRiskCalculationService(
                mockLoggerService.Object
                );
        }

        [Theory]
        [InlineData(V1ExposureRiskCalculationConfiguration.Threshold.OPERATION_GREATER_EQUAL, 2000, 1999, RiskLevel.Low)]
        [InlineData(V1ExposureRiskCalculationConfiguration.Threshold.OPERATION_GREATER_EQUAL, 2000, 2000, RiskLevel.High)]
        public void RiskExposureTest(string scoreSumOp, double scoreSumValue, double scoreSum, RiskLevel expected)
        {

            var configuration = new V1ExposureRiskCalculationConfiguration() {
                DailySummary_DaySummary_ScoreSum = new V1ExposureRiskCalculationConfiguration.Threshold()
                {
                    Op = scoreSumOp,
                    Value = scoreSumValue
                }
            };

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

            RiskLevel result = service.CalcRiskLevel(dailySummary, exposureWindows, configuration);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void HighRiskExposureTest1()
        {
            var configuration = new V1ExposureRiskCalculationConfiguration()
            {
                DailySummary_DaySummary_ScoreSum = new V1ExposureRiskCalculationConfiguration.Threshold()
                {
                    Op = V1ExposureRiskCalculationConfiguration.Threshold.OPERATION_GREATER_EQUAL,
                    Value = 2000.0
                },
                ExposureWindow_ScanInstance_TypicalAttenuationDb_Min = new V1ExposureRiskCalculationConfiguration.Threshold()
                {
                    Op = V1ExposureRiskCalculationConfiguration.Threshold.OPERATION_LESS_EQUAL,
                    Value = 0.0
                },
            };

            var dailySummary = new DailySummary()
            {
                DateMillisSinceEpoch = 0,
                DaySummary = new ExposureSummaryData()
                {
                    ScoreSum = 2000
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
                    {
                        new ScanInstance()
                        {
                            TypicalAttenuationDb = -1
                        }
                    }
                }
            };

            IExposureRiskCalculationService service = CreateService();

            RiskLevel result = service.CalcRiskLevel(dailySummary, exposureWindows, configuration);

            Assert.Equal(RiskLevel.High, result);
        }

        [Fact]
        public void RiskExposureTest_AllNOP()
        {
            var configuration = new V1ExposureRiskCalculationConfiguration()
            {
                // All conditions are NOP
            };

            var dailySummary = new DailySummary()
            {
                DateMillisSinceEpoch = 0,
                DaySummary = new ExposureSummaryData()
                {
                    ScoreSum = 2000.0
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
                    {
                        new ScanInstance()
                        {
                            SecondsSinceLastScan = 600,
                            TypicalAttenuationDb = 1
                        },
                        new ScanInstance()
                        {
                            SecondsSinceLastScan = 600,
                            TypicalAttenuationDb = 10
                        }
                    }
                }
            };

            IExposureRiskCalculationService service = CreateService();

            RiskLevel result = service.CalcRiskLevel(dailySummary, exposureWindows, configuration);

            Assert.Equal(RiskLevel.Low, result);
        }

        [Fact]
        public void RiskExposureTest_AND1()
        {
            var configuration = new V1ExposureRiskCalculationConfiguration()
            {
                DailySummary_DaySummary_ScoreSum = new V1ExposureRiskCalculationConfiguration.Threshold()
                {
                    Op = V1ExposureRiskCalculationConfiguration.Threshold.OPERATION_GREATER_EQUAL,
                    Value = 2000.0
                },
                ExposureWindow_ScanInstance_TypicalAttenuationDb_Min = new V1ExposureRiskCalculationConfiguration.Threshold()
                {
                    Op = V1ExposureRiskCalculationConfiguration.Threshold.OPERATION_LESS,
                    Value = 0.0
                },
                ExposureWindow_ScanInstance_TypicalAttenuationDb_Max = new V1ExposureRiskCalculationConfiguration.Threshold()
                {
                    Op = V1ExposureRiskCalculationConfiguration.Threshold.OPERATION_GREATER,
                    Value = 9.0
                },
            };

            var dailySummary = new DailySummary()
            {
                DateMillisSinceEpoch = 0,
                DaySummary = new ExposureSummaryData()
                {
                    ScoreSum = 2000
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
                    {
                        new ScanInstance()
                        {
                            TypicalAttenuationDb = 1
                        },
                        new ScanInstance()
                        {
                            TypicalAttenuationDb = 10
                        }
                    }
                }
            };

            IExposureRiskCalculationService service = CreateService();

            RiskLevel result = service.CalcRiskLevel(dailySummary, exposureWindows, configuration);

            Assert.Equal(RiskLevel.Low, result);
        }

        [Fact]
        public void RiskExposureTest_AND2()
        {
            var configuration = new V1ExposureRiskCalculationConfiguration()
            {
                DailySummary_DaySummary_ScoreSum = new V1ExposureRiskCalculationConfiguration.Threshold()
                {
                    Op = V1ExposureRiskCalculationConfiguration.Threshold.OPERATION_GREATER_EQUAL,
                    Value = 1170.0
                },
                ExposureWindow_ScanInstance_SecondsSinceLastScanSum = new V1ExposureRiskCalculationConfiguration.Threshold()
                {
                    Op = V1ExposureRiskCalculationConfiguration.Threshold.OPERATION_GREATER_EQUAL,
                    Value = 900.0
                },
            };

            var dailySummary = new DailySummary()
            {
                DateMillisSinceEpoch = 0,
                DaySummary = new ExposureSummaryData()
                {
                    ScoreSum = 1170.0
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
                    {
                        new ScanInstance()
                        {
                            SecondsSinceLastScan = 600,
                            TypicalAttenuationDb = 1
                        },
                        new ScanInstance()
                        {
                            SecondsSinceLastScan = 240,
                            TypicalAttenuationDb = 10
                        }
                    }
                }
            };

            IExposureRiskCalculationService service = CreateService();

            RiskLevel result = service.CalcRiskLevel(dailySummary, exposureWindows, configuration);

            Assert.Equal(RiskLevel.Low, result);
        }
    }
}
