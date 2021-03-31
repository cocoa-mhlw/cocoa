/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Common;
using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public class CosmosSequenceRepository : ISequenceRepository
    {
        private readonly ICosmos _db;
        private readonly ILogger<CosmosSequenceRepository> _logger;

        public CosmosSequenceRepository(ICosmos db, ILogger<CosmosSequenceRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ulong> GetNextAsync(string key, ulong startNo, int increment = 1)
        {
            _logger.LogInformation($"start {nameof(GetNextAsync)} {key}");
            var pk = new PartitionKey(key);
            dynamic[] spParams = { key, startNo, increment, null };
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    var r = await _db.Sequence.Scripts.ExecuteStoredProcedureAsync<SequenceModel>("spIncrement", pk, spParams);
                    _logger.LogInformation($"spIncrement RequestCharge:{r.RequestCharge}");
                    return r.Resource.value;
                }
                catch (CosmosException ex)
                {
                    int ms = (int)(ex.RetryAfter.HasValue ? ex.RetryAfter.Value.TotalMilliseconds : 5);
                    _logger.LogInformation(ex, $"GetNextAsync Retry {i} RequestCharge:{ex.RequestCharge} RetryAfter:{ms}");
                    await Task.Delay(ms);
                    continue;
                }
            }
            _logger.LogWarning("GetNextAsync is over retry count.");
            throw new ApplicationException("GetNextAsync is over retry count.");
        }

        public async Task<ulong> GetNextAsync(PartitionKeyRotation.KeyInformation key, ulong startNo, int increment = 1)
        {
            _logger.LogInformation($"start {nameof(GetNextAsync)} {key.Key}");
            var pk = new PartitionKey(key.Key);
            dynamic[] spParams = { key.Key, startNo, increment, key.Self };
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    var r = await _db.Sequence.Scripts.ExecuteStoredProcedureAsync<SequenceModel>("spIncrement", pk, spParams);
                    key.SetSelf(r.Resource._self);
                    _logger.LogInformation($"spIncrement RequestCharge:{r.RequestCharge}");
                    return r.Resource.value;
                }
                catch (CosmosException ex)
                {
                    int ms = (int)(ex.RetryAfter.HasValue ? ex.RetryAfter.Value.TotalMilliseconds : 5);
                    _logger.LogInformation(ex, $"GetNextAsync Retry {i} RequestCharge:{ex.RequestCharge} RetryAfter:{ms}");
                    await Task.Delay(ms);
                    continue;
                }
            }
            _logger.LogWarning("GetNextAsync is over retry count.");
            throw new ApplicationException("GetNextAsync is over retry count.");
        }
    }
}
