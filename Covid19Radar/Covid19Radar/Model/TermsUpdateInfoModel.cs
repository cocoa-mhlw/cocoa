/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Newtonsoft.Json;

namespace Covid19Radar.Model
{
    public class TermsUpdateInfoModel
    {
        [JsonProperty("terms_of_service")]
        public Detail TermsOfService { get; set; }

        [JsonProperty("privacy_policy")]
        public Detail PrivacyPolicy { get; set; }

        public class Detail
        {
            private readonly TimeSpan TIME_DIFFERENCIAL_JST_UTC = TimeSpan.FromHours(+9);

            [JsonProperty("text")]
            public string Text { get; set; }

            /// <summary>
            /// Updated terms of service(or privacy policy) dateTime, that based on Japan Standard Time (UTC+9)
            /// </summary>
            [JsonProperty("update_date")]
            public DateTime UpdateDateTimeJst { get; set; }

            public DateTime UpdateDateTimeUtc
                => DateTime.SpecifyKind(UpdateDateTimeJst - TIME_DIFFERENCIAL_JST_UTC, DateTimeKind.Utc);
        }
    }
}
