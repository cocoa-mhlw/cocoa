using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Covid19Radar.Api.Models;
using System.Linq;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using Covid19Radar.Api.DataAccess;
using System.Security.Cryptography;
using Covid19Radar.Api.Extensions;
using System.Text.RegularExpressions;

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
            IAuthorizedAppRepository authApp)
        {
            Android = new DeviceValidationAndroidService(config, http);
            Apple = new DeviceValidationAppleService(config, http);
            AuthApp = authApp;
        }

        public async Task<bool> Validation(DiagnosisSubmissionParameter param, DateTimeOffset requestTime)
        {
            var app = await AuthApp.GetAsync(param.Platform);
            return param.Platform switch
            {
                "android" => await Android.Validation(param, param.GetAndroidNonce(), requestTime, app),
                "ios" => await Apple.Validation(param, requestTime, app),
                _ => false,
            };
        }

    }
}
