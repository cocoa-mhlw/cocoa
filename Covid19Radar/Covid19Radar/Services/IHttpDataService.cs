using Covid19Radar.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface IHttpDataService
    {
        Task<UserDataModel> PostRegisterUserAsync();

        Task PutSelfExposureKeysAsync(SelfDiagnosisSubmission request);

        Task<List<TemporaryExposureKeyExportFileModel>> GetTemporaryExposureKeyList(string region, CancellationToken cancellationToken);

        Task<Stream> GetTemporaryExposureKey(string url, CancellationToken cancellationToken);
    }
}
