using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Covid19Radar.Api.Models
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
        /// Title
        /// </summary>
        public string Title { get; set; }
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
