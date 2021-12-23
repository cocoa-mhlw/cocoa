using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Covid19Radar.Api
{
    public class V1EventLog
    {
        public V1EventLog()
        {
        }

        [FunctionName(nameof(V1EventLog))]
        public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "v1/event_log")] HttpRequest req
        )
        {
            return new OkObjectResult("");
        }

    }
}
