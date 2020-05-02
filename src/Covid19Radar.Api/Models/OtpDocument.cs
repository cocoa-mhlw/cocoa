using System;

#nullable enable

namespace Covid19Radar.Models
{
    public class OtpDocument
    {
        public OtpDocument(string id, string userId, string userUuid, DateTime otpCreatedTime, string otp)
        {
            this.id = id;
            UserId = userId;
            UserUuid = userUuid;
            OtpCreatedTime = otpCreatedTime;
            Otp = otp;
        }

        /// <summary>
        /// Cosmos DB document identifier
        /// </summary>
        public string id { get; }

        /// <summary>
        /// User identifier
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// User Uuid identifier
        /// </summary>
        public string UserUuid { get; }

        /// <summary>
        /// OTP Created timestamp
        /// </summary>
        public DateTime OtpCreatedTime { get; }

        /// <summary>
        /// The OTP
        /// </summary>
        public string Otp { get; }
    }
}
