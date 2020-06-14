using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Model
{
    /// <summary>
    /// Notification message model
    /// </summary>
    [Table("NotificationMessage")]
    public class NotificationMessageModel
    {
        /// <summary>
        /// for Cosmos DB
        /// </summary>
        [PrimaryKey]
        public string id { get; set; }

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
