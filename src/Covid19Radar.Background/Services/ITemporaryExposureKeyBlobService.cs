using Covid19Radar.Models;
using Covid19Radar.Protobuf;
using System.IO;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface ITemporaryExposureKeyBlobService
    {
        Task WriteToBlobAsync(Stream s, TemporaryExposureKeyExportModel model, TemporaryExposureKeyExport bin, TEKSignatureList sig);

        Task DeleteAsync(TemporaryExposureKeyExportModel model);
    }
}
