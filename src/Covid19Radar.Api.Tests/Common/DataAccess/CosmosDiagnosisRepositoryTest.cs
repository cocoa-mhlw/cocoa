using Covid19Radar.Api;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests.Common.DataAccess
{
    [TestClass]
    [TestCategory("Common")]
    public class CosmosDiagnosisRepositoryTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var cosmos = new Mock<ICosmos>();
            var logger = new Mock.LoggerMock<CosmosDiagnosisRepository>();
            // action
            var instance = new CosmosDiagnosisRepository(cosmos.Object, logger);
        }

        [DataTestMethod]
        [DataRow("")]
        public async Task DeleteAsyncMethod(string userUuid)
        {
            // preparation
            var model = new DiagnosisModel();
            var itemResponse = new Mock<ItemResponse<DiagnosisModel>>();
            itemResponse.Setup(_ => _.Resource).Returns(model);
            var cosmos = new Mock<ICosmos>();
            cosmos.Setup(_ => _.Diagnosis.DeleteItemAsync<DiagnosisModel>(It.IsAny<string>(),
                                                                        It.IsAny<PartitionKey>(),
                                                                        It.IsAny<ItemRequestOptions>(),
                                                                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(itemResponse.Object)
                .Verifiable();
            var logger = new Mock.LoggerMock<CosmosDiagnosisRepository>();
            var user = new Mock<IUser>();
            user.Setup(_ => _.UserUuid).Returns(userUuid);
            var instance = new CosmosDiagnosisRepository(cosmos.Object, logger);
            // action
            await instance.DeleteAsync(user.Object);
            // Assert
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("", "")]
        [DataRow("1", "2")]
        public async Task GetAsyncMethod(string submissionNumber, string userUuid)
        {
            // preparation
            var model = new DiagnosisModel();
            var itemResponse = new Mock<ItemResponse<DiagnosisModel>>();
            itemResponse.Setup(_ => _.Resource).Returns(model);
            var cosmos = new Mock<ICosmos>();
            cosmos.Setup(_ => _.Diagnosis.ReadItemAsync<DiagnosisModel>(It.IsAny<string>(),
                                                                        It.IsAny<PartitionKey>(),
                                                                        It.IsAny<ItemRequestOptions>(),
                                                                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(itemResponse.Object)
                .Verifiable();
            var logger = new Mock.LoggerMock<CosmosDiagnosisRepository>();
            var instance = new CosmosDiagnosisRepository(cosmos.Object, logger);
            // action
            var result = await instance.GetAsync(submissionNumber, userUuid);
            // Assert
            Assert.AreEqual(model, result);
        }

        [TestMethod]
        public async Task GetNotApprovedAsyncMethod()
        {
            // preparation
            var models = new List<DiagnosisModel>();
            models.Add(new DiagnosisModel());
            var cosmos = new Mock<ICosmos>();

            // repeat 2
            var feed = new Mock<FeedIterator<DiagnosisModel>>();
            feed.SetupSequence(_ => _.HasMoreResults)
                .Returns(true)
                .Returns(true)
                .Returns(false);
            var feedResponse = new Mock<FeedResponse<DiagnosisModel>>();
            feedResponse.Setup(_ => _.Resource).Returns(models);
            feed.Setup(_ => _.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponse.Object);
            cosmos.Setup(_ => _.Diagnosis.GetItemQueryIterator<DiagnosisModel>
                        (It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>()))
                .Returns(feed.Object)
                .Verifiable();
            var logger = new Mock.LoggerMock<CosmosDiagnosisRepository>();
            var instance = new CosmosDiagnosisRepository(cosmos.Object, logger);
            // action
            var result = await instance.GetNotApprovedAsync();
            // Assert
            CollectionAssert.AreEqual(models.Concat(models).ToArray(), result);
        }

        [DataTestMethod]
        public async Task SubmitDiagnosisAsyncMethod()
        {
            // preparation
            string SubmissionNumber = "";
            DateTimeOffset timestamp = DateTimeOffset.UtcNow;
            string UserUuid = "";
            TemporaryExposureKeyModel[] keys = new TemporaryExposureKeyModel[] { new TemporaryExposureKeyModel() };
            var itemResponse = new Mock<ItemResponse<DiagnosisModel>>();
            var cosmos = new Mock<ICosmos>();
            cosmos.Setup(_ => _.Diagnosis.UpsertItemAsync
                        (It.IsAny<DiagnosisModel>(), It.IsAny<PartitionKey?>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .Callback<DiagnosisModel, PartitionKey?, ItemRequestOptions, CancellationToken>((m, p, i, c) =>
                  {
                      itemResponse.Setup(_ => _.Resource).Returns(m);
                  })
                .ReturnsAsync(itemResponse.Object)
                .Verifiable();
            var logger = new Mock.LoggerMock<CosmosDiagnosisRepository>();
            var instance = new CosmosDiagnosisRepository(cosmos.Object, logger);
            // action
            var result = await instance.SubmitDiagnosisAsync(SubmissionNumber, timestamp, UserUuid, keys);
            // Assert
            Assert.AreEqual(SubmissionNumber, result.SubmissionNumber);
            Assert.AreEqual(timestamp.ToUnixTimeSeconds(), result.Timestamp);
            Assert.AreEqual(UserUuid, result.UserUuid);
            Assert.AreEqual(UserUuid, result.id);
            CollectionAssert.AreEqual(keys, result.Keys);
        }

    }
}
