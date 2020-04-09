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
    public class UserApi
    {

        private ICosmos _Cosmos;

        public UserApi(ICosmos cosmos)
        {
            _Cosmos = cosmos;
        }


        [FunctionName("User")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            switch (req.Method)
            {
                case "POST":
                    return await Post(req, log);
                case "GET":
                    return await Get(req, log);
            }

            return  new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        private async Task<IActionResult> Get(HttpRequest req, ILogger log)
        {
            // get name from query 
            string name = req.Query["name"];

            // get UserData from DB
            await Task.CompletedTask;
            var result = new UserDataModel();

            return (ActionResult)new OkObjectResult(result);
        }

        private async Task<IActionResult> Post(HttpRequest req, ILogger log)
        {
            // convert Postdata to UserDataModel
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<UserDataModel>(requestBody);

            // save to DB
            await Task.CompletedTask;
            var result = new ResultModel();
            return (ActionResult)new OkObjectResult(ResultModel.Success);
        }
    }
}
