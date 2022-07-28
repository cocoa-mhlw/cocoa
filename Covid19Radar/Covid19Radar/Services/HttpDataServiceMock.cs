/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Model;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public class HttpDataServiceMock : IHttpDataService
    {
        private readonly ILoggerService _loggerSerivce;

        public HttpDataServiceMock(ILoggerService loggerService)
        {
            _loggerSerivce = loggerService;
        }

        public Task<HttpStatusCode> PostRegisterUserAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return HttpStatusCode.OK;
            });
        }

        public Task<HttpStatusCode> PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request)
        {
            return Task.Factory.StartNew(() =>
            {
                _loggerSerivce.StartMethod();
                HttpStatusCode result;
                try
                {
                    result = request.VerificationPayload switch
                    {
                        "99999910" => HttpStatusCode.NoContent,
                        "99999920" => HttpStatusCode.NotAcceptable,
                        _ => HttpStatusCode.ServiceUnavailable
                    };
                }
                finally
                {
                    _loggerSerivce.EndMethod();
                }
                return result;
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

        public Task<ApiResponse<string>> PutEventLog(V1EventLogRequest request)
        {
            return Task.Factory.StartNew(() =>
            {
                return new ApiResponse<string>((int)HttpStatusCode.Created, "");
            });
        }
    }
}
