/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json;

namespace Covid19Radar.Model
{
    public class DiagnosisSubmissionParameter
    {
        //[JsonProperty("symptomOnsetDate")]
        [JsonIgnore]
        public string SymptomOnsetDate { get; set; }

        [JsonProperty("keys")]
        public Key[] Keys { get; set; }
        [JsonProperty("regions")]
        public string[] Regions { get; set; }
        [JsonProperty("platform")]
        public string Platform { get; set; }
        [JsonProperty("deviceVerificationPayload")]
        public string DeviceVerificationPayload { get; set; }
        [JsonProperty("appPackageName")]
        public string AppPackageName { get; set; }
        // Some signature / code confirming authorization by the verification authority.
        [JsonProperty("verificationPayload")]
        public string VerificationPayload { get; set; }

        //[JsonProperty("idempotency_key")]
        [JsonIgnore]
        public string IdempotencyKey { get; set; }

        // Random data to obscure the size of the request network packet sniffers.
        [JsonProperty("padding")]
        public string Padding { get; set; }

        public class Key
        {
            [JsonProperty("keyData")]
            public string KeyData { get; set; }
            [JsonProperty("rollingStartNumber")]
            public uint RollingStartNumber { get; set; }
            [JsonProperty("rollingPeriod")]
            public uint RollingPeriod { get; set; }

            //[JsonProperty("reportType")]
            [JsonIgnore]
            public uint ReportType { get; set; }
        }
    }
}
