using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Covid19Radar.Models;
using Covid19Radar.DataStore;
using Covid19Radar.Services;

namespace Covid19Radar.Api
{
    public class UserApi
    {

        private readonly ICosmos Cosmos;
        private readonly INotificationService Notification;
        private readonly IValidationUserService Validation;
        private readonly ILogger<UserApi> Logger;

        public UserApi(ICosmos cosmos,
            INotificationService notification,
            IValidationUserService validation,
            ILogger<UserApi> logger)
        {
            Cosmos = cosmos;
            Notification = notification;
            Validation = validation;
            Logger = logger;
        }


        [FunctionName(nameof(UserApi))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/{UserUuid}/{Major}/{Minor}")] HttpRequest req,
            string userUuid,
            string major,
            string minor)
        {
            Logger.LogInformation("C# HTTP trigger function processed a request.");

            switch (req.Method)
            {
                case "GET":
                    return await Get(req, new UserParameter() { UserUuid = userUuid, Major = major, Minor = minor });
            }

            AddBadRequest(req);
            return new BadRequestObjectResult("");
        }

        private async Task<IActionResult> Get(HttpRequest req, UserParameter user)
        {
            // validation
            var validationResult = await Validation.ValidateAsync(req, user);
            if (!validationResult.IsValid)
            {
                AddBadRequest(req);
                return validationResult.ErrorActionResult;
            }

            // query
            return await Query(req, user);
        }

        [FunctionName(nameof(UserApi) + "Post")]
        public async Task<IActionResult> RunPost(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "User")] HttpRequest req)
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
            var validationResult = await Validation.ValidateAsync(req, user);
            if (!validationResult.IsValid)
            {
                AddBadRequest(req);
                return validationResult.ErrorActionResult;
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
                    itemResult.Resource.LastNotificationTime = Notification.LastNotificationTime;
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
