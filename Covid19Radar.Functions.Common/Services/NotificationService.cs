using Covid19Radar.DataStore;
using Covid19Radar.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Covid19Radar.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ICosmos Cosmos;
        private readonly ILogger<NotificationService> Logger;

        private DateTime _LastNotificationTime = DateTime.MinValue;
        private DateTime _LastNotificationCheckTime = DateTime.MinValue;
        private object LastNotificationLock = new object();
        private int LastNotificationCheckInterval;
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
    }
}
