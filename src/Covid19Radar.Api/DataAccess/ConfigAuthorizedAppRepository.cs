using Covid19Radar.Api.Models;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
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
