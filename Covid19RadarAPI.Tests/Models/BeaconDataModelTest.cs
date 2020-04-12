using Covid19Radar.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Tests.Models
{
    [TestClass]
    public class BeaconDataModelTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new BeaconDataModel();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new BeaconDataModel();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

    }
}
