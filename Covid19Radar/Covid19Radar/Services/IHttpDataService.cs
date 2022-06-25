/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Model;
using System.Net;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface IHttpDataService
    {
        Task<HttpStatusCode> PostRegisterUserAsync();

        Task<HttpStatusCode> PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request);

        Task<ApiResponse<LogStorageSas>> GetLogStorageSas();

        Task<ApiResponse<string>> PutEventLog(V1EventLogRequest request);
    }
}
