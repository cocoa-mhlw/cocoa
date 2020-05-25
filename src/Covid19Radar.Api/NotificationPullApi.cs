using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

#nullable enable

namespace Covid19Radar.Api
{
    public class NotificationPullApi
    {
        private readonly IUserRepository UserRepository;
        private readonly INotificationService Notification;
        private readonly ILogger<NotificationPullApi> Logger;

        public NotificationPullApi(
            IUserRepository userRepository,
            INotificationService notification,
            ILogger<NotificationPullApi> logger)
        {
            UserRepository = userRepository;
            Notification = notification;
            Logger = logger;
        }

        [FunctionName(nameof(NotificationPullApi))]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get",
                         Route = "notification/pull/{lastClientUpdateTime:datetime}")]
            HttpRequest req,
            DateTime lastClientUpdateTime)
        {
            Logger.LogInformation($"{nameof(NotificationPullApi)} processed a request.");

            // Query to Notification Service.
            return GetMessages(lastClientUpdateTime);
        }

        private IActionResult GetMessages(DateTime lastClientUpdateTime)
        {
            var result = new NotificationPullResult();
            DateTime lastNotificationTime;
            result.Messages = Notification.GetNotificationMessages(lastClientUpdateTime, out lastNotificationTime).ToArray();
            result.LastNotificationTime = lastNotificationTime;
            return new OkObjectResult(result);
        }

    }
}
