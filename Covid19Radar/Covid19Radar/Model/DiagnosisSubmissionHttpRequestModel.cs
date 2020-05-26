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
            public long RollingStart  { get; set; }
            [JsonProperty("rollingPeriod ")]
            public int RollingDuration  { get; set; }
            [JsonProperty("transmissionRisk")]
            public int TransmissionRisk { get; set; }
            public static Key FromTemporaryExposureKey(TemporaryExposureKey key)
            {
                return new Key()
                {
                    KeyData = Convert.ToBase64String(key.Key),
                    RollingStart  = (long)(key.RollingStart - DateTime.UnixEpoch).TotalMinutes / 10,
                    RollingDuration = (int)(key.RollingDuration.TotalMinutes / 10),
                    TransmissionRisk = (int)key.TransmissionRiskLevel
                };
            }
        }
    }

}
