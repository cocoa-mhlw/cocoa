using Covid19Radar.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Tests
{
    [TestClass]
    public class InfectionApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var cosmos = new Mock.CosmosMock();
            var logger = new Mock.LoggerMock<InfectionApi>();
            var userApi = new InfectionApi(cosmos, logger);
        }

        [TestMethod]
        public void RunMethod()
        {
            // preparation
            var cosmos = new Mock.CosmosMock();
            var logger = new Mock.LoggerMock<InfectionApi>();
            var userApi = new InfectionApi(cosmos, logger);
            var context = new Mock.HttpContextMock();
            // action
            userApi.Run(context.Request);
            // assert
        }
    }
}
