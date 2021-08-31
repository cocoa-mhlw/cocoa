using System.Threading.Tasks;

namespace Covid19Radar.Repository
{
    public interface IUserDataRepository
    {
        Task<long> GetLastProcessDiagnosisKeyTimestampAsync(string region);
        Task SetLastProcessDiagnosisKeyTimestampAsync(string region, long timestamp);
        Task RemoveLastProcessDiagnosisKeyTimestampAsync();
    }
}
