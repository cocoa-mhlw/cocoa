using System;
using Covid19Radar.Api.External.Models;
using Covid19Radar.Common;
using Covid19Radar.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Api.Tests.External.Models
{
    [TestClass]
    [TestCategory("Models")]
    public class NotificationCreateParameterTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new NotificationCreateParameter();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new NotificationCreateParameter();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

    }
}
