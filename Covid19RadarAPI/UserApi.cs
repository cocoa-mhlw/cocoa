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
using Microsoft.Azure.Cosmos;

namespace Covid19Radar.Api
{
    public class UserApi
    {

        private readonly ICosmos Cosmos;
        private readonly ILogger<UserApi> Logger;

        public UserApi(ICosmos cosmos, ILogger<UserApi> logger)
        {
            Cosmos = cosmos;
            Logger = logger;
        }


        [FunctionName("User")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            Logger.LogInformation("C# HTTP trigger function processed a request.");

            switch (req.Method)
            {
                case "POST":
                    return await Post(req);
            }

            AddBadRequest(req);
            return new BadRequestObjectResult("");
        }

        private async Task<IActionResult> Post(HttpRequest req)
        {
            // convert Postdata to BeaconDataModel
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var user = JsonConvert.DeserializeObject<UserParameter>(requestBody);

            // validation
            if (string.IsNullOrWhiteSpace(user.UserUuid)
                || string.IsNullOrWhiteSpace(user.Major)
                || string.IsNullOrWhiteSpace(user.Minor))
            {
                AddBadRequest(req);
                return new BadRequestObjectResult("");
            }

            // query
            return await Query(req, user);
        }

        private async Task<IActionResult> Query(HttpRequest req, UserParameter user)
        {
            try
            {
                var itemResult = await Cosmos.User.ReadItemAsync<UserResultModel>(user.GetId(), PartitionKey.None);
                if (itemResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return new OkObjectResult(itemResult.Resource);
                }
            }
            catch (CosmosException ex)
            {
                // 429–TooManyRequests
                if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    return new StatusCodeResult(503);
                }
                AddBadRequest(req);
                return new StatusCodeResult((int)ex.StatusCode);
            }
            AddBadRequest(req);
            return new NotFoundResult();
        }

        private void AddBadRequest(HttpRequest req)
        {
            // add deny list
        }
    }
}
