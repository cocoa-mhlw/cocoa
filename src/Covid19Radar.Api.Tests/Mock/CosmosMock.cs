using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Covid19Radar.DataStore;
using Microsoft.Azure.Cosmos;

namespace Covid19Radar.Tests.Mock
{
    /// <summary>
    /// Mock Cosmos
    /// </summary>
    public class CosmosMock : ICosmos
    {
        public Container User { get; set; } = new CosmosContainerMock();
        public Container TemporaryExposureKey { get; set; } = new CosmosContainerMock();
        public Container TemporaryExposureKeyExport { get; set; } = new CosmosContainerMock();
        public Container Diagnosis { get; set; } = new CosmosContainerMock();
        public Container Notification { get; set; } = new CosmosContainerMock();
        public Container Sequence { get; set; } = new CosmosContainerMock();
    }
}
