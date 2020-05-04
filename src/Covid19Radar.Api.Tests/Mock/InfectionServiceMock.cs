using Covid19Radar.Models;
using Covid19Radar.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Tests.Mock
{
    public class InfectionServiceMock : IInfectionService
    {
        public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;

        public IEnumerable<InfectionModel> GetList(DateTime lastClientTime, out DateTime lastUpdateTime)
        {
            lastUpdateTime = DateTime.MinValue;
            return Enumerable.Empty<InfectionModel>();
        }
    }
}
