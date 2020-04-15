using System.Threading.Tasks;
using Covid19Radar.Api;
using Covid19Radar.Models;
using Covid19Radar.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Covid19Radar.Tests
{
    [TestClass]
    public class OtpApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var logger = new Mock.LoggerMock<OtpSendApi>();
            var mockOtpService = new Moq.Mock<IOtpService>();
            mockOtpService.Setup(m => m.SendAsync(It.IsAny<OtpSendRequest>()))
                .Returns(Task.FromResult(Task.CompletedTask));
            var otpApi = new OtpSendApi(mockOtpService.Object,logger);
        }

        [TestMethod]
        public void RunMethod()
        {
            // preparation
            var logger = new Mock.LoggerMock<OtpSendApi>();
            var mockOtpService = new Moq.Mock<IOtpService>();
            mockOtpService.Setup(m => m.SendAsync(It.IsAny<OtpSendRequest>()))
                .Returns(Task.FromResult(Task.CompletedTask));
            var otpApi = new OtpSendApi(mockOtpService.Object, logger);
            var context = new Mock.HttpContextMock();
            // action
            otpApi.Run(context.Request);
            // assert
        }
    }
}
