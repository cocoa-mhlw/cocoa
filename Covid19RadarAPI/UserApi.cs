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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            Logger.LogInformation("C# HTTP trigger function processed a request.");

            switch (req.Method)
            {
                case "GET":
                    return await Get(req);
            }

            return new BadRequestObjectResult("Not Supported");
        }

        private async Task<IActionResult> Get(HttpRequest req)
        {
            // get name from query 
            string name = req.Query["UserUuid"];

            // get UserData from DB
            return await Query(req.Query["UserUuid"]);
        }

        private async Task<IActionResult> Query(string userUuid)
        {

            var sqlQueryText = $"SELECT * FROM User WHERE User.UserUuid = '{userUuid}'";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            var queryResultSetIterator = Cosmos.User.GetItemQueryIterator<UserDataModel>(queryDefinition);
            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<UserDataModel> current = await queryResultSetIterator.ReadNextAsync();
                foreach (UserDataModel u in current)
                {
                    return new OkObjectResult(u);
                }
            }
            return new NotFoundResult();
        }
    }
}
