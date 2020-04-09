using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Covid19Radar.DataStore
{
    public class Cosmos : ICosmos
    {
        private string EndpointUri { get; set; }
        // The primary key for the Azure Cosmos account.
        private string PrimaryKey { get; set; }

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        public Cosmos(Microsoft.Extensions.Configuration.IConfiguration config)
        {
            this.EndpointUri = config.GetSection("COSMOS_ENDPOINT_URI").Value;
            this.PrimaryKey = config.GetSection("COSMOS_PRIMARY_KEY").Value;

            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
        }

        public async Task CreateDatabaseAsync(string databaseId)
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }
    }
}
