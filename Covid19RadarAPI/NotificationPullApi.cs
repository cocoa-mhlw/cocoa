using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Covid19Radar.DataStore;
using Covid19Radar.Models;
using Microsoft.Azure.Cosmos;
using Covid19Radar.Services;

namespace Covid19Radar
{
    public class NotificationPullApi
    {
        private readonly ICosmos Cosmos;
        private readonly INotificationService Notification;
        private readonly ILogger<NotificationPullApi> Logger;
        public NotificationPullApi(
            ICosmos cosmos,
            INotificationService notification,
            ILogger<NotificationPullApi> logger)
        {
            Cosmos = cosmos;
            Notification = notification;
            Logger = logger;
        }

        [FunctionName(nameof(NotificationPullApi))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Notification/Pull")] HttpRequest req)
        {
            Logger.LogInformation($"{nameof(NotificationPullApi)} processed a request.");

            switch (req.Method)
            {
                case "POST":
                    return await Post(req);
            }
            AddBadRequest(req);
            return new BadRequestObjectResult("Not Supported");
        }

        private async Task<IActionResult> Post(HttpRequest req)
        {
            // convert Postdata to BeaconDataModel
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var param = JsonConvert.DeserializeObject<NotificationPullParameter>(requestBody);

            // validation
            if (string.IsNullOrWhiteSpace(param.UserUuid)
                || string.IsNullOrWhiteSpace(param.UserMajor)
                || string.IsNullOrWhiteSpace(param.UserMinor))
            {
                AddBadRequest(req);
                return new BadRequestObjectResult("");
            }
            var queryResult = await Query(req, param);
            if (queryResult != null)
            {
                return queryResult;
            }

            // Query to Notification Service.
            return GetMessages(param);
        }

        private async Task<IActionResult> Query(HttpRequest req, IUser user)
        {
            try
            {
                var itemResult = await Cosmos.User.ReadItemAsync<UserResultModel>(user.GetId(), PartitionKey.None);
                if (itemResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return null;
                }
            }
            catch (CosmosException ex)
            {
                // 429-TooManyRequests
                if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    return new StatusCodeResult(503);
                }
            }
            AddBadRequest(req);
            return new BadRequestObjectResult("");
        }

        private IActionResult GetMessages(NotificationPullParameter param)
        {
            var result = new NotificationPullResult();
            DateTime lastNotificationTime;
            result.Messages = Notification.GetNotificationMessages(param.LastNotificationTime, out lastNotificationTime).ToArray();
            result.LastNotificationTime = lastNotificationTime;
            return new OkObjectResult(result);
        }


        private void AddBadRequest(HttpRequest req)
        {
            // add deny list
        }
    }
}
