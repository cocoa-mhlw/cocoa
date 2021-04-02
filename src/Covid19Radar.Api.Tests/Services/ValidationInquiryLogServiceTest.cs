/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Covid19Radar.Api.Tests.Services
{
    [TestClass]
    [TestCategory("Services")]
    public class ValidationInquiryLogServiceTest
    {
        [TestMethod]
        public void Normal_Validate()
        {
            // arrange
            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["InquiryLogApiKey"]).Returns("1234567890");

            var logger = new Mock.LoggerMock<ValidationInquiryLogService>();

            var context = new Mock<HttpContext>();
            context.Setup(_ => _.Request.Headers["x-api-key"]).Returns("1234567890");
            context.Setup(_ => _.Request.Headers.ContainsKey("x-api-key")).Returns(true);

            // act
            var target = new ValidationInquiryLogService(logger, config.Object);
            var result = target.Validate(context.Object.Request);

            // assert
            Assert.AreEqual(IValidationInquiryLogService.ValidateResult.Success, result);
            Assert.IsTrue(result.IsValid);
            Assert.IsNull(result.ErrorActionResult);
        }

        [TestMethod]
        public void Normal_Validate_NoHeader()
        {
            // arrange
            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["InquiryLogApiKey"]).Returns("1234567890");

            var logger = new Mock.LoggerMock<ValidationInquiryLogService>();

            var context = new Mock<HttpContext>();
            context.Setup(_ => _.Request.Headers.ContainsKey("x-api-key")).Returns(false);

            // act
            var target = new ValidationInquiryLogService(logger, config.Object);
            var result = target.Validate(context.Object.Request);

            // assert
            Assert.AreEqual(IValidationInquiryLogService.ValidateResult.Error, result);
            Assert.IsFalse(result.IsValid);
            Assert.IsInstanceOfType(result.ErrorActionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public void Nominal_Validate_differentApiKey()
        {
            // arrange
            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["InquiryLogApiKey"]).Returns("123456789");

            var logger = new Mock.LoggerMock<ValidationInquiryLogService>();

            var context = new Mock<HttpContext>();
            context.Setup(_ => _.Request.Headers["x-api-key"]).Returns("11111111111");
            context.Setup(_ => _.Request.Headers.ContainsKey("x-api-key")).Returns(true);

            // act
            var target = new ValidationInquiryLogService(logger, config.Object);
            var result = target.Validate(context.Object.Request);

            // assert
            Assert.AreEqual(IValidationInquiryLogService.ValidateResult.Error, result);
            Assert.IsFalse(result.IsValid);
            Assert.IsInstanceOfType(result.ErrorActionResult, typeof(BadRequestResult));
        }
    }
}
