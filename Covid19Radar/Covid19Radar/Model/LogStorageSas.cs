/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json;

namespace Covid19Radar.Model
{
    public class LogStorageSas
    {
        [JsonProperty(PropertyName = "sas_token")]
        public string SasToken { get; set; }
    }
}
