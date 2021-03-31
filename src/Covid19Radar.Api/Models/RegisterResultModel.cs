/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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
