using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
{
    public interface ITemporaryExposureKeyExportBatchService
    {
        Task RunAsync();
    }

}
