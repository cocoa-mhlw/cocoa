/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using Newtonsoft.Json;

namespace Covid19Radar.Api.Models
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
