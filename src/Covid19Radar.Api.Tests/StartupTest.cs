﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace Covid19Radar.Api.Tests
{
    [TestClass]
    [TestCategory("Startup")]
    public class StartupTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var startup = new Covid19Radar.Api.Startup();
        }

        [TestMethod]
        public void ConfigureMethod()
        {
            // preparation
            var startup = new Covid19Radar.Api.Startup();
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
