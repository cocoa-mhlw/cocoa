using Covid19Radar.Api;
using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests
{
    [TestClass]
    [TestCategory("Api")]
    public class TemporaryExposureKeysApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var config = new Mock<IConfiguration>();
            var tekExportRepo = new Mock<ITemporaryExposureKeyExportRepository>();
            var logger = new Mock.LoggerMock<TemporaryExposureKeysApi>();
            var temporaryExposureKeysApi = new TemporaryExposureKeysApi(config.Object, tekExportRepo.Object, logger);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("1")]
        [DataRow("100000")]
        [DataRow("10000000000")]
        public async Task RunAsyncMethod(string since)
        {
            // preparation
            var config = new Mock<IConfiguration>();
            var tekExportRepo = new Mock<ITemporaryExposureKeyExportRepository>();
            var resultModels = new List<TemporaryExposureKeyExportModel>();
            for (var i = 0; i < 50; i++)
            {
                resultModels.Add(new TemporaryExposureKeyExportModel());
            }
            tekExportRepo.Setup(_ => _.GetKeysAsync(It.IsAny<ulong>()))
                .ReturnsAsync(resultModels.ToArray());
            var logger = new Mock.LoggerMock<TemporaryExposureKeysApi>();
            var temporaryExposureKeysApi = new TemporaryExposureKeysApi(config.Object, tekExportRepo.Object, logger);
            var context = new Mock<HttpContext>();
            var q = new Mock<IQueryCollection>();
            q.SetupGet(_ => _["since"]).Returns(since);
            context.Setup(_ => _.Request.Query).Returns(q.Object);
            // action
            await temporaryExposureKeysApi.RunAsync(context.Object.Request);
            // assert
        }

        [TestMethod]
        public async Task RunAsyncMethodWithNull()
        {
            // preparation
            var config = new Mock<IConfiguration>();
            var tekExportRepo = new Mock<ITemporaryExposureKeyExportRepository>();
            var resultModels = new List<TemporaryExposureKeyExportModel>();
            tekExportRepo.Setup(_ => _.GetKeysAsync(It.IsAny<ulong>())).ReturnsAsync(resultModels.ToArray());
            var logger = new Mock.LoggerMock<TemporaryExposureKeysApi>();
            var temporaryExposureKeysApi = new TemporaryExposureKeysApi(config.Object, tekExportRepo.Object, logger);
            var context = new Mock<HttpContext>();
            context.Setup(_ => _.Request.Query).Returns<IQueryCollection>(null);
            // action
            await temporaryExposureKeysApi.RunAsync(context.Object.Request);
            // assert
        }

    }
}
