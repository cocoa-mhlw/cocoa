// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.Text;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Newtonsoft.Json;
using Xunit;

namespace Covid19Radar.UnitTests.Common
{
    public class DeviceVerifierUtilsDiagnosisSubmissionParametersTests
    {
        private const string EXPECTED_CLEAR_TEXT_V1 = "jp.go.mhlw.cocoa.unit_test|S2V5RGF0YTE=.10000.0,S2V5RGF0YTI=.20000.0,S2V5RGF0YTM=.30000.0,S2V5RGF0YTQ=.40000.0,S2V5RGF0YTU=.50000.0|440,441|VerificationPayload THIS STRING IS MEANINGLESS";
        private const string EXPECTED_CLEAR_TEXT_V2 = "jp.go.mhlw.cocoa.unit_test|S2V5RGF0YTE=.10000.140,S2V5RGF0YTI=.20000.141,S2V5RGF0YTM=.30000.142,S2V5RGF0YTQ=.40000.143,S2V5RGF0YTU=.50000.70|440,441|VerificationPayload THIS STRING IS MEANINGLESS";
        private const string EXPECTED_CLEAR_TEXT_V3_HASSYMPTOM = "jp.go.mhlw.cocoa.unit_test|2021-12-19T19:02:00.000+09:00|HasSymptom|S2V5RGF0YTE=.10000.140.1,S2V5RGF0YTI=.20000.141.1,S2V5RGF0YTM=.30000.142.1,S2V5RGF0YTQ=.40000.143.1,S2V5RGF0YTU=.50000.70.1|440,441|VerificationPayload THIS STRING IS MEANINGLESS";
        private const string EXPECTED_CLEAR_TEXT_V3_NOSYMPTOM = "jp.go.mhlw.cocoa.unit_test|2021-12-19T19:02:00.000+09:00|NoSymptom|S2V5RGF0YTE=.10000.140.1,S2V5RGF0YTI=.20000.141.1,S2V5RGF0YTM=.30000.142.1,S2V5RGF0YTQ=.40000.143.1,S2V5RGF0YTU=.50000.70.1|440,441|VerificationPayload THIS STRING IS MEANINGLESS";

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
        public void AndroidClearTextTestV1()
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

            string clearText = DeviceVerifierUtils.GetNonceClearTextV1(submissionParameter);
            Assert.Equal(
                EXPECTED_CLEAR_TEXT_V1,
                clearText
                );
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
        public void AndroidClearTextTestV3_HasSymptom()
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
                HasSymptom = true,
                Platform = platform,
                Regions = dummyRegions,
                OnSetOfSymptomOrTestDate = dummySymptomOnsetDate,
                Keys = dummyDiagnosisKeyDataList,
                DeviceVerificationPayload = dummyDeviceVerificationPayload,
                AppPackageName = dummyAppPackageName,
                VerificationPayload = dummyVerificationPayload,
                Padding = dummyPadding,
            };

            string clearText = DeviceVerifierUtils.GetNonceClearTextV3(submissionParameter);
            Assert.Equal(
                EXPECTED_CLEAR_TEXT_V3_HASSYMPTOM,
                clearText
                );
        }


        [Fact]
        public void AndroidClearTextTestV3_NoSymptom()
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
                HasSymptom = false,
                Platform = platform,
                Regions = dummyRegions,
                OnSetOfSymptomOrTestDate = dummySymptomOnsetDate,
                Keys = dummyDiagnosisKeyDataList,
                DeviceVerificationPayload = dummyDeviceVerificationPayload,
                AppPackageName = dummyAppPackageName,
                VerificationPayload = dummyVerificationPayload,
                Padding = dummyPadding,
            };

            string clearText = DeviceVerifierUtils.GetNonceClearTextV3(submissionParameter);
            Assert.Equal(
                EXPECTED_CLEAR_TEXT_V3_NOSYMPTOM,
                clearText
                );
        }
    }

    public class DeviceVerifierUtilsEventLogParametersTests
    {
        private const string JSON_EVENTLOG_SUBMISSION_PARAMETERS1 = "eventlog_submission_parameter1.json";
        private const string EXPECTED_CLEAR_TEXT_FILENAME = "eventlog_submission_parameter1-cleartext.txt";

        private static string GetTestJson(string fileName)
        {
            var path = TestDataUtils.GetLocalFilePath(fileName);
            using (var reader = File.OpenText(path))
            {
                return reader.ReadToEnd();
            }
        }

        [Fact]
        public void AndroidClearTextTestV3()
        {
            string expectedClearText = GetTestJson(EXPECTED_CLEAR_TEXT_FILENAME);
            string eventLogsText = GetTestJson(JSON_EVENTLOG_SUBMISSION_PARAMETERS1);

            V1EventLogRequest eventLogRequest = JsonConvert.DeserializeObject<V1EventLogRequest>(eventLogsText);

            string clearText = DeviceVerifierUtils.GetNonceClearTextV3(eventLogRequest);
            Assert.Equal(
                expectedClearText,
                clearText
                );

        }
    }
}
