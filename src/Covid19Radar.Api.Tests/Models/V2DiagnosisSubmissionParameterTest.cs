﻿/* This Source Code Form is subject to the terms of the Mozilla Public
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
    public class V2DiagnosisSubmissionParameterTest
    {
        private const string EXPECTED_CLEAR_TEXT_V2 = "jp.go.mhlw.cocoa.unit_test|S2V5RGF0YTE=.10000.140,S2V5RGF0YTI=.20000.141,S2V5RGF0YTM=.30000.142,S2V5RGF0YTQ=.40000.143,S2V5RGF0YTU=.50000.70|440,441|VerificationPayload THIS STRING IS MEANINGLESS";
        private const string EXPECTED_TRANSACTION_ID_SEED_V2 = "jp.go.mhlw.cocoa.unit_testS2V5RGF0YTE=.10000.140,S2V5RGF0YTI=.20000.141,S2V5RGF0YTM=.30000.142,S2V5RGF0YTQ=.40000.143,S2V5RGF0YTU=.50000.70440,441";

        private const string EXPECTED_CLEAR_TEXT_V2_NO_KEY = "jp.go.mhlw.cocoa.unit_test||440,441|VerificationPayload THIS STRING IS MEANINGLESS";
        private const string EXPECTED_TRANSACTION_ID_SEED_V2_NO_KEY = "jp.go.mhlw.cocoa.unit_test440,441";

        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new V2DiagnosisSubmissionParameter();
            // assert
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new V2DiagnosisSubmissionParameter();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

        [DataTestMethod]
        [DataRow("KEYDATA", 0, 0, true)]
        [DataRow("KEYDATA", 145, 0, false)]
        [DataRow("KEYDATA", 144, -15, false)]
        [DataRow("KEYDATA", 144, -14, true)]
        [DataRow("KEYDATA", 144, -6, true)]
        [DataRow("KEYDATA", 144, -5, true)]
        [DataRow("KEYDATA", 144, -4, true)]
        [DataRow("KEYDATA", 144, -3, true)]
        [DataRow("KEYDATA", 144, -2, true)]
        [DataRow("KEYDATA", 144, -1, true)]
        [DataRow("KEYDATA", 144, 0, true)]
        [DataRow("KEYDATA", 144, 1, false)]
        public void KeyValidationTest(string keyData, int rollingPeriod, int rollingStartNummberDayOffset, bool isValid)
        {
            var dateTime = DateTime.UtcNow.Date;
            var key = new V2DiagnosisSubmissionParameter.Key() { KeyData = keyData, RollingPeriod = (uint)rollingPeriod, RollingStartNumber = dateTime.AddDays(rollingStartNummberDayOffset).ToRollingStartNumber() };
            Assert.AreEqual(isValid, key.IsValid());
        }

        private static V2DiagnosisSubmissionParameter.Key CreateDiagnosisKey (string keyData, int rollingStartNumber, int rollingPeriod)
        {
            var keyDataBytes = Encoding.UTF8.GetBytes(keyData);
            var keyDataBase64 = Convert.ToBase64String(keyDataBytes);

            return new V2DiagnosisSubmissionParameter.Key() {
                KeyData = keyDataBase64,
                RollingStartNumber = (uint)rollingStartNumber,
                RollingPeriod = (uint)rollingPeriod,
            };
        }

        [TestMethod]
        public void DeviceVerificationTest()
        {
            var platform = "Android";
            var dummyDiagnosisKeyDataList = new [] {
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
            var model = new V2DiagnosisSubmissionParameter()
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
                EXPECTED_CLEAR_TEXT_V2,
                model.ClearText);

            Assert.AreEqual(dummyDeviceVerificationPayload, model.DeviceToken);
            Assert.AreEqual(
                EXPECTED_TRANSACTION_ID_SEED_V2,
                model.TransactionIdSeed);
        }

        [TestMethod]
        public void DeviceVerificationTest_NoKey()
        {
            var platform = "Android";
            V2DiagnosisSubmissionParameter.Key[] dummyDiagnosisKeyDataList = new V2DiagnosisSubmissionParameter.Key[] {
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
            var model = new V2DiagnosisSubmissionParameter()
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
                EXPECTED_CLEAR_TEXT_V2_NO_KEY,
                model.ClearText);

            Assert.AreEqual(dummyDeviceVerificationPayload, model.DeviceToken);
            Assert.AreEqual(
                EXPECTED_TRANSACTION_ID_SEED_V2_NO_KEY,
                model.TransactionIdSeed);
        }

        [TestMethod]
        public void DeviceVerificationTest_NullKey()
        {
            var platform = "Android";
            V2DiagnosisSubmissionParameter.Key[] dummyDiagnosisKeyDataList = null;
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
            var model = new V2DiagnosisSubmissionParameter()
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
                EXPECTED_CLEAR_TEXT_V2_NO_KEY,
                model.ClearText);

            Assert.AreEqual(dummyDeviceVerificationPayload, model.DeviceToken);
            Assert.AreEqual(
                EXPECTED_TRANSACTION_ID_SEED_V2_NO_KEY,
                model.TransactionIdSeed);
        }
    }
}
