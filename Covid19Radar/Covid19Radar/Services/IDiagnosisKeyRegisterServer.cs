using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Chino;

namespace Covid19Radar.Services
{
    public interface IDiagnosisKeyRegisterServer
    {
        public Task<HttpStatusCode> SubmitDiagnosisKeysAsync(
            IList<TemporaryExposureKey> temporaryExposureKeys,
            string processNumber
            );
    }
}
