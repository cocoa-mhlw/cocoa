using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Api.External.Models
{
    public class NotificationCreateParameter
    {
        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }
    }
}
