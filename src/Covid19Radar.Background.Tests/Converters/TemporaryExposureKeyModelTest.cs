/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Models;
using Covid19Radar.Background.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Background.Tests.Converters
{
    [TestClass]
    [TestCategory("Models")]
    public class TemporaryExposureKeyModelTest
    {
        [TestMethod]
        public void ToKeyMethodTest()
        {
            // preparation
            var model = new TemporaryExposureKeyModel();
            model.KeyData = new byte[64];
            // action
            var actual = TemporaryExposureKeyConverter.ConvertToKey(model);
            CollectionAssert.AreEqual(model.KeyData, actual.KeyData.ToByteArray());
            Assert.AreEqual(model.RollingPeriod, actual.RollingPeriod);
            Assert.AreEqual(model.RollingStartIntervalNumber, actual.RollingStartIntervalNumber);
            Assert.AreEqual(model.TransmissionRiskLevel, actual.TransmissionRiskLevel);
        }
    }
}
