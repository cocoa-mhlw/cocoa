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

        // ExposureWindow mode
        Task AppendExposureDataAsync(
            IList<DailySummary> dailySummaryList,
            IList<ExposureWindow> exposueWindowList
            );

        Task<List<DailySummary>> GetDailySummariesAsync();
        Task<List<DailySummary>> GetDailySummariesAsync(int offsetDays);
        Task RemoveDailySummariesAsync();

        Task<List<ExposureWindow>> GetExposureWindowsAsync();
        Task<List<ExposureWindow>> GetExposureWindowsAsync(int offsetDays);
        Task RemoveExposureWindowsAsync();

        // Legacy v1 mode
        Task<List<UserExposureSummary>> GetUserExposureSummariesAsync();

        Task<List<UserExposureInfo>> GetUserExposureInfosAsync();
        Task<List<UserExposureInfo>> GetUserExposureInfosAsync(int offsetDays);

        Task<bool> AppendExposureDataAsync(
            ExposureSummary exposureSummary,
            IList<ExposureInformation> exposureInformationList,
            int minimumRiskScore
            );

        Task RemoveUserExposureInformationAsync();
    }
}
