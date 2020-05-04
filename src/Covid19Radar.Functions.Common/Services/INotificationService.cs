using Covid19Radar.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Services
{
    public interface INotificationService
    {
        DateTime LastNotificationTime { get; }
        IEnumerable<NotificationMessageModel> GetNotificationMessages(DateTime lastScanTime, out DateTime lastNotificationTime);
    }
}
