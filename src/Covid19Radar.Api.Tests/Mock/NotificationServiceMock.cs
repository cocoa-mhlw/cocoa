using Covid19Radar.Models;
using Covid19Radar.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Covid19Radar.Tests.Mock
{
    public class NotificationServiceMock : INotificationService
    {
        public DateTime LastNotificationTime { get; set; } = DateTime.MinValue;

        public IEnumerable<NotificationMessageModel> GetNotificationMessages(DateTime lastScanTime, out DateTime lastNotificationTime)
        {
            lastNotificationTime = DateTime.MinValue;
            return Enumerable.Empty<NotificationMessageModel>();
        }
    }
}
