using Castle.Core.Logging;
using Covid19Radar.Api;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Tests.Mock;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Covid19Radar.Api.Tests.Common.DataStore
{
    [TestClass]
    [TestCategory("Common")]
    public class CosmosTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["COSMOS_AUTO_GENERATE"]).Returns("true");
            var container = new Mock<Container>();
            var containerResponse = new Mock<ContainerResponse>();
            var database = new Mock<Database>();
            database.Setup(_ => _.CreateContainerIfNotExistsAsync
                (It.IsAny<ContainerProperties>(), It.IsAny<int?>(), It.IsAny<Microsoft.Azure.Cosmos.RequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(containerResponse.Object);
            database.Setup(_ => _.GetContainer(It.IsAny<string>()))
                .Returns(container.Object);
            var databaseResponse = new Mock<DatabaseResponse>();
            databaseResponse.Setup(_ => _.Database).Returns(database.Object);
            databaseResponse.Setup(_ => _.StatusCode).Returns(System.Net.HttpStatusCode.OK);
            var client = new Mock<CosmosClient>();
            client.Setup(_ => _.CreateDatabaseIfNotExistsAsync
                (It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<Microsoft.Azure.Cosmos.RequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(databaseResponse.Object);
                
            var logger = new LoggerMock<ICosmos>();
            // action
            var instance = new Cosmos(config.Object, client.Object, logger);
        }

        [TestMethod]
        public void CreateMethodThrowException()
        {
            // preparation
            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["COSMOS_AUTO_GENERATE"]).Returns("true");
            var container = new Mock<Container>();
            var containerResponse = new Mock<ContainerResponse>();
            var database = new Mock<Database>();
            database.Setup(_ => _.CreateContainerIfNotExistsAsync
                (It.IsAny<ContainerProperties>(), It.IsAny<int?>(), It.IsAny<Microsoft.Azure.Cosmos.RequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(containerResponse.Object);
            database.Setup(_ => _.GetContainer(It.IsAny<string>()))
                .Returns(container.Object);
            var databaseResponse = new Mock<DatabaseResponse>();
            databaseResponse.Setup(_ => _.Database).Returns(database.Object);
            databaseResponse.Setup(_ => _.StatusCode).Returns(System.Net.HttpStatusCode.BadRequest);
            var client = new Mock<CosmosClient>();
            client.Setup(_ => _.CreateDatabaseIfNotExistsAsync
                (It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<Microsoft.Azure.Cosmos.RequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(databaseResponse.Object);

            var logger = new LoggerMock<ICosmos>();
            // action
            Assert.ThrowsException<AggregateException>(() => {
                var instance = new Cosmos(config.Object, client.Object, logger);
            });
        }

        [TestMethod]
        public void GetContainerProperties()
        {
            // preparation
            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["COSMOS_AUTO_GENERATE"]).Returns("true");
            var container = new Mock<Container>();
            var containerResponse = new Mock<ContainerResponse>();
            var database = new Mock<Database>();
            database.Setup(_ => _.CreateContainerIfNotExistsAsync
                (It.IsAny<ContainerProperties>(), It.IsAny<int?>(), It.IsAny<Microsoft.Azure.Cosmos.RequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(containerResponse.Object);
            database.Setup(_ => _.GetContainer(It.IsAny<string>()))
                .Returns(container.Object);
            var databaseResponse = new Mock<DatabaseResponse>();
            databaseResponse.Setup(_ => _.Database).Returns(database.Object);
            databaseResponse.Setup(_ => _.StatusCode).Returns(System.Net.HttpStatusCode.OK);
            var client = new Mock<CosmosClient>();
            client.Setup(_ => _.CreateDatabaseIfNotExistsAsync
                (It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<Microsoft.Azure.Cosmos.RequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(databaseResponse.Object);
            client.Setup(_ => _.GetDatabase(It.IsAny<string>()))
                .Returns(database.Object);

            var logger = new LoggerMock<ICosmos>();
            var instance = new Cosmos(config.Object, client.Object, logger);
            // action
            Assert.IsNotNull(instance.Diagnosis);
            Assert.IsNotNull(instance.Notification);
            Assert.IsNotNull(instance.Sequence);
            Assert.IsNotNull(instance.TemporaryExposureKey);
            Assert.IsNotNull(instance.TemporaryExposureKeyExport);
            Assert.IsNotNull(instance.User);
        }
    }
}
