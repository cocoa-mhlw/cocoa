/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Castle.Core.Configuration;
using Covid19Radar.Api;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests.Common.Services
{
    [TestClass]
    [TestCategory("Services")]
    public class ValidationServerServiceTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            var logger = new Mock<ILogger<ValidationServerService>>();
            // action
            var instance = new ValidationServerService(config.Object, logger.Object);
        }

        [DataTestMethod]
        [DataRow(true, "false", null, "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(false, "true", null, "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(true, "false", "", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(false, "true", "", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(true, "false", "1", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(false, "true", "1", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(true, "false", "0", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(false, "true", "0", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(true, "false", "-1", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(false, "true", "-1", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(true, "false", "a", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(false, "true", "a", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(true, "false", "b", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(false, "true", "b", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(true, "false", "a038d2f8-b9c9-11ea-b3de-0242ac130004", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(false, "true", "a038d2f8-b9c9-11ea-b3de-0242ac130004", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(true, "false", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        [DataRow(true, "true", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5", "3a1f9e76-f8c9-4000-b4e0-9bb08abe9fe5")]
        public void ValidateMethod(bool expected, string checkEnabled, string incomingHeaderValue, string frontDoorId)
        {
            // preparation
            var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            var logger = new Mock<ILogger<ValidationServerService>>();
            var req = new Mock<HttpRequest>();
            config.Setup(_ => _["AzureFrontDoorRestrictionEnabled"]).Returns(checkEnabled);
            config.Setup(_ => _["AzureFrontDoorId"]).Returns(frontDoorId);
            req.Setup(_ => _.Headers["X-Azure-FDID"]).Returns(incomingHeaderValue);
            req.Setup(_ => _.Headers.ContainsKey("X-Azure-FDID")).Returns(true);
            var instance = new ValidationServerService(config.Object, logger.Object);
            // action
            var result = instance.Validate(req.Object);
            // assert
            Assert.AreEqual(expected, result.IsValid);
        }

    }
}
