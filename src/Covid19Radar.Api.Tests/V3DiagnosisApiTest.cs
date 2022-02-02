/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Common;
using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Extensions;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Http;

namespace Covid19Radar.Api.Tests
{
    [TestClass]
    [TestCategory("Api")]
    public class V3DiagnosisApiTest
    {
        private const int KEY_LENGTH = 16;

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
            var temporaryExposureKeyValidationService = new Mock<ITemporaryExposureKeyValidationService>();
            var logger = new Mock.LoggerMock<V3DiagnosisApi>();
            var diagnosisApi = new V3DiagnosisApi(config.Object,
                                                tekRepo.Object,
                                                deviceCheck.Object,
                                                verification.Object,
                                                validationServer.Object,
                                                temporaryExposureKeyValidationService.Object,
                                                logger);
        }

        [DataTestMethod]
        [DataRow(true, true, "NotSupportRegion", "xxxxx", "ios", false, HttpStatusCode.BadRequest)]
        [DataRow(true, true, "NotSupportRegion", "xxxxx", "", false, HttpStatusCode.BadRequest)]
        [DataRow(false, false, "Region1", "xxxxx", "ios", false, HttpStatusCode.BadRequest)]
        [DataRow(true, false, "Region1", "xxxxx", "ios", false, HttpStatusCode.BadRequest)]
        [DataRow(true, true, "Region1", "xxxxx", "ios", false, HttpStatusCode.OK)]
        [DataRow(true, true, "NotSupportRegion", "xxxxx", "ios", true, HttpStatusCode.BadRequest)]
        [DataRow(true, true, "NotSupportRegion", "xxxxx", "", true, HttpStatusCode.BadRequest)]
        [DataRow(false, false, "Region1", "xxxxx", "ios", true, HttpStatusCode.BadRequest)]
        [DataRow(true, false, "Region1", "xxxxx", "ios", true, HttpStatusCode.BadRequest)]
        [DataRow(true, true, "Region1", "xxxxx", "ios", true, HttpStatusCode.OK)]
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
            var diagnosisRepo = new Mock<IDiagnosisRepository>();
            diagnosisRepo.Setup(_ => _.SubmitDiagnosisAsync(It.IsAny<string>(),
                                                            It.IsAny<DateTimeOffset>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<TemporaryExposureKeyModel[]>()))
                .ReturnsAsync(new DiagnosisModel());
            var tekRepo = new Mock<ITemporaryExposureKeyRepository>();
            var validationServer = new Mock<IValidationServerService>();
            validationServer.Setup(_ => _.Validate(It.IsAny<HttpRequest>())).Returns(IValidationServerService.ValidateResult.Success);


            var deviceCheck = new Mock<IDeviceValidationService>();
            deviceCheck.Setup(_ => _.Validation(It.IsAny<string>(), It.IsAny<V3DiagnosisSubmissionParameter>(), It.IsAny<DateTimeOffset>())).ReturnsAsync(isValidDevice);
            var verification = new Mock<IVerificationService>();

            var temporaryExposureKeyValidationService = new Mock<ITemporaryExposureKeyValidationService>();
            temporaryExposureKeyValidationService.Setup(x => x.Validate(It.IsAny<bool>(), It.IsAny<V3DiagnosisSubmissionParameter.Key>())).Returns(true);

            var logger = new Mock.LoggerMock<V3DiagnosisApi>();
            var diagnosisApi = new V3DiagnosisApi(config.Object,
                                                tekRepo.Object,
                                                deviceCheck.Object,
                                                verification.Object,
                                                validationServer.Object,
                                                temporaryExposureKeyValidationService.Object,
                                                logger);
            var context = new Mock<HttpContext>();
            var keydata = new byte[KEY_LENGTH];
            RandomNumberGenerator.Create().GetBytes(keydata);
            var keyDataString = Convert.ToBase64String(keydata);

            var dateTime = DateTime.UtcNow.AddDays(-7).Date;

