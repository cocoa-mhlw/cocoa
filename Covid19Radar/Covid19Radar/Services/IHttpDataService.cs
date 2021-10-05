/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Model;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface IHttpDataService
    {
        Task<bool> PostRegisterUserAsync();

        Task<IList<HttpStatusCode>> PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request);

        Task<ApiResponse<LogStorageSas>> GetLogStorageSas();
    }
}
