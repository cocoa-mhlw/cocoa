using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{

    public class NotificationPullResult
    {
        /// <summary>
        /// Last notification date and time
        /// </summary>
        public DateTime LastNotificationTime { get; set; }
        /// <summary>
        /// Notification Messages
        /// </summary>
        public NotificationMessageModel[] Messages { get; set; }
    }
}
