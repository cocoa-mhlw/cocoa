using System.Threading.Tasks;
using Covid19Radar.Models;
using Covid19Radar.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Covid19Radar.Tests
{
    [TestClass]
    public class OtpGenerateApiTest
    {
        private Mock<ILogger<OtpSendApi>> _logger;
        private Mock<IOtpService> _mockOtpService;

        [TestInitialize]
        public void TestInit()
        {
            _logger = new Mock<ILogger<OtpSendApi>>();
            _mockOtpService = new Mock<IOtpService>();
            _mockOtpService
                .Setup(m => m.SendAsync(It.IsAny<OtpSendRequest>()))
                .Returns(Task.FromResult(Task.CompletedTask));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _logger = null;
            _mockOtpService = null;
        }

        [TestMethod]
        public void CreateMethod()
        {
            var otpApi = new OtpSendApi(_mockOtpService.Object,_logger.Object);
        }

        [TestMethod]
        public void RunMethod()
        {
            var otpApi = new OtpSendApi(_mockOtpService.Object, _logger.Object);
            var context = new Mock.HttpContextMock();
            // action
            otpApi.Run(context.Request);
            // assert
        }
    }
}
