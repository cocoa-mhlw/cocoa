using Newtonsoft.Json;

namespace Covid19Radar.Models
{
    [JsonObject("userParameter")]
    public class UserParameter: IUser
    {
        /// <summary>
        /// User UUID / take care misunderstand Becon ID
        /// </summary>
        /// <value>User UUID</value>
        [JsonProperty("userUuid")]
        public string UserUuid { get; set; }

    }
}
