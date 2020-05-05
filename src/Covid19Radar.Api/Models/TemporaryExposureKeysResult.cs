using Covid19Radar.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
    public class TemporaryExposureKeysResult
    {
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("keys")]
        public IEnumerable<Key> Keys { get; set; }

        public class Key
        {
            public Key()
            {
            }

            public Key(byte[] keyData, DateTimeOffset rollingStart, TimeSpan rollingDuration, RiskLevel transmissionRisk)
            {
                KeyData = keyData;
                RollingStart = rollingStart;
                RollingDuration = rollingDuration;
                TransmissionRiskLevel = transmissionRisk;
            }

            internal Key(byte[] keyData, long rollingStart, TimeSpan rollingDuration, RiskLevel transmissionRisk)
            {
                KeyData = keyData;
                RollingStart = DateTimeOffset.FromUnixTimeSeconds(rollingStart * (60 * 10));
                RollingDuration = rollingDuration;
                TransmissionRiskLevel = transmissionRisk;
            }

            public byte[] KeyData { get; set; }

            public DateTimeOffset RollingStart { get; set; }

            public TimeSpan RollingDuration { get; set; }

            public RiskLevel TransmissionRiskLevel { get; set; }

            internal long RollingStartLong
                => RollingStart.ToUnixTimeSeconds() / (60 * 10);

            public static Key FromDatastore(TemporaryExposureKey key)
                => new Key(
                    Convert.FromBase64String(key.Base64KeyData),
                    DateTimeOffset.FromUnixTimeSeconds(key.RollingStartSecondsSinceEpoch),
                    TimeSpan.FromMinutes(key.RollingDuration),
                    (RiskLevel)key.TransmissionRiskLevel);

            public TemporaryExposureKey ToDatastore()
                => new TemporaryExposureKey
                {
                    Base64KeyData = Convert.ToBase64String(KeyData),
                    TimestampSecondsSinceEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    RollingStartSecondsSinceEpoch = RollingStart.ToUnixTimeSeconds(),
                    RollingDuration = (int)RollingDuration.TotalMinutes,
                    TransmissionRiskLevel = (int)TransmissionRiskLevel
                };
        }
    }
}
