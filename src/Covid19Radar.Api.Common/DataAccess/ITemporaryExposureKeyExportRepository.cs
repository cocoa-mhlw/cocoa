using Covid19Radar.Api.Models;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public interface ITemporaryExposureKeyExportRepository
    {
        Task<TemporaryExposureKeyExportModel> CreateAsync(TemporaryExposureKeyExportModel model);
        Task<TemporaryExposureKeyExportModel> GetAsync(string id);
        Task UpdateAsync(TemporaryExposureKeyExportModel model);
        Task<TemporaryExposureKeyExportModel[]> GetKeysAsync(ulong sinceEpochSeconds);
        Task<TemporaryExposureKeyExportModel[]> GetKeysAsync(ulong sinceEpochSeconds, string region);
        Task<TemporaryExposureKeyExportModel[]> GetOutOfTimeKeysAsync();
    }
}
