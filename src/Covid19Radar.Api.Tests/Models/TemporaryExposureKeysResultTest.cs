/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Api.Tests.Models
{
    [TestClass]
    [TestCategory("Models")]
    public class TemporaryExposureKeysResultTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new TemporaryExposureKeysResult();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new TemporaryExposureKeysResult();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

    }
}
