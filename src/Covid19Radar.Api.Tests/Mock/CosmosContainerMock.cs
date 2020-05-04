using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;

namespace Covid19Radar.Tests.Mock
{
    public class CosmosContainerMock : Container
    {
        public string IdValue;
        public override string Id => IdValue;

        public Conflicts ConflictsValue;
        public override Conflicts Conflicts => ConflictsValue;

        public Scripts ScriptsValue;
        public override Scripts Scripts => ScriptsValue;

        public override Task<ItemResponse<T>> CreateItemAsync<T>(T item, PartitionKey? partitionKey = null, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new CosmosItemResponseMock<T>();
            return Task.FromResult((ItemResponse<T>)returnValue);
        }

        public override Task<ResponseMessage> CreateItemStreamAsync(Stream streamPayload, PartitionKey partitionKey, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new ResponseMessage();
            return Task.FromResult(returnValue);
        }

        public TransactionalBatch CreateTransactionalBatchReturnValue;
        public override TransactionalBatch CreateTransactionalBatch(PartitionKey partitionKey)
        {
            return CreateTransactionalBatchReturnValue;
        }

        public override Task<ContainerResponse> DeleteContainerAsync(ContainerRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new CosmosContainerResponseMock();
            return Task.FromResult((ContainerResponse)returnValue);
        }

        public override Task<ResponseMessage> DeleteContainerStreamAsync(ContainerRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new ResponseMessage();
            return Task.FromResult(returnValue);
        }

        public override Task<ItemResponse<T>> DeleteItemAsync<T>(string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new CosmosItemResponseMock<T>();
            return Task.FromResult((ItemResponse<T>)returnValue);
        }

        public override Task<ResponseMessage> DeleteItemStreamAsync(string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new ResponseMessage();
            return Task.FromResult(returnValue);
        }

        public ChangeFeedProcessorBuilder GetChangeFeedEstimatorBuilderReturnValue;
        public override ChangeFeedProcessorBuilder GetChangeFeedEstimatorBuilder(string processorName, ChangesEstimationHandler estimationDelegate, TimeSpan? estimationPeriod = null)
        {
            return GetChangeFeedEstimatorBuilderReturnValue;
        }

        public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder<T>(string processorName, ChangesHandler<T> onChangesDelegate)
        {
            return GetChangeFeedEstimatorBuilderReturnValue;
        }

        public override IOrderedQueryable<T> GetItemLinqQueryable<T>(bool allowSynchronousQueryExecution = false, string continuationToken = null, QueryRequestOptions requestOptions = null)
        {
            var r = new EnumerableQuery<T>(Enumerable.Empty<T>());
            var returnValue = r.OrderBy(_ => _);
            return returnValue;
        }

        public override FeedIterator<T> GetItemQueryIterator<T>(QueryDefinition queryDefinition, string continuationToken = null, QueryRequestOptions requestOptions = null)
        {
            var returnValue = new CosmosFeedIteratorMock<T>();
            return returnValue;
        }

        public override FeedIterator<T> GetItemQueryIterator<T>(string queryText = null, string continuationToken = null, QueryRequestOptions requestOptions = null)
        {
            var returnValue = new CosmosFeedIteratorMock<T>();
            return returnValue;
        }

        public override FeedIterator GetItemQueryStreamIterator(QueryDefinition queryDefinition, string continuationToken = null, QueryRequestOptions requestOptions = null)
        {
            var returnValue = new CosmosFeedIteratorMock();
            return returnValue;
        }

        public override FeedIterator GetItemQueryStreamIterator(string queryText = null, string continuationToken = null, QueryRequestOptions requestOptions = null)
        {
            var returnValue = new CosmosFeedIteratorMock();
            return returnValue;
        }

        public override Task<ContainerResponse> ReadContainerAsync(ContainerRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new CosmosContainerResponseMock();
            return Task.FromResult((ContainerResponse)returnValue);
        }

        public override Task<ResponseMessage> ReadContainerStreamAsync(ContainerRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new ResponseMessage();
            return Task.FromResult(returnValue);
        }

        public override Task<ItemResponse<T>> ReadItemAsync<T>(string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new CosmosItemResponseMock<T>();
            return Task.FromResult((ItemResponse<T>)returnValue);
        }

        public override Task<ResponseMessage> ReadItemStreamAsync(string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new ResponseMessage();
            return Task.FromResult(returnValue);
        }

        public int? ReadThroughputAsyncReturnValue;
        public override Task<int?> ReadThroughputAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ReadThroughputAsyncReturnValue);
        }

        public ThroughputResponse ReadThroughputAsyncReturnValueForThroughputResponse;
        public override Task<ThroughputResponse> ReadThroughputAsync(RequestOptions requestOptions, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ReadThroughputAsyncReturnValueForThroughputResponse);
        }

        public override Task<ContainerResponse> ReplaceContainerAsync(ContainerProperties containerProperties, ContainerRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new CosmosContainerResponseMock();
            return Task.FromResult((ContainerResponse)returnValue);
        }

        public override Task<ResponseMessage> ReplaceContainerStreamAsync(ContainerProperties containerProperties, ContainerRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new ResponseMessage();
            return Task.FromResult(returnValue);
        }

        public override Task<ItemResponse<T>> ReplaceItemAsync<T>(T item, string id, PartitionKey? partitionKey = null, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new CosmosItemResponseMock<T>();
            return Task.FromResult((ItemResponse<T>)returnValue);
        }

        public override Task<ResponseMessage> ReplaceItemStreamAsync(Stream streamPayload, string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new ResponseMessage();
            return Task.FromResult(returnValue);
        }

        public override Task<ThroughputResponse> ReplaceThroughputAsync(int throughput, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new CosmosThroughputResponseMock();
            return Task.FromResult((ThroughputResponse)returnValue);
        }

        public override Task<ItemResponse<T>> UpsertItemAsync<T>(T item, PartitionKey? partitionKey = null, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new CosmosItemResponseMock<T>();
            return Task.FromResult((ItemResponse<T>)returnValue);
        }

        public override Task<ResponseMessage> UpsertItemStreamAsync(Stream streamPayload, PartitionKey partitionKey, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var returnValue = new ResponseMessage();
            return Task.FromResult(returnValue);
        }
    }
}
