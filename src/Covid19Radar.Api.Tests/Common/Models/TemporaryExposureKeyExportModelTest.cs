using System;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Api.Tests.Common.Models
{
    [TestClass]
    [TestCategory("Models")]
    public class TemporaryExposureKeyExportModelTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new TemporaryExposureKeyExportModel();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new TemporaryExposureKeyExportModel();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }


    }
}
