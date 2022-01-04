/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Services
{
    public class DeviceValidationService : IDeviceValidationService
    {
        private readonly DeviceValidationAndroidService Android;
        private readonly DeviceValidationAppleService Apple;
        private readonly IAuthorizedAppRepository AuthApp;

        public DeviceValidationService(
            IConfiguration config,
            IHttpClientFactory http,
            IAuthorizedAppRepository authApp,
            ILogger<DeviceValidationService> logger)
        {
            Android = new DeviceValidationAndroidService();
            Apple = new DeviceValidationAppleService(config, http, logger);
            AuthApp = authApp;
        }

        protected DeviceValidationService() { }

        public async Task<bool> Validation(string platform, IDeviceVerification deviceVerification, DateTimeOffset requestTime)
        {
            var app = await AuthApp.GetAsync(platform);
            // unsupported
            if (app == null) return false;
            if (!app.DeviceValidationEnabled) return true;
            return platform switch
            {
                "android" => Android.Validation(deviceVerification, requestTime, app),
                "ios" => await Apple.Validation(deviceVerification, requestTime, app),
                _ => false,
            };
        }
    }
}
