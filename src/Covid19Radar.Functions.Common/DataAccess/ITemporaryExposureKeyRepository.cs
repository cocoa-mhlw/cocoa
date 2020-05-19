using Covid19Radar.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.DataAccess
{
    public interface ITemporaryExposureKeyRepository
    {

        Task UpsertAsync(TemporaryExposureKeyModel model);
        Task<TemporaryExposureKeyModel> GetAsync(string id);
        Task<TemporaryExposureKeyModel[]> GetNextAsync();
        Task<TemporaryExposureKeyModel[]> GetOutOfTimeKeysAsync();
        Task DeleteAsync(TemporaryExposureKeyModel model);
    }
}
