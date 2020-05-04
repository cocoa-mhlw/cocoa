using System;
using Newtonsoft.Json;

namespace Covid19Radar.Models
{
    [JsonObject("notificationPullResult")]
    public class NotificationPullResult
    {
        /// <summary>
        /// Last notification date and time
        /// </summary>
        [JsonProperty("lastNotificationTime")]
        public DateTime LastNotificationTime { get; set; }
        /// <summary>
        /// Notification Messages
        /// </summary>
        [JsonProperty("messages")]
        public NotificationMessageModel[] Messages { get; set; }
    }
}
