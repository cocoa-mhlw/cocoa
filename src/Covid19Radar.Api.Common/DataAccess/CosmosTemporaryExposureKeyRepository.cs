using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public class CosmosTemporaryExposureKeyRepository : ITemporaryExposureKeyRepository
    {
        const int OutOfDate = -14;
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
            _logger.LogInformation($"start {nameof(GetAsync)}");
            return new TemporaryExposureKeyModel();
        }

        public async Task<TemporaryExposureKeyModel[]> GetNextAsync()
        {
            _logger.LogInformation($"start {nameof(GetNextAsync)}");
            // TODO: implement query
            var ins = new TemporaryExposureKeyModel();
            ins.KeyData = new byte[64];
            ins.RollingPeriod = 24 * 60 / 10;
            ins.RollingStartIntervalNumber = (int)(DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds() / 60 / 10);
            ins.Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ins.TransmissionRiskLevel = 0;
            ins.Region = "Country";

            return Enumerable.Repeat(ins, 40000).ToArray();

        }
        public async Task<TemporaryExposureKeyModel[]> GetOutOfTimeKeysAsync()
        {
            _logger.LogInformation($"start {nameof(GetOutOfTimeKeysAsync)}");
            var oldest = DateTimeOffset.UtcNow.AddDays(OutOfDate).ToUnixTimeSeconds();
            var query = _db.TemporaryExposureKey.GetItemLinqQueryable<TemporaryExposureKeyModel>(true)
                .Where(tek => tek.RollingStartUnixTimeSeconds < oldest)
                .ToFeedIterator();

            var e = Enumerable.Empty<TemporaryExposureKeyModel>();
            while (query.HasMoreResults)
            {
                var r = await query.ReadNextAsync();
                e = e.Concat(r.Resource);
            }

            return e.ToArray();

        }

        public async Task DeleteAsync(TemporaryExposureKeyModel model)
        {
            _logger.LogInformation($"start {nameof(DeleteAsync)}");
            var pk = new PartitionKey(model.PartitionKey);
            await _db.TemporaryExposureKey.DeleteItemAsync<TemporaryExposureKeyModel>(model.id, pk);
        }

        public async Task UpsertAsync(TemporaryExposureKeyModel model)
        {
            _logger.LogInformation($"start {nameof(UpsertAsync)}");
            var pk = new PartitionKey(model.PartitionKey);
            await _db.TemporaryExposureKey.UpsertItemAsync(model, pk);
        }
    }
}
