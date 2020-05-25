using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface IDeviceCheckService
    {
        /// <summary>
        /// Verification device information required for positive submissions
        /// </summary>
        /// <returns>Device Verification Payload</returns>
        /// <remarks>
        /// see deviceVerificationPayload 
        /// https://github.com/google/exposure-notifications-server/blob/master/docs/server_functional_requirements.md
        /// </remarks>
        Task<string> GetDeviceVerificationPayload();
    }
}
