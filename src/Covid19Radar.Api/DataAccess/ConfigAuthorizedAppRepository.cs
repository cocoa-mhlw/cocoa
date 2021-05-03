﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public class ConfigAuthorizedAppRepository : IAuthorizedAppRepository
    {
        public readonly AuthorizedAppInformation Android;
        public readonly AuthorizedAppInformation iOS;
        public ConfigAuthorizedAppRepository(IConfiguration config)
        {
            Android = new AuthorizedAppInformation();
            Android.Platform = "android";
            Android.AllowedRegions = config.SupportRegions();
            Android.PackageName = config.AndroidPackageName();
            Android.DeviceValidationEnabled = config.AndroidDeviceValidationEnabled();
            iOS = new AuthorizedAppInformation();
            iOS.Platform = "ios";
            iOS.AllowedRegions = config.SupportRegions();
            iOS.PackageName = config.iOSBundleId();
            iOS.DeviceCheckKeyId = config.iOSDeviceCheckKeyId();
            iOS.DeviceCheckTeamId = config.iOSDeviceCheckTeamId();
            iOS.DeviceCheckPrivateKey = config.iOSDeviceCheckPrivateKey();
            iOS.DeviceValidationEnabled = config.iOSDeviceValidationEnabled();
        }

        public Task<AuthorizedAppInformation> GetAsync(string platform)
        {
            return platform switch
            {
                "android" => Task.FromResult(Android),
                "ios" => Task.FromResult(iOS),
                _ => Task.FromResult<AuthorizedAppInformation>(null),
            };
        }
    }
}
