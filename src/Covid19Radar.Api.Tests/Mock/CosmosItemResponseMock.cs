using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Api.Tests.Mock
{
    public class CosmosItemResponseMock<T> : ItemResponse<T>
    {
        public CosmosItemResponseMock()
        {
        }
    }
}
