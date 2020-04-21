using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Covid19Radar.DataStore
{
    /// <summary>
    /// Cosmos Database 
    /// </summary>
    public class StoringCosmos : IStoringCosmos
    {
        private ILogger<IStoringCosmos> Logger;

        private string EndpointUri { get; set; }
        // The primary key for the Azure Cosmos account.
        private string PrimaryKey { get; set; }
        // Database Id
        private string DatabaseId { get; set; }
        // The Cosmos client instance
        private CosmosClient CosmosClient;

        // The database we will create
        private Database Database;
    
        // コンテナの自動生成
        private bool AutoGenerate;

        public Container Contact { get => Database.GetContainer("Contact"); }

        /// <summary>
        /// DI Constructor
        /// </summary>
        /// <param name="config">configuration</param>
        public StoringCosmos(Microsoft.Extensions.Configuration.IConfiguration config, ILogger<IStoringCosmos> logger)
        {
            Logger = logger;
            EndpointUri = config.GetSection("COSMOS_STORING_ENDPOINT_URI").Value;
            PrimaryKey = config.GetSection("COSMOS_STORING_PRIMARY_KEY").Value;
            DatabaseId = config.GetSection("COSMOS_STORING_DATABASE_ID").Value;
            AutoGenerate = bool.Parse(config.GetSection("COSMOS_AUTO_GENERATE").Value);

            // Create a new instance of the Cosmos Client
            CosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

            // Autogenerate
            GenerateAsync().Wait();


            // get database
            Database = this.CosmosClient.GetDatabase(DatabaseId);
        }

        /// <summary>
        /// Auto-generate
        /// </summary>
        private async Task GenerateAsync()
        {
            // return if disable AutoGenerate
            if (!AutoGenerate) return;

            Logger.LogInformation("GenerateAsync");
            var dbResult = await CosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
            if (dbResult.StatusCode != System.Net.HttpStatusCode.OK
                && dbResult.StatusCode != System.Net.HttpStatusCode.Created)
            {
                Logger.LogError(dbResult.ToString());
                throw new ApplicationException(dbResult.ToString());
            }

            // Container Contact
            Logger.LogInformation($"GenerateAsync Create Contact Container");
            try
            {
                await dbResult.Database.GetContainer("Contact").DeleteContainerAsync();
            }
            catch { }
            var contactProperties = new ContainerProperties("Contact", "/PartitionKey");
            var contactResult = await dbResult.Database.CreateContainerIfNotExistsAsync(contactProperties);

        }

    }
}
