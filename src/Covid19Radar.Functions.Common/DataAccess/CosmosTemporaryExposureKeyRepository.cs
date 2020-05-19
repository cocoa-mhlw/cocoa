using Covid19Radar.DataStore;
using Covid19Radar.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.DataAccess
{
    public class CosmosTemporaryExposureKeyRepository : ITemporaryExposureKeyRepository
    {
        private readonly ICosmos _db;
        private readonly ILogger<CosmosTemporaryExposureKeyExportRepository> _logger;

        public CosmosTemporaryExposureKeyRepository(
            ICosmos db,
            ILogger<CosmosTemporaryExposureKeyExportRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<TemporaryExposureKeyModel> GetAsync(string id)
        {
            return new TemporaryExposureKeyModel();
        }

        public async Task<TemporaryExposureKeyModel[]> GetNextAsync()
        {
            var ins = new TemporaryExposureKeyModel();
            ins.KeyData = new byte[64];
            ins.RollingPeriod = 24 * 60 / 10;
            ins.RollingStartIntervalNumber = (int)(DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds() / 60 / 10);
            ins.Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ins.TransmissionRiskLevel = 0;
            ins.Region = "Country";

            return Enumerable.Repeat(ins, 40000).ToArray();

        }
    }
}
