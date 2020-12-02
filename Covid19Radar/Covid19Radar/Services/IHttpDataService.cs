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
        Task<UserDataModel> PostRegisterUserAsync();

        Task<HttpStatusCode> PutSelfExposureKeysAsync(DiagnosisSubmissionParameter request);

        Task<List<TemporaryExposureKeyExportFileModel>> GetTemporaryExposureKeyList(string region, CancellationToken cancellationToken);

        Task<Stream> GetTemporaryExposureKey(string url, CancellationToken cancellationToken);

        Task<ApiResponse<LogStorageSas>> GetLogStorageSas();
    }
}
