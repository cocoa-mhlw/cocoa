/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using Covid19Radar.Api.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Covid19Radar.Api.DataStore
{
    /// <summary>
    /// Cosmos Database 
    /// </summary>
    public class Cosmos : ICosmos
    {
        private readonly ILogger<ICosmos> Logger;
        // Database Id
        private string DatabaseId { get; set; }
        // The Cosmos client instance
        private readonly CosmosClient CosmosClient;

        // The database we will create
        private readonly Database Database;

        // コンテナの自動生成
        private readonly bool AutoGenerate;

        // The container we will create.
        public Container User { get => Database.GetContainer("User"); }
        public Container Notification { get => Database.GetContainer("Notification"); }
        public Container TemporaryExposureKey { get => Database.GetContainer("TemporaryExposureKey"); }
        public Container Diagnosis { get => Database.GetContainer("Diagnosis"); }
        public Container TemporaryExposureKeyExport { get => Database.GetContainer("TemporaryExposureKeyExport"); }
        public Container Sequence { get => Database.GetContainer("Sequence"); }
        public Container AuthorizedApp { get => Database.GetContainer("AuthorizedApp"); }
        public Container CustomVerificationStatus { get => Database.GetContainer("CustomVerificationStatus"); }

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

            // Autogenerate: only DEBUG
#if DEBUG 
            GenerateAsync().Wait();
#endif

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
            StoredProcedureResponse storedProcedureResponse = await sequenceResult.Container.Scripts.CreateStoredProcedureAsync(new StoredProcedureProperties
            {
                Id = "spIncrement",
                Body = @"
function increment(name, initialValue, incrementValue, _selfId) {
    function upsertCallback(err, resource, options) {
        if (err) throw err;
        var response = getContext().getResponse();
        response.setBody({'value': resource.value, '_self': resource._self});
    }
    function readCallback(err, resource, options) {
        if (err && err.number == 404) {
            var body = {'id': name, 'PartitionKey': name, 'value': initialValue};
            if(!__.createDocument(__.getSelfLink(), body, {'disableAutomaticIdGeneration': true}, upsertCallback)) throw new Error('The createDocument was not accepted');
            return;
        }
        if (err) throw err;
        var body = resource;
        body.value += incrementValue;
        if(!__.replaceDocument(body._self, body, {'etag': body._etag }, upsertCallback)) throw new Error('The replaceDocument was not accepted');
    }
    if (_selfId) {
        if (!__.readDocument(_selfId, {}, readCallback)) throw new Error('The filter was not accepted by the server.');
    } else {
        if (!__.readDocument(__.getAltLink() + '/docs/' + name, {}, readCallback)) throw new Error('The filter was not accepted by the server.');
    }
}
"
            });

            // Container AuthorizedApp
            Logger.LogInformation("GenerateAsync Create AuthorizedApp Container");
            try
            {
                await dbResult.Database.GetContainer("AuthorizedApp").DeleteContainerAsync();
            }
            catch { }
            var authorizedAppProperties = new ContainerProperties("AuthorizedApp", "/PartitionKey");
            var authorizedAppResult = await dbResult.Database.CreateContainerIfNotExistsAsync(authorizedAppProperties);

            // Container CustomVerificationStatus
            Logger.LogInformation("GenerateAsync Create AuthoCustomVerificationStatusrizedApp Container");
            try
            {
                await dbResult.Database.GetContainer("CustomVerificationStatus").DeleteContainerAsync();
            }
            catch { }
            var customVerificationStatusProperties = new ContainerProperties("CustomVerificationStatus", "/PartitionKey");
            var customVerificationStatusResult = await dbResult.Database.CreateContainerIfNotExistsAsync(customVerificationStatusProperties);

        }
    }
}
