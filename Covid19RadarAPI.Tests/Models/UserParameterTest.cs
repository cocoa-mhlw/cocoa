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
        [DataRow("UUID", "0", "0", "UUID.00000.00000")]
        [DataRow("UUID", "11111", "22222", "UUID.11111.22222")]
        public void IdTest(string uuid, string major, string minor, string expected)
        {
            // preparation
            var model = new UserParameter();
            model.UserUuid = uuid;
            model.Major = major;
            model.Minor = minor;
            // action
            var actual = model.GetId();
            // assert
            Assert.AreEqual(expected, actual);
        }

    }
}
