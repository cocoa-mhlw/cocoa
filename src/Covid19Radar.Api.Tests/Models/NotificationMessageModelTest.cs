using Covid19Radar.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Tests.Models
{
    [TestClass]
    class NotificationMessageModelTest
    {

        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new NotificationMessageModel();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new NotificationMessageModel();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

    }
}
