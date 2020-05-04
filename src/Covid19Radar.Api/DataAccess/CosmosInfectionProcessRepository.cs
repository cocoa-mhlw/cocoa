using Covid19Radar.DataStore;
using Covid19Radar.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.DataAccess
{
    public class CosmosInfectionProcessRepository : IInfectionProcessRepository
    {
        private readonly ICosmos _db;

        public CosmosInfectionProcessRepository(ICosmos db)
        {
            _db = db;
        }
        public Task Upsert(InfectionProcessModel infectionProcess)
        {
            return _db.InfectionProcess.UpsertItemAsync<InfectionProcessModel>(infectionProcess, new PartitionKey(infectionProcess.PartitionKey));

        }
    }
}
