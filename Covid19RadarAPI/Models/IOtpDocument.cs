using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
    public interface IOtpDocument
    {
        /// <summary>
        /// Cosmos DB document identifier
        /// </summary>
        string id { get; set; }

        /// <summary>
        /// User identifier
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// User Uuid identifier
        /// </summary>
        string UserUuid { get; set; }
        
        /// <summary>
        /// OTP Created timestamp
        /// </summary>
        DateTime OtpCreatedTime { get; set; }

    }
}
