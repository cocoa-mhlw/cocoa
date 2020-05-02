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
using Covid19Radar.DataAccess;

#nullable enable

namespace Covid19Radar.Api
{
    public class UserApi
    {
        private readonly IUserRepository UserRepository;
        private readonly INotificationService Notification;
        private readonly IInfectionService Infection;
        private readonly IValidationUserService Validation;
        private readonly ILogger<UserApi> Logger;

        public UserApi(
            IUserRepository userRepository,
            INotificationService notification,
            IInfectionService infection,
            IValidationUserService validation,
            ILogger<UserApi> logger)
        {
            UserRepository = userRepository;
            Notification = notification;
            Infection = infection;
            Validation = validation;
            Logger = logger;
        }

        [FunctionName(nameof(UserApi))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "user/{userUuid}/{major}/{minor}")] HttpRequest req,
            string userUuid,
            string major,
            string minor)
        {
            Logger.LogInformation("C# HTTP trigger function processed a request.");

            var user = new UserParameter() { UserUuid = userUuid, Major = major, Minor = minor };

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
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "User")] HttpRequest req)
        {
            Logger.LogInformation("C# HTTP trigger function processed a request.");

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
                var userResult = await UserRepository.GetById(user.GetId());
                if (userResult != null)
                {
                    userResult.LastNotificationTime = Notification.LastNotificationTime;
                    userResult.LastInfectionUpdateTime = Infection.LastUpdateTime;
                    return new OkObjectResult(userResult);
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
