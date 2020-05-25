using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.ExposureNotifications;

namespace Covid19Radar.Model
{
    public class DiagnosisSubmissionHttpRequestModel
    {
        [JsonProperty("submissionNumber")]
        public string SubmissionNumber { get; set; }
        [JsonProperty("userUuid")]
        public string UserUuid { get; set; }
        [JsonProperty("keys")]
        public Key[] Keys { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("platform")]
        public string Platform { get; set; }
        [JsonProperty("deviceVerificationPayload")]
        public string DeviceVerificationPayload { get; set; }
        [JsonProperty("appPackageName")]
        public string AppPackageName { get; set; }
        public class Key
        {
            [JsonProperty("keyData")]
            public string KeyData { get; set; }
            [JsonProperty("rollingStartNumber")]
            public uint RollingStartNumber { get; set; }
            [JsonProperty("rollingPeriod ")]
            public uint RollingPeriod { get; set; }
            [JsonProperty("transmissionRisk")]
            public int TransmissionRisk { get; set; }
            public static Key FromTemporaryExposureKey(TemporaryExposureKey key)
            {
                return new Key()
                {
                    //KeyData = Convert.ToBase64String(key.Key),
                    RollingStartNumber = (uint)(key.RollingStart.ToUnixTimeSeconds() / 60 / 10),
                    RollingPeriod = (uint)(key.RollingDuration.TotalSeconds / 60 / 10),
                    TransmissionRisk = (int)key.TransmissionRiskLevel
                };
            }
        }
    }

}
