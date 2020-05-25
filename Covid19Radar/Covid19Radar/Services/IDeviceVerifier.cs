using Covid19Radar.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    /// <summary>
    /// Verification device information required for positive submissions
    /// </summary>
    /// <returns>Device Verification Payload</returns>
    /// <remarks>
    /// see deviceVerificationPayload 
    /// https://github.com/google/exposure-notifications-server/blob/master/docs/server_functional_requirements.md
    /// </remarks>

    public interface IDeviceVerifier
    {
        Task<string> VerifyAsync(SelfDiagnosisSubmission submission);
    }
}
