using Covid19Radar.Api;
using Covid19Radar.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Covid19Radar.Tests
{
    [TestClass]
    public class UserApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var userRepo = new Mock<IUserRepository>();
            var notification = new Mock.NotificationServiceMock();
            var validation = new Mock.ValidationUserServiceMock();
            var logger = new Mock.LoggerMock<UserApi>();
            var userApi = new UserApi(userRepo.Object, notification, validation, logger);
        }

        [DataTestMethod]
        [DataRow("", "", "")]
        public void RunMethod(string userUuid, string userMajor, string userMinor)
        {
            // preparation
            var userRepo = new Mock<IUserRepository>();
            var notification = new Mock.NotificationServiceMock();
            var validation = new Mock.ValidationUserServiceMock();
            var logger = new Mock.LoggerMock<UserApi>();
            var userApi = new UserApi(userRepo.Object, notification, validation, logger);
            var context = new Mock.HttpContextMock();
            // action
            userApi.Run(context.Request, userUuid, userMajor, userMinor);
            // assert
        }
    }
}
