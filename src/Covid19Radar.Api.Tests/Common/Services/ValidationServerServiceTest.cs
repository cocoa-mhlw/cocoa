using Castle.Core.Configuration;
using Covid19Radar.Api;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests.Common.Services
{
    [TestClass]
    [TestCategory("Services")]
    public class ValidationServerServiceTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            var logger = new Mock<ILogger<ValidationServerService>>();
            // action
            var instance = new ValidationServerService(config.Object, logger.Object);
        }

        [DataTestMethod]
        [DataRow(true, new string[] { }, "")]
        [DataRow(true, new string[] { "" }, "")]
        [DataRow(true, new string[] { "1" }, "1")]
        [DataRow(false, new string[] { "1", "2", "3" }, "123")]
        [DataRow(false, new string[] { "1" }, "2")]
        public void ValidateMethod(bool expected, string[] headerValues, string frontDoorId)
        {
            // preparation
            var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            var logger = new Mock<ILogger<ValidationServerService>>();
            var req = new Mock<HttpRequest>();
            config.Setup(_ => _["AzureFrontDoorRestrictionEnabled"]).Returns("true");
            config.Setup(_ => _["AzureFrontDoorId"]).Returns(frontDoorId);
            req.Setup(_ => _.Headers["X-Azure-FDID"]).Returns(new StringValues(headerValues));
            var instance = new ValidationServerService(config.Object, logger.Object);
            // action
            var result = instance.Validate(req.Object);
            // assert
            Assert.AreEqual(expected, result.IsValid);
        }

    }
}
