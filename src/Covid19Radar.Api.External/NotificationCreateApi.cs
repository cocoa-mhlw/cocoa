using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Covid19Radar.Api.External.Models;

namespace Covid19Radar.Api.External
{
    public class NotificationCreateApi
    {
        private readonly ICosmos Cosmos;
        private readonly INotificationService Notification;
        private readonly ILogger<NotificationCreateApi> Logger;

        public NotificationCreateApi(
            ICosmos cosmos,
            INotificationService notification,
            ILogger<NotificationCreateApi> logger)
        {
            Cosmos = cosmos;
            Notification = notification;
            Logger = logger;
        }

        [FunctionName(nameof(NotificationCreateApi))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Notification/Create")] HttpRequest req)
        {
            Logger.LogInformation($"{nameof(NotificationCreateApi)} processed a request.");
            return await Post(req);
        }

        private async Task<IActionResult> Post(HttpRequest req)
        {
            // convert Postdata to BeaconDataModel
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var param = JsonConvert.DeserializeObject<NotificationCreateParameter>(requestBody);
            if (string.IsNullOrWhiteSpace(param.Title) || string.IsNullOrWhiteSpace(param.Message))
            {
                return new BadRequestObjectResult("Title or Message is missing");
            }

            var now = DateTime.UtcNow;
            var newNotification = new NotificationMessageModel();
            newNotification.Title = param.Title;
            newNotification.Message = param.Message;
            newNotification.Created = now;
            newNotification.id = Guid.NewGuid().ToString("N");
            var createResult = await Cosmos.Notification.CreateItemAsync(newNotification);
            var result = new NotificationCreateResult() { Message = createResult.Resource };
            return new OkObjectResult(result);
        }
    }
}
