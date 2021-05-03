﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests.External
{
    [TestClass]
    [TestCategory("Startup")]
    public class ExternalStartupTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var startup = new Covid19Radar.Api.External.Startup();
        }

        [TestMethod]
        public void ConfigureMethod()
        {
            // preparation
            var startup = new Covid19Radar.Api.External.Startup();
            var builder = new Mock<IFunctionsHostBuilder>();
            var services = new Mock<Microsoft.Extensions.DependencyInjection.IServiceCollection>();
            var serviceDescriptors = new Mock<IEnumerator<ServiceDescriptor>>();
            services.Setup(_ => _.Add(It.IsAny<ServiceDescriptor>()));
            services.Setup(_ => _.GetEnumerator()).Returns(serviceDescriptors.Object);
            builder.Setup(_ => _.Services)
                .Returns(services.Object);

            // action
            startup.Configure(builder.Object);
            // assert
        }
    }
}
