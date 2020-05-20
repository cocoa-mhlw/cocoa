using Covid19Radar.Api;
using Covid19Radar.DataAccess;
using Covid19Radar.Services;
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
            var notification = new Mock.NotificationServiceMock();
            var validation = new Mock.ValidationUserServiceMock();
            var logger = new Mock.LoggerMock<UserApi>();
            var userApi = new UserApi(notification, validation, logger);
        }

        [DataTestMethod]
        [DataRow("")]
        public void RunMethod(string userUuid)
        {
            // preparation
            var notification = new Mock.NotificationServiceMock();
            var validation = new Mock.ValidationUserServiceMock();
            var logger = new Mock.LoggerMock<UserApi>();
            var userApi = new UserApi( notification, validation, logger);
            var context = new Mock.HttpContextMock();
            // action
            userApi.Run(context.Request, userUuid);
            // assert
        }
    }
}
