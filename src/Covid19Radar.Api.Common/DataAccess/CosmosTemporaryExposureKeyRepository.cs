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
            _logger.LogInformation($"start {nameof(GetAsync)}");
            return new TemporaryExposureKeyModel();
        }

        public async Task<TemporaryExposureKeyModel[]> GetNextAsync()
        {
            _logger.LogInformation($"start {nameof(GetNextAsync)}");
            var oldest = (int)DateTimeOffset.UtcNow.AddDays(Constants.OutOfDateDays).ToUnixTimeSeconds() / 600;
            var query = _db.TemporaryExposureKey.GetItemLinqQueryable<TemporaryExposureKeyModel>(true)
                .Where(tek => tek.RollingStartIntervalNumber > oldest)
                .Where(tek => tek.ExportId == null)
                .ToFeedIterator();
            var e = Enumerable.Empty<TemporaryExposureKeyModel>();
            while (query.HasMoreResults)
            {
                var r = await query.ReadNextAsync();
                e = e.Concat(r.Resource);
            }
            return e.ToArray();
        }
        public async Task<TemporaryExposureKeyModel[]> GetOutOfTimeKeysAsync()
        {
            _logger.LogInformation($"start {nameof(GetOutOfTimeKeysAsync)}");
            var oldest = (int)DateTimeOffset.UtcNow.AddDays(Constants.OutOfDateDays).ToUnixTimeSeconds() / 600;
            var query = _db.TemporaryExposureKey.GetItemLinqQueryable<TemporaryExposureKeyModel>(true)
                .Where(tek => tek.RollingStartIntervalNumber < oldest)
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
