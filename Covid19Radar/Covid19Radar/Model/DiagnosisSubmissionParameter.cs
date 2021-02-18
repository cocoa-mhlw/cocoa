using Newtonsoft.Json;

namespace Covid19Radar.Model
{
    public class DiagnosisSubmissionParameter
    {
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
        }
    }
}
