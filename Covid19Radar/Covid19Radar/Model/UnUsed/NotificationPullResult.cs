/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Model
{
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
