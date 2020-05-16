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
    public class CosmosTemporaryExposureKeyExportRepository : ITemporaryExposureKeyExportRepository
    {
        const string SequenceName = "BatchNum";
        private readonly ICosmos _db;
        private readonly ISequenceRepository _sequence;
        private readonly ILogger<CosmosTemporaryExposureKeyExportRepository> _logger;

        public CosmosTemporaryExposureKeyExportRepository(
            ICosmos db,
            ISequenceRepository sequence,
            ILogger<CosmosTemporaryExposureKeyExportRepository> logger)
        {
            _db = db;
            _sequence = sequence;
            _logger = logger;
        }
        public async Task<TemporaryExposureKeyExportModel> CreateAsync()
        {
            var partitionKey = PartitionKey.None;
            var newItem = new TemporaryExposureKeyExportModel();
            newItem.BatchNum = await _sequence.GetNextAsync(SequenceName, 1);
            var pk = PartitionKey.Null;
            return (await _db.TemporaryExposureKeyExport.CreateItemAsync(newItem, pk)).Resource;
        }

        public async Task<TemporaryExposureKeyExportModel> GetAsync(string id)
        {
            return await _db.TemporaryExposureKeyExport.ReadItemAsync<TemporaryExposureKeyExportModel>(id, PartitionKey.Null);
        }

        public async Task<TemporaryExposureKeyExportModel[]> GetKeysAsync(long sinceEpochSeconds)
        {
            var oldest = DateTimeOffset.UtcNow.AddDays(-14).ToUnixTimeSeconds();

            // Only allow the last 14 days +
            if (sinceEpochSeconds < oldest)
                sinceEpochSeconds = oldest;

            var query = _db.TemporaryExposureKeyExport.GetItemLinqQueryable<TemporaryExposureKeyExportModel>(true)
                .Where(tek => tek.TimestampSecondsSinceEpoch >= sinceEpochSeconds)
                .ToFeedIterator();

            var e = Enumerable.Empty<TemporaryExposureKeyExportModel>();
            while (query.HasMoreResults)
            {
                var r = await query.ReadNextAsync();
                e = e.Concat(r.Resource);
            }

            return e.ToArray();
        }

        public async Task UpdateAsync(TemporaryExposureKeyExportModel model)
        {
            await _db.TemporaryExposureKeyExport.UpsertItemAsync(model);
        }
    }
}
