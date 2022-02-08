/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Linq;
using System.Text;
using Covid19Radar.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Api.Tests.Models
{
    [TestClass]
    [TestCategory("Models")]
    public class V1DiagnosisSubmissionParameterTest
    {
        private const string EXPECTED_CLEAR_TEXT_V1 = "jp.go.mhlw.cocoa.unit_test|S2V5RGF0YTE=.10000.140.0,S2V5RGF0YTI=.20000.141.0,S2V5RGF0YTM=.30000.142.0,S2V5RGF0YTQ=.40000.143.0,S2V5RGF0YTU=.50000.70.0|440,441|VerificationPayload THIS STRING IS MEANINGLESS";
        private const string EXPECTED_TRANSACTION_ID_SEED_V1 = "jp.go.mhlw.cocoa.unit_testS2V5RGF0YTE=.10000.140.0,S2V5RGF0YTI=.20000.141.0,S2V5RGF0YTM=.30000.142.0,S2V5RGF0YTQ=.40000.143.0,S2V5RGF0YTU=.50000.70.0440,441";

        private const string EXPECTED_CLEAR_TEXT_V1_NO_KEY = "jp.go.mhlw.cocoa.unit_test||440,441|VerificationPayload THIS STRING IS MEANINGLESS";
        private const string EXPECTED_TRANSACTION_ID_SEED_V1_NO_KEY = "jp.go.mhlw.cocoa.unit_test440,441";

        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new V1DiagnosisSubmissionParameter();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new V1DiagnosisSubmissionParameter();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

        [DataTestMethod]
        [DataRow("KEYDATA", 0, 144, 0)]
        [DataRow("KEYDATA", 144, 144, 144)]
        [DataRow("KEYDATA", 1, 1, 1)]
        public void ToModelTest(string keyData, int period, int expectedPeriod, int start)
        {
            // preparation
            var keyDataBytes = Encoding.UTF8.GetBytes(keyData);
            var keyDataBase64 = Convert.ToBase64String(keyDataBytes);
            var model = new V1DiagnosisSubmissionParameter();
            model.Keys = new V1DiagnosisSubmissionParameter.Key[] {
                new V1DiagnosisSubmissionParameter.Key() {KeyData = keyDataBase64, RollingPeriod = (uint)period, RollingStartNumber = (uint)start, TransmissionRisk = 0  }
            };
            var timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            // action
            var result = model.Keys.Select(_ => _.ToModel(model, timestamp));
            var item = result.First();
            // assert
            CollectionAssert.AreEqual(keyDataBytes, item.KeyData);
            Assert.AreEqual(expectedPeriod, item.RollingPeriod);
            Assert.AreEqual(start, item.RollingStartIntervalNumber);
        }

        private static V1DiagnosisSubmissionParameter.Key CreateDiagnosisKey(string keyData, int rollingStartNumber, int rollingPeriod)
        {
            var keyDataBytes = Encoding.UTF8.GetBytes(keyData);
            var keyDataBase64 = Convert.ToBase64String(keyDataBytes);

            return new V1DiagnosisSubmissionParameter.Key()
            {
                KeyData = keyDataBase64,
                RollingStartNumber = (uint)rollingStartNumber,
                RollingPeriod = (uint)rollingPeriod,
                TransmissionRisk = 0,
            };
        }

        [TestMethod]
        public void DeviceVerificationTest2()
        {
            var platform = "Android";
            V1DiagnosisSubmissionParameter.Key[] dummyDiagnosisKeyDataList = new V1DiagnosisSubmissionParameter.Key[] {
                CreateDiagnosisKey("KeyData1", 10000, 140 ),
                CreateDiagnosisKey("KeyData2", 20000, 141 ),
                CreateDiagnosisKey("KeyData3", 30000, 142 ),
                CreateDiagnosisKey("KeyData4", 40000, 143 ),
                CreateDiagnosisKey("KeyData5", 50000, 70 ),
            };

            var dummyRegions = new string[]
            {
                "440",
                "441",
            };

            var dummyDeviceVerificationPayload = "DeviceVerificationPayload THIS STRING IS MEANINGLESS";
            var dummyAppPackageName = "jp.go.mhlw.cocoa.unit_test";
            var dummyVerificationPayload = "VerificationPayload THIS STRING IS MEANINGLESS";

            // This value will not affect any result.
            var dummyPadding = new Random().Next().ToString();

            // preparation
            var model = new V1DiagnosisSubmissionParameter()
            {
                Platform = platform,
                Regions = dummyRegions,
                Keys = dummyDiagnosisKeyDataList,
                DeviceVerificationPayload = dummyDeviceVerificationPayload,
                AppPackageName = dummyAppPackageName,
                VerificationPayload = dummyVerificationPayload,
                Padding = dummyPadding,
            };

            Assert.AreEqual(dummyDeviceVerificationPayload, model.JwsPayload);
            Assert.AreEqual(
                EXPECTED_CLEAR_TEXT_V1,
                model.ClearText
                );

            Assert.AreEqual(dummyDeviceVerificationPayload, model.DeviceToken);
            Assert.AreEqual(
                EXPECTED_TRANSACTION_ID_SEED_V1,
                model.TransactionIdSeed
                );
        }

        [TestMethod]
        public void DeviceVerificationTest_NoKey()
        {
            var platform = "Android";
            V1DiagnosisSubmissionParameter.Key[] dummyDiagnosisKeyDataList = new V1DiagnosisSubmissionParameter.Key[] {
            };
            var dummyRegions = new string[]
            {
                "440",
                "441",
            };
            var dummyDeviceVerificationPayload = "DeviceVerificationPayload THIS STRING IS MEANINGLESS";
            var dummyAppPackageName = "jp.go.mhlw.cocoa.unit_test";
            var dummyVerificationPayload = "VerificationPayload THIS STRING IS MEANINGLESS";

            // This value will not affect any result.
            var dummyPadding = new Random().Next().ToString();

            // preparation
            var model = new V1DiagnosisSubmissionParameter()
            {
                Platform = platform,
                Regions = dummyRegions,
                Keys = dummyDiagnosisKeyDataList,
                DeviceVerificationPayload = dummyDeviceVerificationPayload,
                AppPackageName = dummyAppPackageName,
                VerificationPayload = dummyVerificationPayload,
                Padding = dummyPadding,
            };

            Assert.AreEqual(dummyDeviceVerificationPayload, model.JwsPayload);
            Assert.AreEqual(
                EXPECTED_CLEAR_TEXT_V1_NO_KEY,
                model.ClearText);

            Assert.AreEqual(dummyDeviceVerificationPayload, model.DeviceToken);
            Assert.AreEqual(
                EXPECTED_TRANSACTION_ID_SEED_V1_NO_KEY,
                model.TransactionIdSeed);
        }

        [TestMethod]
        public void DeviceVerificationTest_NullKey()
        {
            var platform = "Android";
            V1DiagnosisSubmissionParameter.Key[] dummyDiagnosisKeyDataList = null;
            var dummyRegions = new string[]
            {
                "440",
                "441",
            };
            var dummyDeviceVerificationPayload = "DeviceVerificationPayload THIS STRING IS MEANINGLESS";
            var dummyAppPackageName = "jp.go.mhlw.cocoa.unit_test";
            var dummyVerificationPayload = "VerificationPayload THIS STRING IS MEANINGLESS";

            // This value will not affect any result.
            var dummyPadding = new Random().Next().ToString();

            // preparation
            var model = new V1DiagnosisSubmissionParameter()
            {
                Platform = platform,
                Regions = dummyRegions,
                Keys = dummyDiagnosisKeyDataList,
                DeviceVerificationPayload = dummyDeviceVerificationPayload,
                AppPackageName = dummyAppPackageName,
                VerificationPayload = dummyVerificationPayload,
                Padding = dummyPadding,
            };

            Assert.AreEqual(dummyDeviceVerificationPayload, model.JwsPayload);
            Assert.AreEqual(
                EXPECTED_CLEAR_TEXT_V1_NO_KEY,
                model.ClearText);

            Assert.AreEqual(dummyDeviceVerificationPayload, model.DeviceToken);
            Assert.AreEqual(
                EXPECTED_TRANSACTION_ID_SEED_V1_NO_KEY,
                model.TransactionIdSeed);
        }
    }
}
