using Covid19Radar.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Services
{
    public interface IInfectionService
    {
        DateTime LastUpdateTime { get; }

        IEnumerable<InfectionModel> GetList(DateTime lastClientTime, out DateTime lastUpdateTime);
    }
}
