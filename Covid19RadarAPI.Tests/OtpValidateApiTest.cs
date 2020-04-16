using System.Threading.Tasks;
using Covid19Radar.Models;
using Covid19Radar.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Covid19Radar.Tests
{
    [TestClass]
    public class OtpValidateApiTest
    {
        private Mock<ILogger<OtpValidateApi>> _logger;
        private Mock<IOtpService> _mockOtpService;

        [TestInitialize]
        public void TestInit()
        {
            _logger = new Mock<ILogger<OtpValidateApi>>();
            _mockOtpService = new Mock<IOtpService>();
            _mockOtpService
                .Setup(m => m.ValidateAsync(It.IsAny<OtpValidateRequest>()))
                .Returns(Task.FromResult(true));
        }

        [TestCleanup]
        public void TestCleanup ()
        {
            _logger = null;
            _mockOtpService = null;
        }

        [TestMethod]
        public void CreateMethod()
        {
            //sut
            var otpApi = new OtpValidateApi(_mockOtpService.Object, _logger.Object);
        }

        [TestMethod]
        public void RunMethod()
        {
            var otpApi = new OtpValidateApi(_mockOtpService.Object, _logger.Object);
            var context = new Mock.HttpContextMock();
            // action
            otpApi.Run(context.Request);
            // assert
        }
    }
}
