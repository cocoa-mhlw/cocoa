using System;

namespace Covid19Radar.Models
{
    public class OtpDocument : IOtpDocument
    {
        /// <summary>
        /// Cosmos DB document identifier
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// User identifier
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// User Uuid identifier
        /// </summary>
        public string UserUuid { get; set; }

        /// <summary>
        /// OTP Created timestamp
        /// </summary>
        public DateTime OtpCreatedTime { get; set; }
    }
}
