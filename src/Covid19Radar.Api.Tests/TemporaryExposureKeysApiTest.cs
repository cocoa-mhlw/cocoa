using Covid19Radar.Api;
using Covid19Radar.DataAccess;
using Covid19Radar.Models;
using Covid19Radar.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Tests
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
            tekExportRepo.Setup(_ => _.GetKeysAsync(It.IsAny<ulong>())).ReturnsAsync(resultModels.ToArray());
            var logger = new Mock.LoggerMock<TemporaryExposureKeysApi>();
            var temporaryExposureKeysApi = new TemporaryExposureKeysApi(config.Object, tekExportRepo.Object, logger);
            var context = new Mock.HttpContextMock();
            var q = new Mock<IQueryCollection>();
            q.SetupGet(_ => _["since"]).Returns(since);
            context._Request.Query = q.Object;
            // action
            await temporaryExposureKeysApi.RunAsync(context.Request);
            // assert
        }
    }
}
