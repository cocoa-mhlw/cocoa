using Covid19Radar.Api;
using Covid19Radar.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Covid19Radar.Tests
{
    [TestClass]
    public class RegisterApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var userRepo = new Mock<IUserRepository>();
            var cryption = new Mock.CryptionServiceMock();
            var logger = new Mock.LoggerMock<RegisterApi>();
            var registerApi = new RegisterApi(userRepo.Object, cryption, logger);
        }

        [TestMethod]
        public void RunMethod()
        {
            // preparation
            var userRepo = new Mock<IUserRepository>();
            var cryption = new Mock.CryptionServiceMock();
            var logger = new Mock.LoggerMock<RegisterApi>();
            var registerApi = new RegisterApi(userRepo.Object, cryption, logger);
            var context = new Mock.HttpContextMock();
            // action
            registerApi.Run(context.Request);
            // assert
        }
    }
}
