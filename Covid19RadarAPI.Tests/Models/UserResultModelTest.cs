using System;
using Covid19Radar.Common;
using Covid19Radar.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Tests.Models
{
    [TestClass]
    public class UserResultModelTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new UserResultModel();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new UserResultModel();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

        [DataTestMethod]
        [DataRow(UserStatus.Contactd)]
        [DataRow(UserStatus.Infection)]
        [DataRow(UserStatus.Inspection)]
        [DataRow(UserStatus.None)]
        [DataRow(UserStatus.OnSet)]
        [DataRow(UserStatus.Recovery)]
        [DataRow(UserStatus.Suspected)]
        [DataRow(UserStatus.Treatment)]
        public void SetStatusTest(UserStatus s)
        {
            // preparation
            var model = new UserResultModel();
            // action
            model.SetStatus(s);
            // assert
            var expected = Enum.GetName(typeof(UserStatus), s);
            Assert.AreEqual(expected, model.UserStatus);
        }
    }
}
