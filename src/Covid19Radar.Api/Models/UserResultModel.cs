using System;
using Covid19Radar.Common;
using Newtonsoft.Json;

namespace Covid19Radar.Models
{
    [JsonObject("userResultModel")]
    public class UserResultModel
    {
        /// <summary>
        ///  Status Contactd,OnSet,Suspected,Inspection,Infection
        /// </summary>
        /// <value></value>
        private UserStatus _UserStatus;
        [JsonProperty("userStatus")]
        public string UserStatus
        {
            get
            {
                return Enum.GetName(typeof(Covid19Radar.Common.UserStatus), _UserStatus);
            }
            set
            {
                _UserStatus = Enum.Parse<Covid19Radar.Common.UserStatus>(value);
            }
        }
        /// <summary>
        /// set SserStatus
        /// </summary>
        /// <param name="s">UserStatus</param>
        public void SetStatus(UserStatus s)
        {
            _UserStatus = s;
        }
        /// <summary>
        /// Last notification date and time
        /// </summary>
        [JsonProperty("lastNotificationTime")]
        public DateTime LastNotificationTime { get; set; }
        /// <summary>
        /// Last Infection update date and time
        /// </summary>
        [JsonProperty("lastInfectionUpdateTime")]
        public DateTime LastInfectionUpdateTime { get; set; }
    }
}
