using System;
using System.Threading.Tasks;
using Covid19Radar.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Covid19Radar
{
    public class TemporaryExposureKeyDeleteBatch
    {
        public readonly ITemporaryExposureKeyDeleteBatchService BatchService;

        public TemporaryExposureKeyDeleteBatch(ITemporaryExposureKeyDeleteBatchService batchService)
        {
            BatchService = batchService;
        }

        [FunctionName("TemporaryExposureKeyDeleteBatch")]
        public async Task Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await BatchService.RunAsync();
        }
    }
}
