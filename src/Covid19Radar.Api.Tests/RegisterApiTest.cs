using Covid19Radar.Api;
using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests
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
            var cryption = new Mock<ICryptionService>();
            var logger = new Mock.LoggerMock<RegisterApi>();
            var registerApi = new RegisterApi(userRepo.Object, cryption.Object, logger);
        }

        [TestMethod]
        public async Task RunAsyncMethod()
        {
            // preparation
            var userRepo = new Mock<IUserRepository>();
            var cryption = new Mock<ICryptionService>();
            var logger = new Mock.LoggerMock<RegisterApi>();
            var registerApi = new RegisterApi(userRepo.Object, cryption.Object, logger);
            var context = new Mock<HttpContext>();
            // action
            await registerApi.RunAsync(context.Object.Request);
            // assert
        }
    }
}
