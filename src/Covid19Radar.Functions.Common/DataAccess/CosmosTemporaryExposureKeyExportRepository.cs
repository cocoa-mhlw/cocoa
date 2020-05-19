using Covid19Radar.Common;
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
        const int OutOfDate = -14;
        const int CacheTimeout = 60;
        const string SequenceName = "BatchNum";
        private readonly ICosmos _db;
        private readonly ISequenceRepository _sequence;
        private readonly ILogger<CosmosTemporaryExposureKeyExportRepository> _logger;
        private readonly QueryCache<TemporaryExposureKeyExportModel[]> GetKeysAsyncCache;

        public CosmosTemporaryExposureKeyExportRepository(
            ICosmos db,
            ISequenceRepository sequence,
            ILogger<CosmosTemporaryExposureKeyExportRepository> logger)
        {
            _db = db;
            _sequence = sequence;
            _logger = logger;
            GetKeysAsyncCache = new QueryCache<TemporaryExposureKeyExportModel[]>(CacheTimeout);
        }
        public async Task<TemporaryExposureKeyExportModel> CreateAsync()
        {
            var pk = PartitionKey.None;
            var newItem = new TemporaryExposureKeyExportModel();
            var batchNum = await _sequence.GetNextAsync(SequenceName, 1);
            newItem.id = batchNum.ToString();
            newItem.BatchNum = batchNum;
            await _db.TemporaryExposureKeyExport.CreateItemAsync(newItem, pk);
            return newItem;
        }

        public async Task<TemporaryExposureKeyExportModel> GetAsync(string id)
        {
            _logger.LogInformation($"start {nameof(GetAsync)}");
            return await _db.TemporaryExposureKeyExport.ReadItemAsync<TemporaryExposureKeyExportModel>(id, PartitionKey.Null);
        }

        public async Task<TemporaryExposureKeyExportModel[]> GetKeysAsync(ulong sinceEpochSeconds)
        {
            _logger.LogInformation($"start {nameof(GetKeysAsync)}");

            var oldest = (ulong)DateTimeOffset.UtcNow.AddDays(OutOfDate).ToUnixTimeSeconds();

            // Only allow the last 14 days +
            if (sinceEpochSeconds < oldest)
                sinceEpochSeconds = oldest;

            var items = await GetKeysAsyncCache.QueryWithCacheAsync(async () =>
            {
                var query = _db.TemporaryExposureKeyExport.GetItemLinqQueryable<TemporaryExposureKeyExportModel>(true)
                    .Where(tek => !tek.Deleted)
                    .ToFeedIterator();

                var e = Enumerable.Empty<TemporaryExposureKeyExportModel>();
                while (query.HasMoreResults)
                {
                    var r = await query.ReadNextAsync();
                    e = e.Concat(r.Resource);
                }

                return e.ToArray();
            });

            return items.Where(tek => tek.EndTimestamp >= sinceEpochSeconds).ToArray();
        }

        public async Task<TemporaryExposureKeyExportModel[]> GetOutOfTimeKeysAsync()
        {
            _logger.LogInformation($"start {nameof(GetOutOfTimeKeysAsync)}");
            var oldest = (ulong)DateTimeOffset.UtcNow.AddDays(OutOfDate).ToUnixTimeSeconds();
            var query = _db.TemporaryExposureKeyExport.GetItemLinqQueryable<TemporaryExposureKeyExportModel>(true)
                .Where(tek => !tek.Deleted)
                .Where(tek => tek.EndTimestamp < oldest)
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
            _logger.LogInformation($"start {nameof(UpdateAsync)}");
            await _db.TemporaryExposureKeyExport.UpsertItemAsync(model);
        }

    }
}
