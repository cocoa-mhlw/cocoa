using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public interface ISequenceRepository
    {
        Task<ulong> GetNextAsync(string key, ulong startNo, int increment = 1);
    }
}
