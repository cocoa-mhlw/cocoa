using Covid19Radar.Api;
using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Api.Tests
{
    [TestClass]
    [TestCategory("Api")]
    public class NotificationPullApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var userRepo = new Mock<IUserRepository>();
            var notification = new Mock<INotificationService>();
            var logger = new Mock.LoggerMock<NotificationPullApi>();
            var notificationPullApi = new NotificationPullApi(userRepo.Object, notification.Object, logger);
        }

        [DataTestMethod]
        [DataRow(2020, 7, 1, DisplayName ="lastClientUpdateTime")]
        [DataRow(2020, 6, 1, DisplayName ="lastClientUpdateTime")]
        [DataRow(2020, 5, 1, DisplayName ="lastClientUpdateTime")]
        public void RunMethod(int year, int month, int day)
        {
            // preparation
            var userRepo = new Mock<IUserRepository>();
            var notification = new Mock<INotificationService>();
            var logger = new Mock.LoggerMock<NotificationPullApi>();
            var notificationPullApi = new NotificationPullApi(userRepo.Object, notification.Object, logger);
            var context = new Mock.HttpContextMock();
            // action
            notificationPullApi.Run(context.Request, new DateTime(year, month, day));
            // assert
        }
    }
}
