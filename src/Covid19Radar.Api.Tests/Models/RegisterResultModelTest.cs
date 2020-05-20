using System;
using Covid19Radar.Common;
using Covid19Radar.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Api.Tests.Models
{
    [TestClass]
    [TestCategory("Models")]
    public class RegisterResultModelTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new RegisterResultModel();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new RegisterResultModel();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

    }
}
