using Covid19Radar.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Tests
{
    [TestClass]
    public class UserApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var cosmos = new Mock.CosmosMock();
            var userApi = new UserApi(cosmos);
        }

        [TestMethod]
        public void RunMethod()
        {
            // preparation
            var cosmos = new Mock.CosmosMock();
            var userApi = new UserApi(cosmos);
            var logger = new Mock.LoggerMock();
            var context = new Mock.HttpContextMock();
            // action
            userApi.Run(context.Request, logger);
            // assert
        }
    }
}
