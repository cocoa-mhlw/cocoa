using Covid19Radar.Api;
using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests
{
    [TestClass]
    [TestCategory("Api")]
    public class UserApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var notification = new Mock<INotificationService>();
            var validation = new Mock<IValidationUserService>();
            var logger = new Mock.LoggerMock<UserApi>();
            var userApi = new UserApi(notification.Object, validation.Object, logger);
        }

        [DataTestMethod]
        [DataRow(true, "")]
        [DataRow(true, "UserUuid")]
        [DataRow(false, null)]
        public async Task RunAsyncMethod(bool isValid, string userUuid)
        {
            // preparation
            var notification = new Mock<INotificationService>();
            var validation = new Mock<IValidationUserService>();
            var validationResult = new IValidationUserService.ValidateResult()
            {
                IsValid = isValid
            };
            validation.Setup(_ => _.ValidateAsync(It.IsAny<HttpRequest>(), It.IsAny<IUser>())).ReturnsAsync(validationResult);
            var logger = new Mock.LoggerMock<UserApi>();
            var userApi = new UserApi(notification.Object, validation.Object, logger);
            var context = new Mock<HttpContext>();
            // action
            await userApi.RunAsync(context.Object.Request, userUuid);
            // assert
        }
    }
}
