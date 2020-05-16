using Covid19Radar.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.DataAccess
{
    public interface ITemporaryExposureKeyRepository
    {

        Task<TemporaryExposureKeyModel> GetAsync(string id);
        Task<TemporaryExposureKeyModel[]> GetNextAsync();
    }
}
