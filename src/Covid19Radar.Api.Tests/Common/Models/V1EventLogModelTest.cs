/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.IO;
using Covid19Radar.Api.Models;
using Covid19Radar.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Covid19Radar.Api.Tests.Models
{
    [TestClass]
    [TestCategory("Models")]
    public class DiagnosisApiTest
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

        [TestMethod]
        public void CreateMethod()
        {
            string expectedClearText = GetTestJson(EXPECTED_CLEAR_TEXT_FILENAME);
            string eventLogsText = GetTestJson(JSON_EVENTLOG_SUBMISSION_PARAMETERS1);

            V1EventLogSubmissionParameter eventLogRequest = JsonConvert.DeserializeObject<V1EventLogSubmissionParameter>(eventLogsText);
            string clearText = eventLogRequest.ClearText;

            Assert.AreEqual(expectedClearText, clearText);
        }
    }
}
