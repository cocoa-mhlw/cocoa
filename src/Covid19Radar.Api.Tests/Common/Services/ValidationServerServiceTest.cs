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



    }
}
