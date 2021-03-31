/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Covid19Radar.Api.Tests
{
    [TestClass]
    [TestCategory("Api")]
    public class InquiryLogApiTest
    {
        [TestMethod]
        public void Normal_Run()
        {
            // arrange
            var logger = new Mock.LoggerMock<InquiryLogApi>();
            var inquiryLogBlobService = new Mock<IInquiryLogBlobService>();
            inquiryLogBlobService.Setup(_ => _.GetServiceSASToken()).Returns("1234567890");
            var validationServer = new Mock<IValidationServerService>();
            validationServer.Setup(_ => _.Validate(It.IsAny<HttpRequest>())).Returns(IValidationServerService.ValidateResult.Success);
            var validationInquiryLog = new Mock<IValidationInquiryLogService>();
            validationInquiryLog.Setup(_ => _.Validate(It.IsAny<HttpRequest>())).Returns(IValidationInquiryLogService.ValidateResult.Success);
            var context = new Mock<HttpContext>();

            // act
            var target = new InquiryLogApi(logger, inquiryLogBlobService.Object, validationServer.Object, validationInquiryLog.Object);
            var result = target.Run(context.Object.Request);

            // assert
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            var resultString = (result as JsonResult).Value.ToString();
            Assert.AreEqual(@"{ sas_token = 1234567890 }", resultString);
        }

        [TestMethod]
        public void Exception_Run_ServerValidationError()
        {
            // arrange
            var logger = new Mock.LoggerMock<InquiryLogApi>();
            var inquiryLogBlobService = new Mock<IInquiryLogBlobService>();
            inquiryLogBlobService.Setup(_ => _.GetServiceSASToken()).Returns("1234567890");
            var validationServer = new Mock<IValidationServerService>();
            validationServer.Setup(_ => _.Validate(It.IsAny<HttpRequest>())).Returns(IValidationServerService.ValidateResult.Error);
            var validationInquiryLog = new Mock<IValidationInquiryLogService>();
            validationInquiryLog.Setup(_ => _.Validate(It.IsAny<HttpRequest>())).Returns(IValidationInquiryLogService.ValidateResult.Success);
            var context = new Mock<HttpContext>();

            // act
            var target = new InquiryLogApi(logger, inquiryLogBlobService.Object, validationServer.Object, validationInquiryLog.Object);
            var result = target.Run(context.Object.Request);

            // assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void Exception_Run_InquiryLogValidationError()
        {
            // arrange
            var logger = new Mock.LoggerMock<InquiryLogApi>();
            var inquiryLogBlobService = new Mock<IInquiryLogBlobService>();
            inquiryLogBlobService.Setup(_ => _.GetServiceSASToken()).Returns("1234567890");
            var validationServer = new Mock<IValidationServerService>();
            validationServer.Setup(_ => _.Validate(It.IsAny<HttpRequest>())).Returns(IValidationServerService.ValidateResult.Success);
            var validationInquiryLog = new Mock<IValidationInquiryLogService>();
            validationInquiryLog.Setup(_ => _.Validate(It.IsAny<HttpRequest>())).Returns(IValidationInquiryLogService.ValidateResult.Error);
            var context = new Mock<HttpContext>();

            // act
            var target = new InquiryLogApi(logger, inquiryLogBlobService.Object, validationServer.Object, validationInquiryLog.Object);
            var result = target.Run(context.Object.Request);

            // assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }
    }
}
