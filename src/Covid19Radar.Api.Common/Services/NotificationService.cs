using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using Covid19Radar.Api.Common;
using System.Reflection.Metadata;

namespace Covid19Radar.Api.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ICosmos Cosmos;
        private readonly ILogger<NotificationService> Logger;

        private QueryCache<DateTime> LastNotificationTimeQuery = new QueryCache<DateTime>(Constants.CacheTimeout);
        private QueryCache<NotificationMessageModel[]> MessagesQuery = new QueryCache<NotificationMessageModel[]>(Constants.CacheTimeout);

        public NotificationService(
            Microsoft.Extensions.Configuration.IConfiguration config,
            ICosmos cosmos,
            ILogger<NotificationService> logger)
        {
            this.Cosmos = cosmos;
            this.Logger = logger;
        }

        public async Task<DateTime> GetLastNotificationTimeAsync()
        {
            return await LastNotificationTimeQuery.QueryWithCacheAsync(async () =>
            {
                var q = new QueryDefinition("SELECT * FROM c ORDER BY c.Created DESC");
                var opt = new QueryRequestOptions();
                opt.MaxItemCount = 1;
                var iterator = Cosmos.Notification.GetItemQueryIterator<NotificationMessageModel>(q, null, opt);
                var result = await iterator.ReadNextAsync();
                return result.Resource.FirstOrDefault()?.Created ?? DateTime.MinValue;
            });
        }

        public async Task<NotificationMessageModel[]> GetNotificationMessagesAsync(DateTime lastNotificationTime)
        {
            var result = await MessagesQuery.QueryWithCacheAsync(async () =>
            {
                var q = new QueryDefinition("SELECT * FROM c ORDER BY c.Created DESC");
                var opt = new QueryRequestOptions();
                var iterator = Cosmos.Notification.GetItemQueryIterator<NotificationMessageModel>(q, null, opt);
                var resultCollection = new List<NotificationMessageModel>();
                while (iterator.HasMoreResults)
                {
                    var result = await iterator.ReadNextAsync();
                    resultCollection.AddRange(result.Resource);
                }
                return resultCollection.ToArray();
            });
            return result.Where(_ => _.Created >= lastNotificationTime).ToArray();
        }
    }
}
