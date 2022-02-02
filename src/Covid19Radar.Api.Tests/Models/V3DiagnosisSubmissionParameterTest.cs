/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Text;
using Covid19Radar.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Api.Tests.Models
{
    [TestClass]
    [TestCategory("Models")]
    public class V3DiagnosisSubmissionParameterTest
    {
        private const string EXPECTED_CLEAR_TEXT_V3_HASSYMPTOM = "2021-12-19T19:02:00.000+09:00|HasSymptom|jp.go.mhlw.cocoa.unit_test|S2V5RGF0YTE=.10000.140.1,S2V5RGF0YTI=.20000.141.1,S2V5RGF0YTM=.30000.142.1,S2V5RGF0YTQ=.40000.143.1,S2V5RGF0YTU=.50000.70.1|440,441|VerificationPayload THIS STRING IS MEANINGLESS";
        private const string EXPECTED_TRANSACTION_ID_SEED_V3_HASSYMPTOM = "2021-12-19T19:02:00.000+09:00HasSymptomjp.go.mhlw.cocoa.unit_testS2V5RGF0YTE=.10000.140.1,S2V5RGF0YTI=.20000.141.1,S2V5RGF0YTM=.30000.142.1,S2V5RGF0YTQ=.40000.143.1,S2V5RGF0YTU=.50000.70.1440,441";
        private const string EXPECTED_CLEAR_TEXT_V3_NOSYMPTOM = "2021-12-19T19:02:00.000+09:00|NoSymptom|jp.go.mhlw.cocoa.unit_test|S2V5RGF0YTE=.10000.140.1,S2V5RGF0YTI=.20000.141.1,S2V5RGF0YTM=.30000.142.1,S2V5RGF0YTQ=.40000.143.1,S2V5RGF0YTU=.50000.70.1|440,441|VerificationPayload THIS STRING IS MEANINGLESS";
        private const string EXPECTED_TRANSACTION_ID_SEED_V3_NOSYMPTOM = "2021-12-19T19:02:00.000+09:00NoSymptomjp.go.mhlw.cocoa.unit_testS2V5RGF0YTE=.10000.140.1,S2V5RGF0YTI=.20000.141.1,S2V5RGF0YTM=.30000.142.1,S2V5RGF0YTQ=.40000.143.1,S2V5RGF0YTU=.50000.70.1440,441";

        private const string EXPECTED_CLEAR_TEXT_V3_NO_KEY = "2021-12-19T19:02:00.000+09:00|HasSymptom|jp.go.mhlw.cocoa.unit_test||440,441|VerificationPayload THIS STRING IS MEANINGLESS";
        private const string EXPECTED_TRANSACTION_ID_SEED_V3_NO_KEY = "2021-12-19T19:02:00.000+09:00HasSymptomjp.go.mhlw.cocoa.unit_test440,441";

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
        public void DeviceVerificationTest_HasSymptom()
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
                HasSymptom = true,
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
                EXPECTED_CLEAR_TEXT_V3_HASSYMPTOM,
                model.ClearText
                );

            Assert.AreEqual(dummyDeviceVerificationPayload, model.DeviceToken);
            Assert.AreEqual(
                EXPECTED_TRANSACTION_ID_SEED_V3_HASSYMPTOM,
                model.TransactionIdSeed
                );
        }

        [TestMethod]
        public void DeviceVerificationTest_NoSymptom()
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
                HasSymptom = false,
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
                EXPECTED_CLEAR_TEXT_V3_NOSYMPTOM,
                model.ClearText
                );

            Assert.AreEqual(dummyDeviceVerificationPayload, model.DeviceToken);
            Assert.AreEqual(
                EXPECTED_TRANSACTION_ID_SEED_V3_NOSYMPTOM,
                model.TransactionIdSeed
                );
        }

        [TestMethod]
        public void DeviceVerificationTest_NoKey()
        {
            var platform = "Android";
            V3DiagnosisSubmissionParameter.Key[] dummyDiagnosisKeyDataList = new V3DiagnosisSubmissionParameter.Key[]{
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
                HasSymptom = true,
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
                EXPECTED_CLEAR_TEXT_V3_NO_KEY,
                model.ClearText
                );

            Assert.AreEqual(dummyDeviceVerificationPayload, model.DeviceToken);
            Assert.AreEqual(
                EXPECTED_TRANSACTION_ID_SEED_V3_NO_KEY,
                model.TransactionIdSeed
                );
        }

        [TestMethod]
        public void DeviceVerificationTestKeysNull()
        {
            var platform = "Android";
            V3DiagnosisSubmissionParameter.Key[] dummyDiagnosisKeyDataList = null;

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
                HasSymptom = true,
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
                EXPECTED_CLEAR_TEXT_V3_NO_KEY,
                model.ClearText
                );

            Assert.AreEqual(dummyDeviceVerificationPayload, model.DeviceToken);
            Assert.AreEqual(
                EXPECTED_TRANSACTION_ID_SEED_V3_NO_KEY,
                model.TransactionIdSeed
                );
        }
    }
}
