using Covid19Radar.DataStore;
using Covid19Radar.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.DataAccess
{
    public class CosmosBeaconRepository : IBeaconRepository
    {
        private readonly ICosmos _db;

        public CosmosBeaconRepository(ICosmos db)
        {
            _db = db;
        }

        public Task Upsert(BeaconModel beacon)
        {
            return _db.Beacon.UpsertItemAsync<BeaconModel>(beacon, new PartitionKey(beacon.PartitionKey));
        }
    }
}
