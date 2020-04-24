using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
    /// <summary>
    /// Notification message model
    /// </summary>
    public class NotificationMessageModel
    {
        /// <summary>
        /// for Cosmos DB
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// PartitionKey
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Created DateTime
        /// </summary>
        public DateTime Created { get; set; }
    }
}
