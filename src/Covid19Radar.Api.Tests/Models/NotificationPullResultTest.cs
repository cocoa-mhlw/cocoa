using System;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Api.Tests.Models
{
    [TestClass]
    [TestCategory("Models")]
    public class NotificationPullResultTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new NotificationPullResult();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new NotificationPullResult();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

    }
}
