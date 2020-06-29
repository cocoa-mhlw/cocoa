using Covid19Radar.Api;
using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
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
            var validationServer = new Mock<IValidationServerService>();
            var logger = new Mock.LoggerMock<RegisterApi>();
            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["AzureFrontDoorRestrictionEnabled"]).Returns("true");
            config.Setup(_ => _["AzureFrontDoorId"]).Returns("3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5");
            var req = new Mock<HttpRequest>();
            req.Setup(_ => _.Headers["X-Azure-FDID"]).Returns("3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5");
            var registerApi = new RegisterApi(userRepo.Object, cryption.Object, validationServer.Object, logger);
        }

        [TestMethod]
        public async Task RunAsyncMethod()
        {
            // preparation
            var userRepo = new Mock<IUserRepository>();
            var cryption = new Mock<ICryptionService>();
            var validationServer = new Mock<IValidationServerService>();
            var logger = new Mock.LoggerMock<RegisterApi>();

            var registerApi = new RegisterApi(userRepo.Object, cryption.Object, validationServer.Object, logger);
            var context = new Mock<HttpContext>();
            context.Setup(_ => _.Request.Headers["X-Azure-FDID"]).Returns("3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5");

            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["AzureFrontDoorRestrictionEnabled"]).Returns("true");
            config.Setup(_ => _["AzureFrontDoorId"]).Returns("3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5");
            // action
            await registerApi.RunAsync(context.Object.Request);
            // assert
        }
    }
}
