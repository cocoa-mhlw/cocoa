using Covid19Radar.Api.Common;
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
    public class CosmosTemporaryExposureKeyExportRepository : ITemporaryExposureKeyExportRepository
    {
        private readonly ICosmos _db;
        private readonly ILogger<CosmosTemporaryExposureKeyExportRepository> _logger;
        private readonly QueryCache<TemporaryExposureKeyExportModel[]> GetKeysAsyncCache;

        public CosmosTemporaryExposureKeyExportRepository(
            ICosmos db,
            ISequenceRepository sequence,
            ILogger<CosmosTemporaryExposureKeyExportRepository> logger)
        {
            _db = db;
            _logger = logger;
            GetKeysAsyncCache = new QueryCache<TemporaryExposureKeyExportModel[]>(Constants.CacheTimeout);
        }
        public async Task<TemporaryExposureKeyExportModel> CreateAsync(TemporaryExposureKeyExportModel model)
        {
            var pk = new PartitionKey(model.Region);
            var response = await _db.TemporaryExposureKeyExport.CreateItemAsync(model, pk);
            return response.Resource;
        }

        public async Task<TemporaryExposureKeyExportModel> GetAsync(string id)
        {
            _logger.LogInformation($"start {nameof(GetAsync)}");
            return await _db.TemporaryExposureKeyExport.ReadItemAsync<TemporaryExposureKeyExportModel>(id, PartitionKey.Null);
        }

        public async Task<TemporaryExposureKeyExportModel[]> GetKeysAsync(ulong sinceEpochSeconds)
        {
            _logger.LogInformation($"start {nameof(GetKeysAsync)}");

            var oldest = (ulong)DateTimeOffset.UtcNow.AddDays(Constants.OutOfDateDays).ToUnixTimeSeconds();

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

        public async Task<TemporaryExposureKeyExportModel[]> GetKeysAsync(ulong sinceEpochSeconds, string region)
        {
            _logger.LogInformation($"start {nameof(GetKeysAsync)}");

            var oldest = (ulong)DateTimeOffset.UtcNow.AddDays(Constants.OutOfDateDays).ToUnixTimeSeconds();

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

            return items
                .Where(tek => tek.EndTimestamp >= sinceEpochSeconds)
                .Where(tek => tek.Region == region)
                .ToArray();
        }
        public async Task<TemporaryExposureKeyExportModel[]> GetOutOfTimeKeysAsync()
        {
            _logger.LogInformation($"start {nameof(GetOutOfTimeKeysAsync)}");
            var oldest = (ulong)DateTimeOffset.UtcNow.AddDays(Constants.OutOfDateDays).ToUnixTimeSeconds();
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
