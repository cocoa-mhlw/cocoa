using Covid19Radar.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Tests.Models
{
    [TestClass]
    public class BeaconParameterTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new BeaconParameter();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new BeaconParameter();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

    }
}
