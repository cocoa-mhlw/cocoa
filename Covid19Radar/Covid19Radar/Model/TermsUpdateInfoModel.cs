/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Newtonsoft.Json;
using TimeZoneConverter;

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
            private readonly TimeZoneInfo TIMEZONE_JST = TZConvert.GetTimeZoneInfo("ASIA/Tokyo");

            [JsonProperty("text")]
            public string Text { get; set; }

            /// <summary>
            /// Updated terms of service(or privacy policy) dateTime, that based on Japan Standard Time (UTC+9)
            /// </summary>
            [JsonProperty("update_date")]
            public DateTime UpdateDateTimeJst { get; set; }

            public DateTime UpdateDateTimeUtc
                => TimeZoneInfo.ConvertTimeToUtc(
                        DateTime.SpecifyKind(UpdateDateTimeJst, DateTimeKind.Unspecified),
                        TIMEZONE_JST
                        );
        }
    }
}
