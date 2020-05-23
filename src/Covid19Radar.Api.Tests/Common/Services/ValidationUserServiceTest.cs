using Castle.Core.Configuration;
using Covid19Radar.Api;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests.Common.Services
{
    [TestClass]
    [TestCategory("Services")]
    public class ValidationUserServiceTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var cosmos = new Mock<ICosmos>();
            var cryption = new Mock<ICryptionService>();
            var logger = new Mock<ILogger<ValidationUserService>>();
            // action
            var instance = new ValidationUserService(cosmos.Object, cryption.Object, logger.Object);
        }

        [DataTestMethod]
        [DataRow("", "", true, false)]
        [DataRow("XXXX", "", false, false)]
        [DataRow("XXXX", "", true, false)]
        [DataRow("XXXX", null, true, false)]
        [DataRow("XXXX", "Pearer Value Invalid", true, false)]
        [DataRow("XXXX", "Pearer Value", true, false)]
        [DataRow("XXXX", "Bearer Value", true, true)]
        public async Task ValidateAsyncMethod(string userUuid, string authorization, bool tryGetValueResult, bool expected)
        {
            // preparation
            var cosmos = new Mock<ICosmos>();
            var cryption = new Mock<ICryptionService>();
            cryption.Setup(_ => _.ValidateSecret(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            var logger = new Mock<ILogger<ValidationUserService>>();
            var instance = new ValidationUserService(cosmos.Object, cryption.Object, logger.Object);
            var request = new Mock<HttpRequest>();
            var outValue = new StringValues(authorization);
            if (authorization == null) outValue = new StringValues();
            request.Setup(_ => _.Headers.TryGetValue(It.IsAny<string>(), out outValue))
                .Returns(tryGetValueResult);
            var user = new Mock<IUser>();
            user.Setup(_ => _.UserUuid)
                .Returns(userUuid);
            // action
            var actual = await instance.ValidateAsync(request.Object, user.Object);
            // assert
            Assert.AreEqual(expected, actual.IsValid);
            if (!expected)
            {
                Assert.AreEqual(IValidationUserService.ValidateResult.Error, actual);
            }
        }

        [DataTestMethod]
        [DataRow(false, HttpStatusCode.NotFound, "")]
        [DataRow(false, HttpStatusCode.OK, "ERROR")]
        [DataRow(true, HttpStatusCode.OK, "Value")]
        public async Task ValidateAsyncMethodIncludeQuery(bool expected, HttpStatusCode statusCode, string authorizationCode)
        {
            // preparation
            var auth = "Value";
            var isMatchAuth = auth == authorizationCode;
            var cosmos = new Mock<ICosmos>();
            var model = new UserModel();
            var response = new Mock<ItemResponse<UserModel>>();
            response.Setup(_ => _.StatusCode)
                .Returns(statusCode);
            response.Setup(_ => _.Resource)
                .Returns(model);
            cosmos.Setup(_ => _.User.ReadItemAsync<UserModel>(It.IsAny<string>(),
                                                              It.IsAny<PartitionKey>(),
                                                              It.IsAny<ItemRequestOptions>(),
                                                              It.IsAny<CancellationToken>()))
                .ReturnsAsync(response.Object);
            var cryption = new Mock<ICryptionService>();
            cryption.Setup(_ => _.ValidateSecret(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);
            cryption.Setup(_ => _.Unprotect(It.IsAny<string>()))
                .Returns(authorizationCode);
            var logger = new Mock<ILogger<ValidationUserService>>();
            var instance = new ValidationUserService(cosmos.Object, cryption.Object, logger.Object);
            var request = new Mock<HttpRequest>();
            var outValue = new StringValues($"Bearer {auth}");
            request.Setup(_ => _.Headers.TryGetValue(It.IsAny<string>(), out outValue))
                .Returns(true);
            var user = new Mock<IUser>();
            user.Setup(_ => _.UserUuid)
                .Returns("XXXX");
            // action
            var actual = await instance.ValidateAsync(request.Object, user.Object);
            // assert
            Assert.AreEqual(expected, actual.IsValid);
            if (isMatchAuth)
            {
                Assert.AreEqual(model, actual.User);
                Assert.IsNull(actual.ErrorActionResult);
            }
            else
            {
                Assert.IsNull(actual.User);
                Assert.IsInstanceOfType(actual.ErrorActionResult, typeof(BadRequestResult));
            }
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.TooManyRequests, 503, typeof(StatusCodeResult))]
        [DataRow(HttpStatusCode.NotFound, 404, typeof(StatusCodeResult))]
        public async Task ValidateAsyncMethodIncludeQueryError(HttpStatusCode statusCode, int resultStatusCode, System.Type resultType)
        {
            // preparation
            var cosmos = new Mock<ICosmos>();
            var exception = new CosmosException("", statusCode, 0, "", 1.0);
            cosmos.Setup(_ => _.User.ReadItemAsync<UserModel>(It.IsAny<string>(),
                                                              It.IsAny<PartitionKey>(),
                                                              It.IsAny<ItemRequestOptions>(),
                                                              It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            var cryption = new Mock<ICryptionService>();
            cryption.Setup(_ => _.ValidateSecret(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);
            var logger = new Mock<ILogger<ValidationUserService>>();
            var instance = new ValidationUserService(cosmos.Object, cryption.Object, logger.Object);
            var request = new Mock<HttpRequest>();
            var outValue = new StringValues($"Bearer Value");
            request.Setup(_ => _.Headers.TryGetValue(It.IsAny<string>(), out outValue))
                .Returns(true);
            var user = new Mock<IUser>();
            user.Setup(_ => _.UserUuid)
                .Returns("XXXX");
            // action
            var actual = await instance.ValidateAsync(request.Object, user.Object);
            // assert
            Assert.AreEqual(false, actual.IsValid);
            Assert.IsNull(actual.User);
            Assert.IsInstanceOfType(actual.ErrorActionResult, resultType);
            Assert.AreEqual(resultStatusCode, ((StatusCodeResult)actual.ErrorActionResult).StatusCode);
        }

    }
}
