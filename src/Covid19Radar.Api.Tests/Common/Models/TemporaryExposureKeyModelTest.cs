
using System;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.Models;
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
            Helper.ModelTestHelper.PropetiesTest(model);
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

        [TestMethod]
        public void ToKeyMethodTest()
        {
            // preparation
            var model = new TemporaryExposureKeyModel();
            model.KeyData = new byte[64];
            // action
            var actual = model.ToKey();
            CollectionAssert.AreEqual(model.KeyData, actual.KeyData.ToByteArray());
            Assert.AreEqual(model.RollingPeriod, actual.RollingPeriod);
            Assert.AreEqual(model.RollingStartIntervalNumber, actual.RollingStartIntervalNumber);
            Assert.AreEqual(model.TransmissionRiskLevel, actual.TransmissionRiskLevel);
        }

    }
}
