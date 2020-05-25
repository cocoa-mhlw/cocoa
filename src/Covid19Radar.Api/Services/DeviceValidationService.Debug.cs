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

namespace Covid19Radar.Api.Services
{
    public class DeviceValidationDebugService : IDeviceValidationService
    {
        public DeviceValidationDebugService(
            IConfiguration config,
            IHttpClientFactory client)
        {
        }

        public Task<bool> Validation(DiagnosisSubmissionParameter param)
        {
            return Task.FromResult(true);
        }

    }
}
