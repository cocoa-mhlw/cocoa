/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface IHttpDataService
    {
        Task<bool> PostRegisterUserAsync();

        Task<HttpStatusCode> PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request);

        Task<List<TemporaryExposureKeyExportFileModel>> GetTemporaryExposureKeyList(string region, CancellationToken cancellationToken);

        Task<Stream> GetTemporaryExposureKey(string url, CancellationToken cancellationToken);

        Task<ApiResponse<LogStorageSas>> GetLogStorageSas();
    }
}
