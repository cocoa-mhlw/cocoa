using Covid19Radar.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Tests.Mock
{
    public class NotificationServiceMock : INotificationService
    {
        public DateTime LastNotificationTime { get; set; } = DateTime.MinValue;
    }
}