            var bodyJson = new V3DiagnosisSubmissionParameter()
            {
                SymptomOnsetDate = dateTime.ToString(Constants.FORMAT_TIMESTAMP),
                VerificationPayload = verificationPayload,
                Regions = new[] { region },
                Platform = platform,
                DeviceVerificationPayload = "DeviceVerificationPayload",
                AppPackageName = "Covid19Radar",
                Keys = new V3DiagnosisSubmissionParameter.Key[] {
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-8).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-7).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-6).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-5).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-4).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-3).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-2).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-1).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(0).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(1).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(2).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(3).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(4).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(5).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(6).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(7).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(8).ToRollingStartNumber(), ReportType = 7 },
                }
            };
            var bodyString = JsonConvert.SerializeObject(bodyJson);
            using var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                await writer.WriteAsync(bodyString);
                await writer.FlushAsync();
            }
            stream.Seek(0, SeekOrigin.Begin);

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
            if (result is OkObjectResult okObjectResult)
            {
                Assert.AreEqual(((int)expectedStatusCode), okObjectResult.StatusCode);
            }
            else if (result is BadRequestErrorMessageResult)
            {
                Assert.AreEqual(expectedStatusCode, HttpStatusCode.BadRequest);
            }
        }

        [DataTestMethod]
        [DataRow(true, true, "Region1", "xxxxx", "ios", true, HttpStatusCode.OK)]
        public async Task SetDaysSinceOnsetOfSymptomsTest(bool isValid,
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
            var diagnosisRepo = new Mock<IDiagnosisRepository>();
            diagnosisRepo.Setup(_ => _.SubmitDiagnosisAsync(It.IsAny<string>(),
                                                            It.IsAny<DateTimeOffset>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<TemporaryExposureKeyModel[]>()))
                .ReturnsAsync(new DiagnosisModel());
            var tekRepo = new Mock<ITemporaryExposureKeyRepository>();
            var validationServer = new Mock<IValidationServerService>();
            validationServer.Setup(_ => _.Validate(It.IsAny<HttpRequest>())).Returns(IValidationServerService.ValidateResult.Success);


            var deviceCheck = new Mock<IDeviceValidationService>();
            deviceCheck.Setup(_ => _.Validation(It.IsAny<string>(), It.IsAny<V3DiagnosisSubmissionParameter>(), It.IsAny<DateTimeOffset>())).ReturnsAsync(isValidDevice);
            var verification = new Mock<IVerificationService>();

            var temporaryExposureKeyValidationService = new Mock<ITemporaryExposureKeyValidationService>();
            temporaryExposureKeyValidationService.Setup(x => x.Validate(It.IsAny<bool>(), It.IsAny<V3DiagnosisSubmissionParameter.Key>())).Returns(true);

            var logger = new Mock.LoggerMock<V3DiagnosisApi>();
            var diagnosisApi = new V3DiagnosisApi(config.Object,
                                                tekRepo.Object,
                                                deviceCheck.Object,
                                                verification.Object,
                                                validationServer.Object,
                                                temporaryExposureKeyValidationService.Object,
                                                logger);
            var context = new Mock<HttpContext>();
            var keydata = new byte[KEY_LENGTH];
            RandomNumberGenerator.Create().GetBytes(keydata);
            var keyDataString = Convert.ToBase64String(keydata);

            var dateTime = DateTime.UtcNow.AddDays(-7).Date;

            var bodyJson = new V3DiagnosisSubmissionParameter()
            {
                SymptomOnsetDate = dateTime.ToString(Constants.FORMAT_TIMESTAMP),
                VerificationPayload = verificationPayload,
                Regions = new[] { region },
                Platform = platform,
                DeviceVerificationPayload = "DeviceVerificationPayload",
                AppPackageName = "Covid19Radar",
                Keys = new V3DiagnosisSubmissionParameter.Key[] {
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-8).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-7).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-6).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-5).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-4).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-3).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-2).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(-1).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(0).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(1).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(2).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(3).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(4).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(5).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(6).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(7).ToRollingStartNumber(), ReportType = 7 },
                    new V3DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 144, RollingStartNumber = dateTime.AddDays(8).ToRollingStartNumber(), ReportType = 7 },
                }
            };
            var bodyString = JsonConvert.SerializeObject(bodyJson);
            using var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                await writer.WriteAsync(bodyString);
                await writer.FlushAsync();
            }
            stream.Seek(0, SeekOrigin.Begin);

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
            if (result is OkObjectResult okObjectResult)
            {
                Assert.AreEqual(((int)expectedStatusCode), okObjectResult.StatusCode);

                V3DiagnosisSubmissionParameter resultParameter = JsonConvert.DeserializeObject<V3DiagnosisSubmissionParameter>(okObjectResult.Value.ToString());

                Assert.AreEqual(17, resultParameter.Keys.Count());

                Assert.AreEqual(-8, resultParameter.Keys[0].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(-7, resultParameter.Keys[1].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(-6, resultParameter.Keys[2].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(-5, resultParameter.Keys[3].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(-4, resultParameter.Keys[4].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(-3, resultParameter.Keys[5].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(-2, resultParameter.Keys[6].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(-1, resultParameter.Keys[7].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(0, resultParameter.Keys[8].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(1, resultParameter.Keys[9].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(2, resultParameter.Keys[10].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(3, resultParameter.Keys[11].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(4, resultParameter.Keys[12].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(5, resultParameter.Keys[13].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(6, resultParameter.Keys[14].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(7, resultParameter.Keys[15].DaysSinceOnsetOfSymptoms);
                Assert.AreEqual(8, resultParameter.Keys[16].DaysSinceOnsetOfSymptoms);
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}
