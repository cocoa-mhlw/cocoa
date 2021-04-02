/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    class HttpDataServiceMock : IHttpDataService
    {
        public Task MigrateFromUserData(UserDataModel userData)
        {
            return Task.CompletedTask;
        }

        public (string, string) GetCredentials()
        {
            return ("user-uuid", "secret");
        }

        public void RemoveCredentials()
        {
        }

        Task<Stream> IHttpDataService.GetTemporaryExposureKey(string url, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew<Stream>(() =>
            {
                Debug.WriteLine("HttpDataServiceMock::GetTemporaryExposureKey called");
                return new MemoryStream();
            });
        }

        Task<List<TemporaryExposureKeyExportFileModel>> IHttpDataService.GetTemporaryExposureKeyList(string region, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew<List<TemporaryExposureKeyExportFileModel>>(() =>
            {
                Debug.WriteLine("HttpDataServiceMock::GetTemporaryExposureKeyList called");
                return new List<TemporaryExposureKeyExportFileModel>();
            });
        }

        async Task<bool> IHttpDataService.PostRegisterUserAsync()
        {
            Debug.WriteLine("HttpDataServiceMock::PostRegisterUserAsync called");
            return await Task.FromResult(true);
        }

        Task<HttpStatusCode> IHttpDataService.PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request)
        {
            return Task.Factory.StartNew<HttpStatusCode>(() =>
            {
                Debug.WriteLine("HttpDataServiceMock::PutSelfExposureKeysAsync called");
                return HttpStatusCode.OK;
            });
        }

        public Task<ApiResponse<LogStorageSas>> GetLogStorageSas()
        {
            return Task.Factory.StartNew(() =>
            {
                Debug.WriteLine("HttpDataServiceMock::GetStorageKey called");
                return new ApiResponse<LogStorageSas>((int)HttpStatusCode.OK, new LogStorageSas { SasToken = "sv=2012-02-12&se=2015-07-08T00%3A12%3A08Z&sr=c&sp=wl&sig=t%2BbzU9%2B7ry4okULN9S0wst%2F8MCUhTjrHyV9rDNLSe8g%3Dsss" });
            });
        }
    }
}
