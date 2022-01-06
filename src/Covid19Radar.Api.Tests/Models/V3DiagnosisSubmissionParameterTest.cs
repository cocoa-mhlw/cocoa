/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Api.Extensions;
using System.Text;
using Covid19Radar.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Api.Tests.Models
{
    [TestClass]
    [TestCategory("Models")]
    public class V3DiagnosisSubmissionParameterTest
    {
        private const string EXPECTED_CLEAR_TEXT_V3 = "2021-12-19T19:02:00.000+09:00|jp.go.mhlw.cocoa.unit_test|S2V5RGF0YTE=.10000.140.1,S2V5RGF0YTI=.20000.141.1,S2V5RGF0YTM=.30000.142.1,S2V5RGF0YTQ=.40000.143.1,S2V5RGF0YTU=.50000.70.1|440,441|VerificationPayload THIS STRING IS MEANINGLESS";
        private const string EXPECTED_TRANSACTION_ID_SEED_V3 = "2021-12-19T19:02:00.000+09:00jp.go.mhlw.cocoa.unit_testS2V5RGF0YTE=.10000.140.1,S2V5RGF0YTI=.20000.141.1,S2V5RGF0YTM=.30000.142.1,S2V5RGF0YTQ=.40000.143.1,S2V5RGF0YTU=.50000.70.1440,441";

        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new V3DiagnosisSubmissionParameter();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new V3DiagnosisSubmissionParameter();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

        [DataTestMethod]
        [DataRow("KEYDATA", 0, 0, 0, true)]
        [DataRow("KEYDATA", 145, 0, 0, false)]
        [DataRow("KEYDATA", 144, -15, 0, false)]
        [DataRow("KEYDATA", 144, -14, 0, true)]
        [DataRow("KEYDATA", 144, -6, 0, true)]
        [DataRow("KEYDATA", 144, -5, 0, true)]
        [DataRow("KEYDATA", 144, -4, 0, true)]
        [DataRow("KEYDATA", 144, -3, 0, true)]
        [DataRow("KEYDATA", 144, -2, 0, true)]
        [DataRow("KEYDATA", 144, -1, 0, true)]
        [DataRow("KEYDATA", 144, 0, 0, true)]
        [DataRow("KEYDATA", 144, 0, -14, true)]
        [DataRow("KEYDATA", 144, 0, 14, true)]
        [DataRow("KEYDATA", 144, 0, -15, false)]
        [DataRow("KEYDATA", 144, 0, 15, false)]
        [DataRow("KEYDATA", 144, 1, 0, false)]
        public void KeyValidationTest(string keyData, int rollingPeriod, int rollingStartNummberDayOffset, int daysSinceOnsetOfSymptoms, bool isValid)
        {
            var dateTime = DateTime.UtcNow.Date;

            var key = CreateDiagnosisKey(
                keyData,
                (int)dateTime.AddDays(rollingStartNummberDayOffset).ToRollingStartNumber(),
                rollingPeriod,
                1,
                daysSinceOnsetOfSymptoms
            );
            Assert.AreEqual(isValid, key.IsValid());
        }

        private static V3DiagnosisSubmissionParameter.Key CreateDiagnosisKey(
            string keyData,
            int rollingStartNumber,
            int rollingPeriod,
            int reportType,
            int daysSinceOnsetOfSymptoms
            )
        {
            var keyDataBytes = Encoding.UTF8.GetBytes(keyData);
            var keyDataBase64 = Convert.ToBase64String(keyDataBytes);

            return new V3DiagnosisSubmissionParameter.Key()
            {
                KeyData = keyDataBase64,
                RollingStartNumber = (uint)rollingStartNumber,
                RollingPeriod = (uint)rollingPeriod,
                ReportType = (uint)reportType,
                DaysSinceOnsetOfSymptoms = daysSinceOnsetOfSymptoms,
            };
        }

        [TestMethod]
        public void DeviceVerificationTest()
        {
            var platform = "Android";
            var dummyDiagnosisKeyDataList = new[] {
                CreateDiagnosisKey("KeyData1", 10000, 140, 1, -1),
                CreateDiagnosisKey("KeyData2", 20000, 141, 1, 0),
                CreateDiagnosisKey("KeyData3", 30000, 142, 1, 1),
                CreateDiagnosisKey("KeyData4", 40000, 143, 1, 2),
                CreateDiagnosisKey("KeyData5", 50000, 70, 1, 3),
            };

            var dummyRegions = new string[]
            {
                "440",
                "441",
            };
            var dummySymptomOnsetDate = "2021-12-19T19:02:00.000+09:00";

            var dummyDeviceVerificationPayload = "DeviceVerificationPayload THIS STRING IS MEANINGLESS";
            var dummyAppPackageName = "jp.go.mhlw.cocoa.unit_test";
            var dummyVerificationPayload = "VerificationPayload THIS STRING IS MEANINGLESS";

            // This value will not affect any result.
            var dummyPadding = new Random().Next().ToString();

            // preparation
            var model = new V3DiagnosisSubmissionParameter()
            {
                Platform = platform,
                Regions = dummyRegions,
                SymptomOnsetDate = dummySymptomOnsetDate,
                Keys = dummyDiagnosisKeyDataList,
                DeviceVerificationPayload = dummyDeviceVerificationPayload,
                AppPackageName = dummyAppPackageName,
                VerificationPayload = dummyVerificationPayload,
                Padding = dummyPadding,
            };

            Assert.AreEqual(dummyDeviceVerificationPayload, model.JwsPayload);
            Assert.AreEqual(
                EXPECTED_CLEAR_TEXT_V3,
                model.ClearText
                );

            Assert.AreEqual(dummyDeviceVerificationPayload, model.DeviceToken);
            Assert.AreEqual(
                EXPECTED_TRANSACTION_ID_SEED_V3,
                model.TransactionIdSeed
                );
        }
    }
}
