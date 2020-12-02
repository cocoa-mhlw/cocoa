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
            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("update_date")]
            public DateTime UpdateDateTime { get; set; }
        }
    }
}
