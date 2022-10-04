/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Covid19Radar.Api.DataAccess
{
    public class CosmosEventLogRepository : IEventLogRepository
    {
        private readonly ICosmos _db;
        private readonly ILogger<CosmosEventLogRepository> _logger;

        public CosmosEventLogRepository(
            ICosmos db,
            ILogger<CosmosEventLogRepository> logger
            )
        {
            _db = db;
            _logger = logger;
        }

        public async Task UpsertAsync(EventLogModel eventLog)
        {
            PartitionKey? pk = null;
            if(eventLog.PartitionKey is not null)
            {
                pk = new PartitionKey(eventLog.PartitionKey);
            }        
            ItemResponse<EventLogModel> recordAdded = await _db.EventLog.UpsertItemAsync(eventLog, pk);
            _logger.LogInformation($"{nameof(UpsertAsync)} RequestCharge:{recordAdded.RequestCharge}");
        }
    }
}
