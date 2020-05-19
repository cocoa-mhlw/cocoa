using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface ITemporaryExposureKeyDeleteBatchService
    {
        Task RunAsync();
    }
}
