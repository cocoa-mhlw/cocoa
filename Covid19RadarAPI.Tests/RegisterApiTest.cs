using Covid19Radar.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Tests
{
    [TestClass]
    public class RegisterApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var cosmos = new Mock.CosmosMock();
            var cryption = new Mock.CryptionServiceMock();
            var logger = new Mock.LoggerMock<RegisterApi>();
            var registerApi = new RegisterApi(cosmos, cryption, logger);
        }

        [TestMethod]
        public void RunMethod()
        {
            // preparation
            var cosmos = new Mock.CosmosMock();
            var cryption = new Mock.CryptionServiceMock();
            var logger = new Mock.LoggerMock<RegisterApi>();
            var registerApi = new RegisterApi(cosmos, cryption, logger);
            var context = new Mock.HttpContextMock();
            // action
            registerApi.Run(context.Request);
            // assert
        }
    }
}
