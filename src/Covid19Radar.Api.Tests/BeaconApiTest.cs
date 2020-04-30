using Covid19Radar.Api;
using Covid19Radar.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Covid19Radar.Tests
{
    [TestClass]
    public class BeaconApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var userRepo = new Mock<IUserRepository>();
            var beaconRepo = new Mock<IBeaconRepository>();
            var validation = new Mock.ValidationUserServiceMock();
            var logger = new Mock.LoggerMock<BeaconApi>();
            var beaconApi = new BeaconApi(beaconRepo.Object, userRepo.Object, validation, logger);
        }

        [TestMethod]
        public void RunMethod()
        {
            // preparation
            var userRepo = new Mock<IUserRepository>();
            var beaconRepo = new Mock<IBeaconRepository>();
            var validation = new Mock.ValidationUserServiceMock();
            var logger = new Mock.LoggerMock<BeaconApi>();
            var beaconApi = new BeaconApi(beaconRepo.Object, userRepo.Object, validation, logger);
            var context = new Mock.HttpContextMock();
            // action
            beaconApi.Run(context.Request);
            // assert
        }
    }
}
