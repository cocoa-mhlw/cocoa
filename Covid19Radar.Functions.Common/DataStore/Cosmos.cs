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
    public class Cosmos : ICosmos
    {
        private ILogger<ICosmos> Logger;

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

        // The container we will create.
        public Container User { get => Database.GetContainer("User"); }
        public Container Beacon { get => Database.GetContainer("Beacon"); }
        public Container Sequence { get => Database.GetContainer("Sequence");  }
        public Container Otp { get => Database.GetContainer("Otp"); }

        /// <summary>
        /// DI Constructor
        /// </summary>
        /// <param name="config">configuration</param>
        public Cosmos(Microsoft.Extensions.Configuration.IConfiguration config, ILogger<ICosmos> logger)
        {
            Logger = logger;
            EndpointUri = config.GetSection("COSMOS_ENDPOINT_URI").Value;
            PrimaryKey = config.GetSection("COSMOS_PRIMARY_KEY").Value;
            DatabaseId = config.GetSection("COSMOS_DATABASE_ID").Value;
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

            // Container Sequence
            Logger.LogInformation("GenerateAsync Create Sequence Container");
            try
            {
                await dbResult.Database.GetContainer("Sequence").DeleteContainerAsync();
            }
            catch { }
            var sequenceProperties = new ContainerProperties("Sequence", "/PartitionKey");
            var sequenceResult = await dbResult.Database.CreateContainerIfNotExistsAsync(sequenceProperties);
            var sequence = dbResult.Database.GetContainer("Sequence");
            await sequence.CreateItemAsync(new Models.SequenceDataModel()
            {
                Major = 0,
                Minor = 0
            }, null);

            // Container User
            Logger.LogInformation("GenerateAsync Create User Container");
            try
            {
                await dbResult.Database.GetContainer("User").DeleteContainerAsync();
            }
            catch { }
            var userProperties = new ContainerProperties("User", "/PartitionKey");
            var userDataResult = await dbResult.Database.CreateContainerIfNotExistsAsync(userProperties);

            // Container Beacon
            Logger.LogInformation("GenerateAsync Create Beacon Container");
            try
            {
                await dbResult.Database.GetContainer("Beacon").DeleteContainerAsync();
            }
            catch { }
            var beaconProperties = new ContainerProperties("Beacon", "/PartitionKey");
            var beaconResult = await dbResult.Database.CreateContainerIfNotExistsAsync(beaconProperties);

            // Container Otp
            Logger.LogInformation("GenerateAsync Create Otp Container");
            try
            {
                await dbResult.Database.GetContainer("Otp").DeleteContainerAsync();
            }
            catch { }
            var otpProperties = new ContainerProperties("Otp", "/UserUuid");
            var otpResult = await dbResult.Database.CreateContainerIfNotExistsAsync(otpProperties);
        }

    }
}
