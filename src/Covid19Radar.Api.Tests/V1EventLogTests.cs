/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Covid19Radar.UnitTests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Covid19Radar.Api.Tests
{
    [TestClass]
    [TestCategory("Api")]
    public class V1EventLogTests
    {
        private static string GetTestJson(string fileName)
        {
            var path = TestDataUtils.GetLocalFilePath(fileName);
            using (var reader = File.OpenText(path))
            {
                return reader.ReadToEnd();
            }
        }

        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var eventLogRepository = new Mock<IEventLogRepository>();
            var validationServer = new Mock<IValidationServerService>();
            var deviceValidationService = new Mock<IDeviceValidationService>();
            var logger = new Mock.LoggerMock<V1EventLog>();

            var eventLogApi = new V1EventLog(
                eventLogRepository.Object,
                validationServer.Object,
                deviceValidationService.Object,
                logger
                );
        }

        [DataTestMethod]
        [DataRow(true, true, -1, "eventlog_submission_parameter1.json", HttpStatusCode.BadRequest)]
        [DataRow(false, false, 250, "eventlog_submission_parameter1.json", 418)]
        [DataRow(true, false, 250, "eventlog_submission_parameter1.json", HttpStatusCode.BadRequest)]
        [DataRow(false, true, 250, "eventlog_submission_parameter1.json", 418)]
        [DataRow(true, true, 250, "eventlog_submission_parameter1.json", HttpStatusCode.OK)]
        [DataRow(true, true, Constants.MAX_EVENT_LOG_PAYLOAD + 1, "eventlog_submission_parameter1.json", HttpStatusCode.RequestEntityTooLarge)]
        public async Task RunAsyncMethod(
            bool isValidRoute,
            bool isValidDevice,
            long contentLength,
            string jsonFileName,
            int expectedStatusCode
    )
        {
            // preparation
            var eventLogRepository = new Mock<IEventLogRepository>();
            var validationServer = new Mock<IValidationServerService>();
            var deviceValidationService = new Mock<IDeviceValidationService>();
            var logger = new Mock.LoggerMock<V1EventLog>();

            var eventLogApi = new V1EventLog(
                eventLogRepository.Object,
                validationServer.Object,
                deviceValidationService.Object,
                logger
                );

            if (isValidRoute)
            {
                validationServer
                    .Setup(x => x.Validate(It.IsAny<HttpRequest>()))
                    .Returns(IValidationServerService.ValidateResult.Success);
            }
            else
            {
                validationServer
                    .Setup(x => x.Validate(It.IsAny<HttpRequest>()))
                    .Returns(IValidationServerService.ValidateResult.InvalidAzureFrontDoorId);
            }

            deviceValidationService
                .Setup(x => x.Validation(It.IsAny<string>(), It.IsAny<IDeviceVerification>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(isValidDevice);


            var context = new Mock<HttpContext>();

            // Conetnt-Length header
            context.Setup(_ => _.Request.Headers).Returns(new HeaderDictionary());
            if (contentLength >= 0)
            {
                IHeaderDictionary headers = new HeaderDictionary() {
                    { "Content-Length", $"{contentLength}" }
                };
                context.Setup(_ => _.Request.Headers).Returns(headers);
            }

            // body
            string bodyString = GetTestJson(jsonFileName);
            using var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                await writer.WriteAsync(bodyString);
                await writer.FlushAsync();
            }
            stream.Seek(0, SeekOrigin.Begin);
            context.Setup(_ => _.Request.Body).Returns(stream);

            // action
            var result = await eventLogApi.RunAsync(context.Object.Request);

            if (result is StatusCodeResult statusCodeResult)
            {
                Assert.AreEqual(expectedStatusCode, statusCodeResult.StatusCode);
            }

        }
    }
}
