using System;
using Covid19Radar.Common;
using Covid19Radar.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Tests.Models
{
    [TestClass]
    public class UserParameterTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new UserParameter();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new UserParameter();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

        [DataTestMethod]
        [DataRow("UUID1", "UUID1")]
        [DataRow("UUID2", "UUID2")]
        public void IdTest(string uuid, string expected)
        {
            // preparation
            var model = new UserParameter();
            model.UserUuid = uuid;
            // action
            var actual = model.GetId();
            // assert
            Assert.AreEqual(expected, actual);
        }

    }
}
