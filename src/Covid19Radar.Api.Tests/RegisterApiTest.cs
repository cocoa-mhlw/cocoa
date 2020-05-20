using Covid19Radar.Api;
using Covid19Radar.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Covid19Radar.Tests
{
    [TestClass]
    [TestCategory("Api")]
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
        public async Task RunAsyncMethod()
        {
            // preparation
            var userRepo = new Mock<IUserRepository>();
            var cryption = new Mock.CryptionServiceMock();
            var logger = new Mock.LoggerMock<RegisterApi>();
            var registerApi = new RegisterApi(userRepo.Object, cryption, logger);
            var context = new Mock.HttpContextMock();
            // action
            await registerApi.RunAsync(context.Request);
            // assert
        }
    }
}
