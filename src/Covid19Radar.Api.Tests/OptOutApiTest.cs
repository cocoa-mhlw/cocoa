﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api;
using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
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
    public class OptOutApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var userRepo = new Mock<IUserRepository>();
            var diagnosisRepo = new Mock<IDiagnosisRepository>();
            var validation = new Mock<IValidationUserService>();
            var validationServer = new Mock<IValidationServerService>();
            var logger = new Mock.LoggerMock<OptOutApi>();
            var optOutApi = new OptOutApi(userRepo.Object, diagnosisRepo.Object, validation.Object, validationServer.Object, logger);
        }

        [DataTestMethod]
        [DataRow("UserUuid", true)]
        [DataRow("UserUuid", false)]
        public async Task RunAsyncMethod(string userUuid, bool isValid)
        {
            // preparation
            var userRepo = new Mock<IUserRepository>();
            var diagnosisRepo = new Mock<IDiagnosisRepository>();
            var validation = new Mock<IValidationUserService>();

            var validationServer = new Mock<IValidationServerService>();
            validationServer.Setup(_ => _.Validate(It.IsAny<HttpRequest>())).Returns(IValidationServerService.ValidateResult.Success);

            var validationResult = new IValidationUserService.ValidateResult()
            {
                IsValid = isValid
            };
            validation.Setup(_ => _.ValidateAsync(It.IsAny<HttpRequest>(), It.IsAny<IUser>())).ReturnsAsync(validationResult);
            var logger = new Mock.LoggerMock<OptOutApi>();
            var optOutApi = new OptOutApi(userRepo.Object, diagnosisRepo.Object, validation.Object, validationServer.Object, logger);
            var context = new Mock<HttpContext>();
            // action
            await optOutApi.RunAsync(context.Object.Request, userUuid);
            // assert
        }
    }
}
