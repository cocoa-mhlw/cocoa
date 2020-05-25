using Covid19Radar.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Api.Services
{
    public interface INotificationService
    {
        DateTime LastNotificationTime { get; }
        IEnumerable<NotificationMessageModel> GetNotificationMessages(DateTime lastScanTime, out DateTime lastNotificationTime);
    }
}
