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

        Task<(IList<DailySummary>, IList<ExposureWindow>)> GetExposureWindowDataAsync();
        Task<(IList<DailySummary>, IList<ExposureWindow>)> GetExposureWindowDataAsync(int offsetDays);

        Task<(IList<UserExposureSummary>, IList<UserExposureInfo>)> GetUserExposureDataAsync();
        Task<(IList<UserExposureSummary>, IList<UserExposureInfo>)> GetUserExposureDataAsync(int offsetDays);

        Task<bool> AppendExposureDataAsync(
            ExposureSummary exposureSummary,
            IList<ExposureInformation> exposureInformationList,
            int minimumRiskScore
            );

        Task AppendExposureDataAsync(
            IList<DailySummary> dailySummaryList,
            IList<ExposureWindow> exposueWindowList
            );

        Task RemoveExposureInformationAsync();
    }
}
