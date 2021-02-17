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
    public class V2DiagnosisApiTest
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
            var deviceCheck = new Mock<IDeviceValidationService>();
            var verification = new Mock<IVerificationService>();
            var logger = new Mock.LoggerMock<Covid19Radar.Api.V2DiagnosisApi>();
            var diagnosisApi = new V2DiagnosisApi(config.Object,
                                                diagnosisRepo.Object,
                                                tekRepo.Object,
                                                deviceCheck.Object,
                                                verification.Object,
                                                validationServer.Object,
                                                logger);
        }

        [DataTestMethod]
        [DataRow(true, true, "RegionX", "xxxxx", "ios")]
        [DataRow(true, true, "RegionX", "xxxxx", "")]
        [DataRow(false, false, "Region1", "xxxxx", "ios")]
        [DataRow(true, false, "Region1", "xxxxx", "ios")]
        [DataRow(true, true, "Region1", "xxxxx", "ios")]
        public async Task RunAsyncMethod(bool isValid,
                                         bool isValidDevice,
                                         string region,
                                         string verificationPayload,
                                         string platform)
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
            deviceCheck.Setup(_ => _.Validation(It.IsAny<DiagnosisSubmissionParameter>(), It.IsAny<DateTimeOffset>())).ReturnsAsync(isValidDevice);
            var verification = new Mock<IVerificationService>();
            var logger = new Mock.LoggerMock<Covid19Radar.Api.V2DiagnosisApi>();
            var diagnosisApi = new V2DiagnosisApi(config.Object,
                                                diagnosisRepo.Object,
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
            var bodyJson = new DiagnosisSubmissionParameter()
            {
                VerificationPayload = verificationPayload,
                Regions = new[] { region },
                Platform = platform,
                DeviceVerificationPayload = "DeviceVerificationPayload",
                AppPackageName = "Covid19Radar",
                Keys = new DiagnosisSubmissionParameter.Key[] {
                    new DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 0, RollingStartNumber = startNumber },
                    new DiagnosisSubmissionParameter.Key() { KeyData = keyDataString, RollingPeriod = 0, RollingStartNumber = startNumber } }
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
