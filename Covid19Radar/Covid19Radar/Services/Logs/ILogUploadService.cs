using System.Threading.Tasks;

namespace Covid19Radar.Services.Logs
{
    public interface ILogUploadService
    {
        Task<bool> UploadAsync(string zipFileName);
    }
}
