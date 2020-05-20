using Covid19Radar.Api;
using Covid19Radar.DataAccess;
using Covid19Radar.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Covid19Radar.Tests
{
    [TestClass]
    [TestCategory("Api")]
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
        [DataRow("UserUuid")]
        [DataRow(null)]
        public async Task RunAsyncMethod(string userUuid)
        {
            // preparation
            var notification = new Mock.NotificationServiceMock();
            var validation = new Mock.ValidationUserServiceMock();
            var logger = new Mock.LoggerMock<UserApi>();
            var userApi = new UserApi( notification, validation, logger);
            var context = new Mock.HttpContextMock();
            // action
            await userApi.RunAsync(context.Request, userUuid);
            // assert
        }
    }
}
