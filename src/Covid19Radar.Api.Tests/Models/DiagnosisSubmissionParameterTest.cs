/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Linq;
using System.Text;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.Models;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Api.Tests.Models
{
    [TestClass]
    [TestCategory("Models")]
    public class DiagnosisSubmissionParameterTest
    {
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

    }
}
