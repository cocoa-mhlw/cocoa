using System;
using Covid19Radar.DataStore;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Covid19Radar
{
    public class BeaconQueueFunction
    {
        private readonly ICosmos Cosmos;
        private readonly ILogger<BeaconQueueFunction> Logger;
        private readonly Microsoft.Extensions.Configuration.IConfiguration Config;
        public BeaconQueueFunction(ICosmos cosmos,
            IConfiguration config,
            ILogger<BeaconQueueFunction> logger)
        {
            Cosmos = cosmos;
            Logger = logger;
            Config = config;
        }


        [FunctionName("BeaconQueueFunction")]
        public void Run([QueueTrigger("myqueue-items", Connection = "")]string myQueueItem)
        {
            Logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
