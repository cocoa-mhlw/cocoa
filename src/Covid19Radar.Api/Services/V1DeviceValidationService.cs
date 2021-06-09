/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Extensions;
using Covid19Radar.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Services
{

    public class V1DeviceValidationService : IV1DeviceValidationService
    {

        private readonly DeviceValidationAndroidService Android;
        private readonly DeviceValidationAppleService Apple;
        private readonly IAuthorizedAppRepository AuthApp;

    public V1DeviceValidationService(
            IConfiguration config,
            IHttpClientFactory http,
            IAuthorizedAppRepository authApp,
            ILogger<V1DeviceValidationService> logger)
        {
            Android = new DeviceValidationAndroidService();
            Apple = new DeviceValidationAppleService(config, http, logger);
            AuthApp = authApp;
        }

        public async Task<bool> Validation(V1DiagnosisSubmissionParameter param, DateTimeOffset requestTime)
        {
            var app = await AuthApp.GetAsync(param.Platform);
            // unsupported
            if (app == null) return false;
            if (!app.DeviceValidationEnabled) return true;
            return param.Platform switch
            {
                "android" =>  Android.Validation(param, param.GetAndroidNonce(), requestTime, app),
                "ios" => await Apple.Validation(param, requestTime, app),
                _ => false,
            };
        }
    }

}
