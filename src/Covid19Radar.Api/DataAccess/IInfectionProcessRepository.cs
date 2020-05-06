using Covid19Radar.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.DataAccess
{
    [Obsolete]
    public interface IInfectionProcessRepository
    {
        Task Upsert(InfectionProcessModel infectionProcess);
    }
}
