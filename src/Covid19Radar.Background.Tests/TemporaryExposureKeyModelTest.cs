/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Models;
using Covid19Radar.Api.Tests.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Api.Tests.Common.Models
{
    [TestClass]
    [TestCategory("Models")]
    public class TemporaryExposureKeyModelTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new TemporaryExposureKeyModel();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new TemporaryExposureKeyModel();
            // model property access
            ModelTestHelper.PropetiesTest(model);
        }

        [DataTestMethod]
        [DataRow(600, 1)]
        [DataRow(6000, 10)]
        [DataRow(0, 0)]
        public void GetRollingPeriodSecondsMethod(int expected, int period)
        {
            // preparation
            var model = new TemporaryExposureKeyModel();
            model.RollingPeriod = period;
            // action
            Assert.AreEqual(expected, model.GetRollingPeriodSeconds());
        }

        [DataTestMethod]
        [DataRow(600, 1)]
        [DataRow(6000, 10)]
        [DataRow(0, 0)]
        public void GetRollingStartUnixTimeSecondsMethod(int expected, int start)
        {
            // preparation
            var model = new TemporaryExposureKeyModel();
            model.RollingStartIntervalNumber = start;
            // action
            Assert.AreEqual(expected, model.GetRollingStartUnixTimeSeconds());
        }
    }
}
