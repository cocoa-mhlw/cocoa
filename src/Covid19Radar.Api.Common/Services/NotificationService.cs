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

namespace Covid19Radar.Api.Services
{
    public class NotificationService : INotificationService, IEqualityComparer<NotificationMessageModel>
    {
        private readonly ICosmos Cosmos;
        private readonly ILogger<NotificationService> Logger;

        private DateTime _LastNotificationTime = DateTime.MinValue;
        private DateTime _LastNotificationCheckTime = DateTime.MinValue;
        private object LastNotificationLock = new object();
        private int LastNotificationCheckInterval;

        private DateTime LastNotificationGetTime = DateTime.MinValue;
        private object LastNotificationMessageLock = new object();
        private NotificationMessageModel[] Messages = new NotificationMessageModel[] { };

        public DateTime LastNotificationTime
        {
            get
            {
                RefreshNotificationTime();
                return _LastNotificationTime;
            }
        }

        public NotificationService(
            Microsoft.Extensions.Configuration.IConfiguration config,
            ICosmos cosmos,
            ILogger<NotificationService> logger)
        {
            this.Cosmos = cosmos;
            this.Logger = logger;
            this.LastNotificationCheckInterval = Convert.ToInt32(config.GetSection("LAST_NOTIFICATION_CHECK_INTERVAL").Value);
        }

        public IEnumerable<NotificationMessageModel> GetNotificationMessages(DateTime lastScanTime, out DateTime lastNotificationTime)
        {
            lastNotificationTime = LastNotificationTime;
            if (lastNotificationTime > LastNotificationGetTime)
            {
                lock (LastNotificationMessageLock)
                {
                    if (lastNotificationTime > LastNotificationGetTime)
                    {
                        var qTask = QueryNotificationMessages(LastNotificationGetTime);
                        qTask.Wait();
                        LastNotificationGetTime = lastNotificationTime;
                        Messages = qTask.Result.Concat(Messages)
                            .Distinct(this)
                            .OrderByDescending(_ => _.Created).ToArray();
                    }
                }
            }
            return Messages.Where(_ => _.Created > lastScanTime);
        }

        private void RefreshNotificationTime()
        {
            if ((DateTime.UtcNow - _LastNotificationCheckTime).TotalSeconds > LastNotificationCheckInterval)
            {
                lock (LastNotificationLock)
                {
                    if ((DateTime.UtcNow - _LastNotificationCheckTime).TotalSeconds > LastNotificationCheckInterval)
                    {
                        var qTask = QueryLastNotificationTime();
                        qTask.Wait();
                        _LastNotificationTime = qTask.Result;
                        _LastNotificationCheckTime = DateTime.UtcNow;
                    }
                }
            }
        }

        private async Task<DateTime> QueryLastNotificationTime()
        {
            var q = new QueryDefinition("SELECT * FROM c ORDER BY c.Created DESC");
            var opt = new QueryRequestOptions();
            opt.MaxItemCount = 1;
            var iterator = Cosmos.Notification.GetItemQueryIterator<NotificationMessageModel>(q, null, opt);
            var result = await iterator.ReadNextAsync();
            return result.Resource.FirstOrDefault()?.Created ?? DateTime.MinValue;
        }

        private async Task<NotificationMessageModel[]> QueryNotificationMessages(DateTime lastScanTime)
        {
            var q = new QueryDefinition("SELECT * FROM c WHERE c.Created >= @Created ORDER BY c.Created DESC");
            q.WithParameter("@Created", lastScanTime);
            var opt = new QueryRequestOptions();
            var iterator = Cosmos.Notification.GetItemQueryIterator<NotificationMessageModel>(q, null, opt);
            var resultCollection = new List<NotificationMessageModel>();
            while (iterator.HasMoreResults)
            {
                var result = await iterator.ReadNextAsync();
                resultCollection.AddRange(result.Resource);
            }
            return resultCollection.ToArray();
        }

        
        public int GetHashCode([DisallowNull] NotificationMessageModel obj)
        {
            return (obj.id + obj.PartitionKey).GetHashCode();
        }

        bool IEqualityComparer<NotificationMessageModel>.Equals(NotificationMessageModel x, NotificationMessageModel y)
        {
            return x.id == y.id;
        }
    }
}
