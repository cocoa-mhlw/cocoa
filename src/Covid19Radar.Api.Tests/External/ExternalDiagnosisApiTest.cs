﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests.External
{
    [TestClass]
    [TestCategory("ExternalApi")]
    public class ExternalDiagnosisApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var diagnosisRepo = new Mock<IDiagnosisRepository>();
            var tekRepo = new Mock<ITemporaryExposureKeyRepository>();
            var logger = new Mock.LoggerMock<Covid19Radar.Api.External.DiagnosisApi>();
            var diagnosisApi = new Covid19Radar.Api.External.DiagnosisApi(diagnosisRepo.Object, tekRepo.Object, logger);
        }

        [TestMethod]
        public async Task RunAsyncMethod()
        {
            // preparation
            var diagnosisRepo = new Mock<IDiagnosisRepository>();
            diagnosisRepo.Setup(_ => _.SubmitDiagnosisAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<string>(), It.IsAny<TemporaryExposureKeyModel[]>()))
                .ReturnsAsync(new DiagnosisModel());
            var tekRepo = new Mock<ITemporaryExposureKeyRepository>();
            var logger = new Mock.LoggerMock<Covid19Radar.Api.External.DiagnosisApi>();
            var diagnosisApi = new Covid19Radar.Api.External.DiagnosisApi(diagnosisRepo.Object, tekRepo.Object, logger);
            var context = new Mock<HttpContext>();

            // action
            await diagnosisApi.RunAsync(context.Object.Request);
            // assert
        }

        [DataTestMethod]
        [DataRow("xxxxx", "UserUuid")]
        [DataRow("", "")]
        [DataRow(null, null)]
        public async Task RunApprovedAsyncMethod(string submissionNumber, string userUuid)
        {
            // preparation
            var diagnosisRepo = new Mock<IDiagnosisRepository>();
            var keyModel = new TemporaryExposureKeyModel()
            {
                id = "id123"
            };
            var model = new DiagnosisModel()
            {
                SubmissionNumber = submissionNumber,
                UserUuid = userUuid,
                Keys = new[] { keyModel }
            };
            diagnosisRepo.Setup(_ => _.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(model);
            var tekRepo = new Mock<ITemporaryExposureKeyRepository>();
            var logger = new Mock.LoggerMock<Covid19Radar.Api.External.DiagnosisApi>();
            var diagnosisApi = new Covid19Radar.Api.External.DiagnosisApi(diagnosisRepo.Object, tekRepo.Object, logger);
            var context = new Mock<HttpContext>();

            tekRepo.Setup(_ => _.UpsertAsync(It.IsAny<TemporaryExposureKeyModel>()))
                .Verifiable();

            // action
            await diagnosisApi.RunApprovedAsync(context.Object.Request, submissionNumber, userUuid);
            // assert
        }
    }
}
