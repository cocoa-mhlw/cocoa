using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Services
{
    public interface INotificationService
    {
        EventHandler NotificationReceived { get; set; }
        void Initialize();
        int ScheduleNotification(string title, string message);
        void ReceiveNotification(string title, string message);
    }
}
