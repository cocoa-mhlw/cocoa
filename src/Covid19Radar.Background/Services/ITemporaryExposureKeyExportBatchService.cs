using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface ITemporaryExposureKeyExportBatchService
    {
        Task RunAsync();
    }

}
