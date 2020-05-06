using Covid19Radar.DataStore;
using Covid19Radar.Models;
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
        private readonly ILogger<CosmosTemporaryExposureKeyRepository> _logger;

        public CosmosTemporaryExposureKeyRepository(ICosmos db, ILogger<CosmosTemporaryExposureKeyRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<TemporaryExposureKeysResult> GetKeysAsync(long sinceEpochSeconds)
        {
            var oldest = DateTimeOffset.UtcNow.AddDays(-14).ToUnixTimeSeconds();

            // Only allow the last 14 days +
            if (sinceEpochSeconds < oldest)
                sinceEpochSeconds = oldest;

            var results = _db.TemporaryExposureKey.GetItemLinqQueryable<TemporaryExposureKey>(true)
                .Where(tek => tek.TimestampSecondsSinceEpoch >= sinceEpochSeconds).ToArray();

            var newestTimestamp = results
                .OrderByDescending(tek => tek.TimestampSecondsSinceEpoch)
                .FirstOrDefault()?.TimestampSecondsSinceEpoch;
            var keys = results.Select(tek => TemporaryExposureKeysResult.Key.FromDatastore(tek));

            return new TemporaryExposureKeysResult
            {
                Timestamp = newestTimestamp ?? DateTimeOffset.MinValue.ToUnixTimeSeconds(),
                Keys = keys
            };
        }
    }
}
