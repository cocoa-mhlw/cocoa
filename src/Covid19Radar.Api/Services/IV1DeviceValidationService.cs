using Covid19Radar.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Services
{
    public interface IV1DeviceValidationService
    {
        Task<bool> Validation(V1DiagnosisSubmissionParameter param, DateTimeOffset requestTime);
    }
}
