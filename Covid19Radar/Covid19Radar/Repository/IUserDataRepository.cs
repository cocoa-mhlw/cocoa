using System.Collections.Generic;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Model;

namespace Covid19Radar.Repository
{
    public interface IUserDataRepository
    {
        Task<long> GetLastProcessDiagnosisKeyTimestampAsync(string region);
        Task SetLastProcessDiagnosisKeyTimestampAsync(string region, long timestamp);
        Task RemoveLastProcessDiagnosisKeyTimestampAsync();

        Task<(UserExposureSummary, IList<UserExposureInfo>)> GetUserExposureDataAsync();
        Task SetExposureDataAsync(ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformationList);
        Task RemoveExposureInformationAsync();
    }
}
