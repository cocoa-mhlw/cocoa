using System;
using Newtonsoft.Json;

namespace Covid19Radar.Models
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
        /// Major - in this app case mapping user id
        /// </summary>
        /// <value>BLE major number</value>
        [JsonProperty("userMajor")]
        public string UserMajor { get; set; }
        /// <summary>
        /// MInor - in this app case mapping user id
        /// </summary>
        /// <value>BLE minor number</value>
        [JsonProperty("userMinor")]
        public string UserMinor { get; set; }

        /// <summary>
        /// Last notification date and time
        /// </summary>
        [JsonProperty("lastNotificationTime")]
        public DateTime LastNotificationTime { get; set; }

        string IUser.Major => UserMajor;

        string IUser.Minor => UserMinor;
    }
}
