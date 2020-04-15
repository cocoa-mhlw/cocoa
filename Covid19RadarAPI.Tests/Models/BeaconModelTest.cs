using Covid19Radar.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Tests.Models
{
    [TestClass]
    public class BeaconModelTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new BeaconModel();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new BeaconModel();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

    }
}
