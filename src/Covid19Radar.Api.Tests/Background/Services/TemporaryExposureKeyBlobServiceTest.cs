/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using Covid19Radar.Background.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests.Background.Services
{
    [TestClass]
    [TestCategory("Services")]
    public class TemporaryExposureKeyBlobServiceTest
    {

        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["TekExportKeyUrl"]).Returns("http://localhost/");
            config.Setup(_ => _["TekExportBlobStorage"]).Returns("UseDevelopmentStorage=true");
            config.Setup(_ => _["TekExportBlobStorageContainerPrefix"]).Returns("c19r");
            var logger = new Mock.LoggerMock<TemporaryExposureKeyBlobService>();
            // action
            var service = new TemporaryExposureKeyBlobService(config.Object, logger);
        }

    }
}
