using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Covid19Radar.Models;
using Covid19Radar.DataStore;

namespace Covid19Radar.Api
{
    public class BeaconApi
    {

        private ICosmos Cosmos;
        private ILogger<BeaconApi> Logger;

        public BeaconApi(ICosmos cosmos, ILogger<BeaconApi> logger)
        {
            Cosmos = cosmos;
            Logger = logger;
        }

        [FunctionName("Beacon")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            Logger.LogInformation("C# HTTP trigger function processed a request.");

            switch (req.Method)
            {
                case "POST":
                    return await Post(req);
            }

            return new BadRequestObjectResult("Not Supported");
        }

        private async Task<IActionResult> Post(HttpRequest req)
        {
            // convert Postdata to BeaconDataModel
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<BeaconDataModel>(requestBody);

            // save to DB
            return await Add(data);
        }

        private async Task<IActionResult> Add(BeaconDataModel data)
        {
            data.id = Guid.NewGuid().ToString();
            var result = await Cosmos.Beacon.CreateItemAsync(data);
            return new StatusCodeResult(201);
        }
    }
}
