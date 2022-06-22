﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */


using Covid19Radar.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Api.Tests.Common.Models
{
    [TestClass]
    [TestCategory("Models")]
    public class SequenceModelTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new SequenceModel();
            // assert
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new SequenceModel();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }


    }
}
