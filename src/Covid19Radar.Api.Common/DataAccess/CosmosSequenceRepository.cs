using Covid19Radar.Api.Common;
using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<ulong> GetNextAsync(string key, ulong startNo)
        {
            _logger.LogInformation($"start {nameof(GetNextAsync)}");
            var pk = new PartitionKey(key);
            dynamic[] spParams = { key, startNo };
            for (var i = 0; i < 50; i++)
            {
                try
                {
                    var r = await _db.Sequence.Scripts.ExecuteStoredProcedureAsync<SequenceModel>("spIncrement", pk, spParams);
                    _logger.LogInformation($"spIncrement RequestCharge:{r.RequestCharge}");
                    return r.Resource.value;
                }
                catch (CosmosException ex)
                {
                    _logger.LogInformation(ex, $"GetNextAsync Retry {i} RequestCharge:{ex.RequestCharge}");
                    await Task.Delay(5);
                    continue;
                }
            }
            _logger.LogWarning("GetNextAsync is over retry count.");
            throw new ApplicationException("GetNextAsync is over retry count.");
        }
    }
}
