using System;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Api.Tests.Models
{
    [TestClass]
    [TestCategory("Models")]
    public class NotificationPullParameterTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new NotificationPullParameter();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new NotificationPullParameter();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

    }
}
