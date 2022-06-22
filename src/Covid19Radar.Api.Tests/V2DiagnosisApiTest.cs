﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Http;

namespace Covid19Radar.Api.Tests
{
    [TestClass]
    [TestCategory("Api")]
    public class V2DiagnosisApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["SupportRegions"]).Returns("Region1,Region2");
            var tekRepo = new Mock<ITemporaryExposureKeyRepository>();
            var validationServer = new Mock<IValidationServerService>();
            var deviceCheck = new Mock<IDeviceValidationService>();
            var verification = new Mock<IVerificationService>();
            var logger = new Mock.LoggerMock<Covid19Radar.Api.V2DiagnosisApi>();
            var diagnosisApi = new V2DiagnosisApi(config.Object,
                                                tekRepo.Object,
                                                deviceCheck.Object,
                                                verification.Object,
                                                validationServer.Object,
                                                logger);
            // assert
            Assert.IsNotNull(diagnosisApi);
        }

        [DataTestMethod]
        [DataRow(true, true, "NotSupportRegion", "xxxxx", "ios", false, HttpStatusCode.BadRequest)]
        [DataRow(true, true, "NotSupportRegion", "xxxxx", "", false, HttpStatusCode.BadRequest)]
        [DataRow(false, false, "Region1", "xxxxx", "ios", false, HttpStatusCode.BadRequest)]
        [DataRow(true, false, "Region1", "xxxxx", "ios", false, HttpStatusCode.BadRequest)]
        [DataRow(true, true, "Region1", "xxxxx", "ios", false, HttpStatusCode.NoContent)]
        [DataRow(true, true, "NotSupportRegion", "xxxxx", "ios", true, HttpStatusCode.BadRequest)]
        [DataRow(true, true, "NotSupportRegion", "xxxxx", "", true, HttpStatusCode.BadRequest)]
        [DataRow(false, false, "Region1", "xxxxx", "ios", true, HttpStatusCode.BadRequest)]
        [DataRow(true, false, "Region1", "xxxxx", "ios", true, HttpStatusCode.BadRequest)]
        [DataRow(true, true, "Region1", "xxxxx", "ios", true, HttpStatusCode.NoContent)]
        public async Task RunAsyncMethod(bool isValid,
                                         bool isValidDevice,
                                         string region,
                                         string verificationPayload,
                                         string platform,
                                         bool isChaffRequest,
                                         HttpStatusCode expectedStatusCode
            )
        {
            // preparation
            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["SupportRegions"]).Returns("Region1,Region2");

            var tekRepo = new Mock<ITemporaryExposureKeyRepository>();
            var validationServer = new Mock<IValidationServerService>();
            validationServer.Setup(_ => _.Validate(It.IsAny<HttpRequest>())).Returns(IValidationServerService.ValidateResult.Success);


            var deviceCheck = new Mock<IDeviceValidationService>();
            deviceCheck.Setup(_ => _.Validation(It.IsAny<string>(), It.IsAny<V2DiagnosisSubmissionParameter>(), It.IsAny<DateTimeOffset>())).ReturnsAsync(isValidDevice);
            var verification = new Mock<IVerificationService>();
            var logger = new Mock.LoggerMock<Covid19Radar.Api.V2DiagnosisApi>();
            var diagnosisApi = new V2DiagnosisApi(config.Object,
                                                tekRepo.Object,
                                                deviceCheck.Object,
                                                verification.Object,
                                                validationServer.Object,
                                                logger);
            var context = new Mock<HttpContext>();
            var keydata = new byte[16];
            RandomNumberGenerator.Create().GetBytes(keydata);
            var keyDataString = Convert.ToBase64String(keydata);
            var startNumber = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 600;
            var bodyJson = new V2DiagnosisSubmissionParameter()
            {
                VerificationPayload = verificationPayload,
                Regions = new[] { region },
                Platform = platform,
                DeviceVerificationPayload = "DeviceVerificationPayload",
                AppPackageName = "Covid19Radar",
                Keys = new V2DiagnosisSubmissionParameter.Key[] {
                    new V2DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 0, RollingStartNumber = startNumber },
                    new V2DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 0, RollingStartNumber = startNumber } }
            };
            var bodyString = Newtonsoft.Json.JsonConvert.SerializeObject(bodyJson);
            using var stream = new System.IO.MemoryStream();
            using (var writer = new System.IO.StreamWriter(stream, leaveOpen: true))
            {
                await writer.WriteAsync(bodyString);
                await writer.FlushAsync();
            }
            stream.Seek(0, System.IO.SeekOrigin.Begin);

            context.Setup(_ => _.Request.Body).Returns(stream);

            if (isChaffRequest)
            {
                IHeaderDictionary headers = new HeaderDictionary() {
                    { "X-Chaff", "Foo" /* Server will check X-Chaff header existence, content no matter. */ }
                };
                context.Setup(_ => _.Request.Headers).Returns(headers);
            }

            // action
            var result = await diagnosisApi.RunAsync(context.Object.Request);

            // assert
            if (result is StatusCodeResult statusCodeResult)
            {
                Assert.AreEqual(((int)expectedStatusCode), statusCodeResult.StatusCode);
            }
            else if (result is BadRequestErrorMessageResult)
            {
                Assert.AreEqual(expectedStatusCode, HttpStatusCode.BadRequest);
            }
        }
    }
}
