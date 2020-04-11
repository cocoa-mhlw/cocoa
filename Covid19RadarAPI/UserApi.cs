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
        

        public UserApi(ICosmos cosmos)
        {
            Cosmos = cosmos;
        }


        [FunctionName("User")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            switch (req.Method)
            {
                case "GET":
                    return await Get(req, log);
            }

            return  new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        private async Task<IActionResult> Get(HttpRequest req, ILogger log)
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
