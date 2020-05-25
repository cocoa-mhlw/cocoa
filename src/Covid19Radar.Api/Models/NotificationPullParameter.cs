using System;
using Newtonsoft.Json;

namespace Covid19Radar.Api.Models
{
    [JsonObject("notificationPullParameter")]
    public class NotificationPullParameter : IUser
    {
        /// <summary>
        /// User UUID / take care misunderstand Becon ID
        /// </summary>
        /// <value>User UUID</value>
        [JsonProperty("userUuid")]
        public string UserUuid { get; set; }
        /// <summary>
        /// Last notification date and time
        /// </summary>
        [JsonProperty("lastNotificationTime")]
        public DateTime LastNotificationTime { get; set; }

    }
}
