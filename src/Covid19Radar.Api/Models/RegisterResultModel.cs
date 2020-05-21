using Newtonsoft.Json;

namespace Covid19Radar.Api.Models
{
    [JsonObject("registerResultModel")]
    public class RegisterResultModel
    {
        /// <summary>
        /// User UUID / take care misunderstand Becon ID
        /// </summary>
        /// <value>User UUID</value>
        [JsonProperty("userUuid")]
        public string UserUuid { get; set; }

        /// <summary>
        /// Secret key
        /// </summary>
        /// <value>Secret Key</value>
        [JsonProperty("secret")]
        public string Secret { get; set; }

    }
}
