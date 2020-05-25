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
            var deviceCheck = new Mock<IDeviceValidationService>();
            var logger = new Mock.LoggerMock<Covid19Radar.Api.DiagnosisApi>();
            var diagnosisApi = new DiagnosisApi(config.Object,
                                                diagnosisRepo.Object,
                                                tekRepo.Object,
                                                validation.Object,
                                                deviceCheck.Object,
                                                logger);
        }

        [DataTestMethod]
        [DataRow(true, true, "xxxxx", "UserUuid")]
        [DataRow(false, true, "xxxxx", "UserUuid")]
        [DataRow(false, true, "xxxxx", "UserUuid")]
        public async Task RunAsyncMethod(bool isValid, bool isValidDevice, string submissionNumber, string userUuid)
        {
            // preparation
            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["SupportRegions"]).Returns("Region1,Region2");
            var diagnosisRepo = new Mock<IDiagnosisRepository>();
            diagnosisRepo.Setup(_ => _.SubmitDiagnosisAsync(It.IsAny<string>(),
                                                            It.IsAny<DateTimeOffset>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<TemporaryExposureKeyModel[]>()))
                .Returns(Task.CompletedTask);
            var tekRepo = new Mock<ITemporaryExposureKeyRepository>();
            var validation = new Mock<IValidationUserService>();
            var validationResult = new IValidationUserService.ValidateResult()
            {
                IsValid = isValid
            };
            validation.Setup(_ => _.ValidateAsync(It.IsAny<HttpRequest>(), It.IsAny<IUser>())).ReturnsAsync(validationResult);
            var deviceCheck = new Mock<IDeviceValidationService>();
            deviceCheck.Setup(_ => _.Validation(It.IsAny<DiagnosisSubmissionParameter>())).ReturnsAsync(isValidDevice);
            var logger = new Mock.LoggerMock<Covid19Radar.Api.DiagnosisApi>();
            var diagnosisApi = new DiagnosisApi(config.Object,
                                                diagnosisRepo.Object,
                                                tekRepo.Object,
                                                validation.Object,
                                                deviceCheck.Object,
                                                logger);
            var context = new Mock<HttpContext>();
            var bodyJson = new DiagnosisSubmissionParameter()
            {
                SubmissionNumber = submissionNumber,
                UserUuid = userUuid,
                Keys = new DiagnosisSubmissionParameter.Key[] {
                    new DiagnosisSubmissionParameter.Key() { KeyData = "", RollingPeriod = 1, RollingStartNumber = 1 } }
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
