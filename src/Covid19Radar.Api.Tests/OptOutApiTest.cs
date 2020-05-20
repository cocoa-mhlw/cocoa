using Covid19Radar.Api;
using Covid19Radar.DataAccess;
using Covid19Radar.Models;
using Covid19Radar.Services;
using Microsoft.AspNetCore.Http;
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
    public class OptOutApiTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var userRepo = new Mock<IUserRepository>();
            var diagnosisRepo = new Mock<IDiagnosisRepository>();
            var validation = new Mock<IValidationUserService>();
            var logger = new Mock.LoggerMock<OptOutApi>();
            var optOutApi = new OptOutApi(userRepo.Object, diagnosisRepo.Object, validation.Object, logger);
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
            var validationResult = new IValidationUserService.ValidateResult()
            {
                IsValid = isValid
            };
            validation.Setup(_ => _.ValidateAsync(It.IsAny<HttpRequest>(), It.IsAny<IUser>())).ReturnsAsync(validationResult);
            var logger = new Mock.LoggerMock<OptOutApi>();
            var optOutApi = new OptOutApi(userRepo.Object, diagnosisRepo.Object, validation.Object, logger);
            var context = new Mock.HttpContextMock();
            // action
            await optOutApi.RunAsync(context.Request, userUuid);
            // assert
        }
    }
}
