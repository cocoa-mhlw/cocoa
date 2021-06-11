/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using Covid19Radar.Model;

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
        Task<string> VerifyAsync(DiagnosisSubmissionParameter submission);
    }
}
