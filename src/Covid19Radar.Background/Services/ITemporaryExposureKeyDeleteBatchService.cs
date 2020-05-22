using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
{
    public interface ITemporaryExposureKeyDeleteBatchService
    {
        Task RunAsync();
    }
}
