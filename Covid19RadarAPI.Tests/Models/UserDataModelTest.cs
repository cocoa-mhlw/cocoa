using System;
using Covid19Radar.Common;
using Covid19Radar.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Tests.Models
{
    [TestClass]
    public class UserDataModelTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new UserModel();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new UserModel();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

        [DataTestMethod]
        [DataRow("UUID", "0", "0", "UUID.00000.00000")]
        [DataRow("UUID", "11111", "22222", "UUID.11111.22222")]
        public void IdTest(string uuid, string major, string minor, string expected)
        {
            // preparation
            var model = new UserModel();
            model.UserUuid = uuid;
            model.Major = major;
            model.Minor = minor;
            // action
            var actual = model.id;
            // assert
            Assert.AreEqual(expected, actual);
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
            var model = new UserModel();
            // action
            model.SetStatus(s);
            // assert
            var expected = Enum.GetName(typeof(UserStatus), s);
            Assert.AreEqual(expected, model.UserStatus);
        }
    }
}
