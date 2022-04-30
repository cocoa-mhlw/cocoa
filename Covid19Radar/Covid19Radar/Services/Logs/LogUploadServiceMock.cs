// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Net;
using System.Threading.Tasks;
using Covid19Radar.Model;

namespace Covid19Radar.Services.Logs
{
    public class LogUploadServiceMock : ILogUploadService
    {
        public Task<ApiResponse<LogStorageSas>> GetLogStorageSas()
            => Task.FromResult(new ApiResponse<LogStorageSas>(
                (int)HttpStatusCode.OK,
                new LogStorageSas()
                {
                    SasToken = "Dummy Token"
                }
                ));

        public Task<HttpStatusCode> UploadAsync(string zipFilePath, string sasToken)
            => Task.FromResult(HttpStatusCode.Created);
    }
}
