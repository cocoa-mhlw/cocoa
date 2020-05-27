using Covid19Radar.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Services
{
    public interface INotificationService
    {
        Task<DateTime> GetLastNotificationTimeAsync();
        Task<NotificationMessageModel[]> GetNotificationMessagesAsync(DateTime lastNotificationTime);
    }
}
