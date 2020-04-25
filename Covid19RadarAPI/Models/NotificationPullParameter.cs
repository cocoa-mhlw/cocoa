using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{

    public class NotificationPullParameter : IUser
    {
        /// <summary>
        /// User UUID / take care misunderstand Becon ID
        /// </summary>
        /// <value>User UUID</value>
        public string UserUuid { get; set; }
        /// <summary>
        /// Major - in this app case mapping user id
        /// </summary>
        /// <value>BLE major number</value>
        public string UserMajor { get; set; }
        /// <summary>
        /// MInor - in this app case mapping user id
        /// </summary>
        /// <value>BLE minor number</value>
        public string UserMinor { get; set; }

        /// <summary>
        /// Last notification date and time
        /// </summary>
        public DateTime LastNotificationTime { get; set; }

        string IUser.Major => UserMajor;

        string IUser.Minor => UserMinor;
    }
}
