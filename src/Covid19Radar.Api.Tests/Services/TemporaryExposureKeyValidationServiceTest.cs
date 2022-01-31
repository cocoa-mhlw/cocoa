/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Api.Extensions;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Covid19Radar.Api.Tests.Services
{
    [TestClass]
    [TestCategory("Services")]
    public class TemporaryExposureKeyValidationServiceTest
    {
        [DataTestMethod]

        // KeyData
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 0, 1, 0, -3, +10, -3, +7, true)]
        [DataRow(false, "                        ", 0, 144, 0, 1, 0, -3, +10, -3, +7, false)]
        [DataRow(false, null, 0, 144, 0, 1, 0, -3, +10, -3, +7, false)]
        [DataRow(false, "", 0, 144, 0, 1, 0, -3, +10, -3, +7, false)]

        // rollingStartNumber
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ=", 0, 144, 0, 1, 0, -20, +20, -20, +20, false)]
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 1, 144, 0, 1, 0, -20, +20, -20, +20, false)]
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", -1, 144, 0, 1, 0, -20, +20, -20, +20, true)]
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", -15, 144, 0, 1, 0, -20, +20, -20, +20, false)]

        // rollingPeriod
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 0, 1, 0, -20, +20, -20, +20, true)]
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 145, 0, 1, 0, -20, +20, -20, +20, false)]

        // TransmissionRisk
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 5, 0, -20, +20, -20, +20, true)]
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 8, 5, 0, -20, +20, -20, +20, false)]

        // ReportType
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 5, 0, -3, -20, +20, -20, +20, true)]
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 6, 0, -3, -20, +20, -20, +20, false)]

        // DaysSinceOnsetOfSymptoms
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 5, +14, -20, +20, -20, +20, true)]
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 5, +15, -20, +20, -20, +20, false)]
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 5, -14, -20, +20, -20, +20, true)]
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 5, -15, -20, +20, -20, +20, false)]

        // DaysSinceOnsetOfSymptom
        [DataRow(true, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 5, -4, -4, +10, -3, +7, true)]
        [DataRow(true, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 5, -5, -4, +10, -6, +7, false)]
        [DataRow(true, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 5, +10, -4, +10, -3, +9, true)]
        [DataRow(true, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 5, +11, -4, +10, -6, +12, false)]

        // DaysSinceOnsetOfDiagnosis
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 5, -3, -2, +10, -3, +7, true)]
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 5, -4, -4, +10, -3, +7, false)]
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 5, +10, -4, +9, -3, +10, true)]
        [DataRow(false, "IVb9YMEnlTzsReDYUmZxQQ==", 0, 144, 7, 5, +11, -4, +12, -6, +10, false)]
        public void Test(
            bool hasSymptom,
            string keyData,
            int rollingStartNumberDayOffset,
            int rollingPeriod,
            int transmissionRisk,
            int reportType,
            int daysSinceOnsetOfSymptoms,
            int minDaysSinceOnsetOfSymptom,
            int maxDaysSinceOnsetOfSymptom,
            int minDaysSinceOnsetOfDiagnosis,
            int maxDaysSinceOnsetOfDiagnosis,
            bool expectedResult
            )
        {
            // arrange
            var config = new Mock<IConfiguration>();
            config.Setup(x => x["MinDaysSinceOnsetOfSymptoms"]).Returns($"{minDaysSinceOnsetOfSymptom}");
            config.Setup(x => x["MaxDaysSinceOnsetOfSymptoms"]).Returns($"{maxDaysSinceOnsetOfSymptom}");
            config.Setup(x => x["MinDaysSinceOnsetOfDiagnosis"]).Returns($"{minDaysSinceOnsetOfDiagnosis}");
            config.Setup(x => x["MaxDaysSinceOnsetOfDiagnosis"]).Returns($"{maxDaysSinceOnsetOfDiagnosis}");

            var logger = new Mock.LoggerMock<TemporaryExposureKeyValidationService>();

            var service = new TemporaryExposureKeyValidationService(
                config.Object,
                logger
                );

            bool result = service.Validate(hasSymptom, new V3DiagnosisSubmissionParameter.Key()
            {
                KeyData = keyData,
                RollingStartNumber = DateTime.UtcNow.Date.AddDays(rollingStartNumberDayOffset).ToRollingStartNumber(),
                RollingPeriod = (uint)rollingPeriod,
                TransmissionRisk = transmissionRisk,
                ReportType = (uint)reportType,
                DaysSinceOnsetOfSymptoms = daysSinceOnsetOfSymptoms,
            });

            Assert.AreEqual(expectedResult, result);
        }
    }
}
