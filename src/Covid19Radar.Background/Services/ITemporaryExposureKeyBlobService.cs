using Covid19Radar.Api.Models;
using Covid19Radar.Api.Protobuf;
using System.IO;
using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
{
    public interface ITemporaryExposureKeyBlobService
    {
        Task WriteToBlobAsync(Stream s, TemporaryExposureKeyExportModel model, TemporaryExposureKeyExport bin, TEKSignatureList sig);

        Task DeleteAsync(TemporaryExposureKeyExportModel model);
    }
}
