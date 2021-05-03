﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests
{
    [TestClass]
    [TestCategory("Api")]
    public class DiagnosisApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["SupportRegions"]).Returns("Region1,Region2");
            var diagnosisRepo = new Mock<IDiagnosisRepository>();
            var tekRepo = new Mock<ITemporaryExposureKeyRepository>();
            var validation = new Mock<IValidationUserService>();
            var validationServer = new Mock<IValidationServerService>();
            var deviceCheck = new Mock<IV1DeviceValidationService>();
            var verification = new Mock<IVerificationService>();
            var logger = new Mock.LoggerMock<Covid19Radar.Api.DiagnosisApi>();
            var diagnosisApi = new DiagnosisApi(config.Object,
                                                diagnosisRepo.Object,
                                                tekRepo.Object,
                                                validation.Object,
                                                deviceCheck.Object,
                                                verification.Object,
                                                validationServer.Object,
                                                logger);
        }

        [DataTestMethod]
        [DataRow(true, true, "RegionX", "xxxxx", "ios", "UserUuid")]
        [DataRow(true, true, "RegionX", "xxxxx", "", "UserUuid")]
        [DataRow(false, false, "Region1", "xxxxx", "ios", "UserUuid")]
        [DataRow(true, false, "Region1", "xxxxx", "ios", "UserUuid")]
        [DataRow(true, true, "Region1", "xxxxx", "ios", "UserUuid")]
        public async Task RunAsyncMethod(bool isValid,
                                         bool isValidDevice,
                                         string region,
                                         string verificationPayload,
                                         string platform,
                                         string userUuid)
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
            var validation = new Mock<IValidationUserService>();
            var validationServer = new Mock<IValidationServerService>();
            validationServer.Setup(_ => _.Validate(It.IsAny<HttpRequest>())).Returns(IValidationServerService.ValidateResult.Success);

            var validationResult = new IValidationUserService.ValidateResult()
            {
                IsValid = isValid
            };
            validation.Setup(_ => _.ValidateAsync(It.IsAny<HttpRequest>(), It.IsAny<IUser>())).ReturnsAsync(validationResult);
            var deviceCheck = new Mock<IV1DeviceValidationService>();
            deviceCheck.Setup(_ => _.Validation(It.IsAny<V1DiagnosisSubmissionParameter>(), It.IsAny<DateTimeOffset>())).ReturnsAsync(isValidDevice);
            var verification = new Mock<IVerificationService>();
            var logger = new Mock.LoggerMock<Covid19Radar.Api.DiagnosisApi>();
            var diagnosisApi = new DiagnosisApi(config.Object,
                                                diagnosisRepo.Object,
                                                tekRepo.Object,
                                                validation.Object,
                                                deviceCheck.Object,
                                                verification.Object,
                                                validationServer.Object,
                                                logger);
            var context = new Mock<HttpContext>();
            var keydata = new byte[16];
            RandomNumberGenerator.Create().GetBytes(keydata);
            var keyDataString = Convert.ToBase64String(keydata);
            var startNumber = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 600;
            var bodyJson = new V1DiagnosisSubmissionParameter()
            {
                VerificationPayload = verificationPayload,
                Regions = new[] { region },
                UserUuid = userUuid,
                Platform = platform,
                DeviceVerificationPayload = "DeviceVerificationPayload",
                AppPackageName = "Covid19Radar",
                Keys = new V1DiagnosisSubmissionParameter.Key[] {
                    new V1DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 0, RollingStartNumber = startNumber },
                    new V1DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 0, RollingStartNumber = startNumber } }
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
            // action
            await diagnosisApi.RunAsync(context.Object.Request);
            // assert
        }
    }
}
