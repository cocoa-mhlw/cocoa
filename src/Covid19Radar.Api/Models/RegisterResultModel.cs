using Newtonsoft.Json;

namespace Covid19Radar.Api.Models
{
    [JsonObject("registerResultModel")]
    public class RegisterResultModel
    {
        /// <summary>
        /// User Internal UUID
        /// </summary>
        /// <value>User Internal UUID</value>
        [JsonProperty("userUuid")]
        public string UserUuid { get; set; }

        /// <summary>
        /// API Secret key
        /// </summary>
        /// <value>Secret Key</value>
        [JsonProperty("secret")]
        public string Secret { get; set; }

        /// <summary>
        /// Jump Consistent Hash Key
        /// </summary>
        /// <value>Jump Consistent Hash</value>
        [JsonProperty("jumpConsistentSeed")]
        public ulong JumpConsistentSeed { get; set; }

    }
}
