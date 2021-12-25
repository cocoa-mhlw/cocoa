// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Text;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Xunit;

namespace Covid19Radar.UnitTests.Common
{
    public class DeviceVerifierUtilsTests
    {
        private const string EXPECTED_CLEAR_TEXT_V2 = "jp.go.mhlw.cocoa.unit_test|S2V5RGF0YTE=.10000.140,S2V5RGF0YTI=.20000.141,S2V5RGF0YTM=.30000.142,S2V5RGF0YTQ=.40000.143,S2V5RGF0YTU=.50000.70|440,441|VerificationPayload THIS STRING IS MEANINGLESS";
        private const string EXPECTED_CLEAR_TEXT_V3 = "2021-12-19T19:02:00.000+09:00|jp.go.mhlw.cocoa.unit_test|S2V5RGF0YTE=.10000.140.1,S2V5RGF0YTI=.20000.141.1,S2V5RGF0YTM=.30000.142.1,S2V5RGF0YTQ=.40000.143.1,S2V5RGF0YTU=.50000.70.1|440,441|VerificationPayload THIS STRING IS MEANINGLESS";

        private static DiagnosisSubmissionParameter.Key CreateDiagnosisKey(
            string keyData,
            int rollingStartNumber,
            int rollingPeriod,
            int reportType
            )
        {
            var keyDataBytes = Encoding.UTF8.GetBytes(keyData);
            var keyDataBase64 = Convert.ToBase64String(keyDataBytes);

            return new DiagnosisSubmissionParameter.Key()
            {
                KeyData = keyDataBase64,
                RollingStartNumber = (uint)rollingStartNumber,
                RollingPeriod = (uint)rollingPeriod,
                ReportType = (uint)reportType
            };
        }

        [Fact]
        public void AndroidClearTextTestV2()
        {
            var platform = "Android";
            var dummyDiagnosisKeyDataList = new[] {
                CreateDiagnosisKey("KeyData1", 10000, 140, 1),
                CreateDiagnosisKey("KeyData2", 20000, 141, 1),
                CreateDiagnosisKey("KeyData3", 30000, 142, 1),
                CreateDiagnosisKey("KeyData4", 40000, 143, 1),
                CreateDiagnosisKey("KeyData5", 50000, 70, 1),
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

            var submissionParameter = new DiagnosisSubmissionParameter()
            {
                Platform = platform,
                Regions = dummyRegions,
                Keys = dummyDiagnosisKeyDataList,
                DeviceVerificationPayload = dummyDeviceVerificationPayload,
                AppPackageName = dummyAppPackageName,
                VerificationPayload = dummyVerificationPayload,
                Padding = dummyPadding,
            };

            string clearText = DeviceVerifierUtils.GetNonceClearTextV2(submissionParameter);
            Assert.Equal(
                EXPECTED_CLEAR_TEXT_V2,
                clearText
                );
        }

        [Fact]
        public void AndroidClearTextTestV3()
        {
            var platform = "Android";
            var dummyDiagnosisKeyDataList = new[] {
                CreateDiagnosisKey("KeyData1", 10000, 140, 1),
                CreateDiagnosisKey("KeyData2", 20000, 141, 1),
                CreateDiagnosisKey("KeyData3", 30000, 142, 1),
                CreateDiagnosisKey("KeyData4", 40000, 143, 1),
                CreateDiagnosisKey("KeyData5", 50000, 70, 1),
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

            var submissionParameter = new DiagnosisSubmissionParameter()
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

            string clearText = DeviceVerifierUtils.GetNonceClearTextV3(submissionParameter);
            Assert.Equal(
                EXPECTED_CLEAR_TEXT_V3,
                clearText
                );
        }
    }
}
