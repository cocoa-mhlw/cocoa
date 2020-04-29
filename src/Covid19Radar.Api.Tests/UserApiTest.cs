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
            var notification = new Mock.NotificationServiceMock();
            var validation = new Mock.ValidationUserServiceMock();
            var logger = new Mock.LoggerMock<UserApi>();
            var userApi = new UserApi(cosmos, notification, validation, logger);
        }

        [TestMethod]
        public void RunMethod()
        {
            // preparation
            var cosmos = new Mock.CosmosMock();
            var notification = new Mock.NotificationServiceMock();
            var validation = new Mock.ValidationUserServiceMock();
            var logger = new Mock.LoggerMock<UserApi>();
            var userApi = new UserApi(cosmos, notification, validation, logger);
            var context = new Mock.HttpContextMock();
            // action
            userApi.Run(context.Request);
            // assert
        }
    }
}
