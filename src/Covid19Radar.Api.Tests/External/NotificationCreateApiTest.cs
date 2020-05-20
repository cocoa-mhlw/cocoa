using Covid19Radar.DataAccess;
using Covid19Radar.DataStore;
using Covid19Radar.Models;
using Covid19Radar.Services;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests.External
{
    [TestClass]
    [TestCategory("ExternalApi")]
    public class NotificationCreateApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var cosmos = new Mock<ICosmos>();
            var notification = new Mock<INotificationService>();
            var logger = new Mock.LoggerMock<Covid19Radar.Api.External.NotificationCreateApi>();
            var notificationCreateApi = new Covid19Radar.Api.External.NotificationCreateApi(cosmos.Object, notification.Object, logger);
        }

        [DataTestMethod]
        [DataRow(null, null, typeof(BadRequestObjectResult))]
        [DataRow(null, "", typeof(BadRequestObjectResult))]
        [DataRow("", null, typeof(BadRequestObjectResult))]
        [DataRow("", "", typeof(BadRequestObjectResult))]
        [DataRow("1", null, typeof(BadRequestObjectResult))]
        [DataRow("1", "", typeof(BadRequestObjectResult))]
        [DataRow(null, "1", typeof(BadRequestObjectResult))]
        [DataRow("", "1", typeof(BadRequestObjectResult))]
        [DataRow("1", "1", typeof(OkObjectResult))]
        public async Task RunAsyncMethod(string title, string message, System.Type ResultType)
        {
            // preparation
            var result = new Mock<ItemResponse<NotificationMessageModel>>();
            result.SetupGet(_ => _.Resource)
                .Returns(new NotificationMessageModel() { Message = message });
            var cosmosNotification = new Mock<Container>();
            cosmosNotification.Setup(_ => _.CreateItemAsync(It.IsAny<NotificationMessageModel>(),
                                                            It.IsAny<PartitionKey?>(),
                                                            It.IsAny<ItemRequestOptions>(),
                                                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(result.Object);
            var cosmos = new Mock<ICosmos>();
            cosmos.SetupGet(_ => _.Notification)
                .Returns(cosmosNotification.Object);
            var notification = new Mock<INotificationService>();
            var logger = new Mock.LoggerMock<Covid19Radar.Api.External.NotificationCreateApi>();
            var notificationCreateApi = new Covid19Radar.Api.External.NotificationCreateApi(cosmos.Object, notification.Object, logger);
            var context = new Mock.HttpContextMock();
            var param = new Api.External.Models.NotificationCreateParameter()
            {
                Title = title,
                Message = message
            };
            var bodyString = Newtonsoft.Json.JsonConvert.SerializeObject(param);
            using var stream = new System.IO.MemoryStream();
            using (var writer = new System.IO.StreamWriter(stream, leaveOpen: true))
            {
                await writer.WriteAsync(bodyString);
                await writer.FlushAsync();
            }
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            context._Request.Body = stream;

            // action
            var actual = await notificationCreateApi.RunAsync(context.Request);
            // assert
            Assert.IsInstanceOfType(actual, ResultType);
        }

    }
}
