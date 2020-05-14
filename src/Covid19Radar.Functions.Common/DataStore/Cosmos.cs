using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Covid19Radar.Models;
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

        // Beacon store name
        private string BeaconStoreName;

        // The container we will create.
        public Container User { get => Database.GetContainer("User"); }
        public Container Notification { get => Database.GetContainer("Notification"); }
        public Container TemporaryExposureKey { get => Database.GetContainer("TemporaryExposureKey"); }
        public Container Diagnosis { get => Database.GetContainer("Diagnosis"); }

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

            // Container User
            Logger.LogInformation("GenerateAsync Create User Container");
            try
            {
                await dbResult.Database.GetContainer("User").DeleteContainerAsync();
            }
            catch { }
            var userProperties = new ContainerProperties("User", "/PartitionKey");
            var userDataResult = await dbResult.Database.CreateContainerIfNotExistsAsync(userProperties);
            var user = dbResult.Database.GetContainer("User");
            await user.CreateItemAsync<UserModel>(new UserModel() {UserUuid = "TEST" });

            // Container Notification
            Logger.LogInformation("GenerateAsync Create Notification Container");
            try
            {
                await dbResult.Database.GetContainer("Notification").DeleteContainerAsync();
            }
            catch { }
            var notificationProperties = new ContainerProperties("Notification", "/PartitionKey");
            var notificationResult = await dbResult.Database.CreateContainerIfNotExistsAsync(notificationProperties);

            // Container TemporaryExposureKey
            Logger.LogInformation("GenerateAsync Create TemporaryExposureKey Container");
            try
            {
                await dbResult.Database.GetContainer("TemporaryExposureKey").DeleteContainerAsync();
            }
            catch { }
            var temporaryExposureKeyProperties = new ContainerProperties("TemporaryExposureKey", "/PartitionKey");
            var temporaryExposureKeyResult = await dbResult.Database.CreateContainerIfNotExistsAsync(temporaryExposureKeyProperties);

            // Container Diagnosis
            Logger.LogInformation("GenerateAsync Create Diagnosis Container");
            try
            {
                await dbResult.Database.GetContainer("Diagnosis").DeleteContainerAsync();
            }
            catch { }
            var diagnosisExposureKeyProperties = new ContainerProperties("Diagnosis", "/PartitionKey");
            var diagnosisExposureKeyResult = await dbResult.Database.CreateContainerIfNotExistsAsync(diagnosisExposureKeyProperties);
        }

    }
}
