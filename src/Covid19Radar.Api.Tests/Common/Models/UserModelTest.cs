/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Api.Tests.Common.Models
{
    [TestClass]
    [TestCategory("Models")]
    public class UserModelTest
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
        [DataRow("UUID1", "UUID1")]
        [DataRow("UUID2", "UUID2")]
        public void IdTest(string uuid, string expected)
        {
            // preparation
            var model = new UserModel();
            model.UserUuid = uuid;
            // action
            var actual = model.id;
            // assert
            Assert.AreEqual(expected, actual);
        }

    }
}
