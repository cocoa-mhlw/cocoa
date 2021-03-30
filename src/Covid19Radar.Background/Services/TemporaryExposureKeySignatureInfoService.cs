/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using Covid19Radar.Background.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Covid19Radar.Background.Services
{
    public class TemporaryExposureKeySignatureInfoService : ITemporaryExposureKeySignatureInfoService
    {
        public readonly string AppBundleId;
        public readonly string AndroidPackage;
        public readonly string Algorithm = "1.2.840.10045.4.3.2";
        public readonly SignatureInfo Info;
        public readonly ILogger<TemporaryExposureKeySignatureInfoService> Logger;

        public TemporaryExposureKeySignatureInfoService(
            IConfiguration config,
            ILogger<TemporaryExposureKeySignatureInfoService> logger)
        {
            Logger = logger;
            Logger.LogInformation($"{nameof(TemporaryExposureKeySignatureInfoService)} constructor");
            AppBundleId = config.iOSBundleId();
            AndroidPackage = config.AndroidPackageName();
            Info = new SignatureInfo();
            Info.AppBundleId = AppBundleId;
            Info.AndroidPackage = AndroidPackage;
            Info.SignatureAlgorithm = Algorithm;
        }

        public SignatureInfo Create()
        {
            Logger.LogInformation($"start {nameof(Create)}");
            var si = new SignatureInfo(Info);
            return si;
        }

    }
}
