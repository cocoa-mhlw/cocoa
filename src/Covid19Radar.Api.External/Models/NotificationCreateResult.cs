using Covid19Radar.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Api.External.Models
{
    public class NotificationCreateResult
    {
        /// <summary>
        /// Message
        /// </summary>
        public NotificationMessageModel Message { get; set; }
    }
}
