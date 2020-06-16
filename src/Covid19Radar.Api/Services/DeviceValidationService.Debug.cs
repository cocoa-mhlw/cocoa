using Covid19Radar.Api.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Services
{
    public class DeviceValidationDebugService : IDeviceValidationService
    {
        public DeviceValidationDebugService(
            IConfiguration config,
            IHttpClientFactory client)
        {
        }

        public Task<bool> Validation(DiagnosisSubmissionParameter param, DateTimeOffset requestTime)
        {
            return Task.FromResult(true);
        }

    }
}
