using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Covid19Radar.Api.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Covid19Radar.Api.DataStore
{
    /// <summary>
    /// Cosmos Database 
    /// </summary>
    public class Cosmos : ICosmos
    {
        private ILogger<ICosmos> Logger;
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
        public Container Notification { get => Database.GetContainer("Notification"); }
        public Container TemporaryExposureKey { get => Database.GetContainer("TemporaryExposureKey"); }
        public Container Diagnosis { get => Database.GetContainer("Diagnosis"); }
        public Container TemporaryExposureKeyExport { get => Database.GetContainer("TemporaryExposureKeyExport"); }
        public Container Sequence { get => Database.GetContainer("Sequence"); }
        public Container AuthorizedApp { get => Database.GetContainer("AuthorizedApp"); }

        /// <summary>
        /// DI Constructor
        /// </summary>
        /// <param name="config">configuration</param>
        /// <param name="client">cosmos db client</param>
        /// <param name="logger">logger</param>
        public Cosmos(IConfiguration config,
                      CosmosClient client,
                      ILogger<ICosmos> logger)
        {
            Logger = logger;
            DatabaseId = config.CosmosDatabaseId();
            AutoGenerate = config.CosmosAutoGenerate();

            // Create a new instance of the Cosmos Client
            CosmosClient = client;

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
            await user.CreateItemAsync<UserModel>(new UserModel() { UserUuid = "TEST" });

            // Container Notification
            Logger.LogInformation("GenerateAsync Create Notification Container");
            try
            {
                await dbResult.Database.GetContainer("Notification").DeleteContainerAsync();
            }
            catch { }
            var notificationProperties = new ContainerProperties("Notification", "/PartitionKey");
            var notificationResult = await dbResult.Database.CreateContainerIfNotExistsAsync(notificationProperties);

            // Container Diagnosis
            Logger.LogInformation("GenerateAsync Create Diagnosis Container");
            try
            {
                await dbResult.Database.GetContainer("Diagnosis").DeleteContainerAsync();
            }
            catch { }
            var diagnosisExposureKeyProperties = new ContainerProperties("Diagnosis", "/PartitionKey");
            var diagnosisExposureKeyResult = await dbResult.Database.CreateContainerIfNotExistsAsync(diagnosisExposureKeyProperties);

            // Container TemporaryExposureKey
            Logger.LogInformation("GenerateAsync Create TemporaryExposureKey Container");
            try
            {
                await dbResult.Database.GetContainer("TemporaryExposureKey").DeleteContainerAsync();
            }
            catch { }
            var temporaryExposureKeyProperties = new ContainerProperties("TemporaryExposureKey", "/PartitionKey");
            var temporaryExposureKeyResult = await dbResult.Database.CreateContainerIfNotExistsAsync(temporaryExposureKeyProperties);

            // Container TemporaryExposureKeyExport
            Logger.LogInformation("GenerateAsync Create TemporaryExposureKeyExport Container");
            try
            {
                await dbResult.Database.GetContainer("TemporaryExposureKeyExport").DeleteContainerAsync();
            }
            catch { }
            var temporaryExposureKeyExportProperties = new ContainerProperties("TemporaryExposureKeyExport", "/PartitionKey");
            var temporaryExposureKeyExportResult = await dbResult.Database.CreateContainerIfNotExistsAsync(temporaryExposureKeyExportProperties);

            // Container Sequence
            Logger.LogInformation("GenerateAsync Create Sequence Container");
            try
            {
                await dbResult.Database.GetContainer("Sequence").DeleteContainerAsync();
            }
            catch { }
            var sequenceProperties = new ContainerProperties("Sequence", "/PartitionKey");
            var sequenceResult = await dbResult.Database.CreateContainerIfNotExistsAsync(sequenceProperties);

            // Container AuthorizedApp
            Logger.LogInformation("GenerateAsync Create AuthorizedApp Container");
            try
            {
                await dbResult.Database.GetContainer("AuthorizedApp").DeleteContainerAsync();
            }
            catch { }
            var authorizedAppProperties = new ContainerProperties("AuthorizedApp", "/PartitionKey");
            var authorizedAppResult = await dbResult.Database.CreateContainerIfNotExistsAsync(authorizedAppProperties);

        }
    }
}
