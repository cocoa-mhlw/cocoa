using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Covid19Radar.Tests.Mock
{
    public class CosmosFeedIteratorMock : FeedIterator
    {
        public bool HasMoreResultsValue;
        public override bool HasMoreResults => HasMoreResultsValue;

        public CosmosFeedIteratorMock()
        {
        }
        public ResponseMessage ReadNextAsyncReturnValue;
        public override Task<ResponseMessage> ReadNextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ReadNextAsyncReturnValue);
        }
    }

    public class CosmosFeedIteratorMock<T> : FeedIterator<T>
    {
        public bool HasMoreResultsValue;
        public override bool HasMoreResults => HasMoreResultsValue;


        public CosmosFeedIteratorMock()
        {
        }
        public FeedResponse<T> ReadNextAsyncReturnValue;
        public override Task<FeedResponse<T>> ReadNextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ReadNextAsyncReturnValue);
        }
    }
}
