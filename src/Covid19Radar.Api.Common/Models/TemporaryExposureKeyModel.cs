using System;

namespace Covid19Radar.Api.Models
{
    public class TemporaryExposureKeyModel
    {
        public string id { get; set; } = Guid.NewGuid().ToString("N");
        public string PartitionKey { get; set; }
        public byte[] KeyData { get; set; }
        public int RollingPeriod { get; set; }
        public int RollingStartIntervalNumber { get; set; }
        public int TransmissionRiskLevel { get; set; }
        public long GetRollingStartUnixTimeSeconds() => RollingStartIntervalNumber * 10 * 60;
        public long GetRollingPeriodSeconds() => RollingPeriod * 10 * 60;

        public ulong Timestamp { get; set; }
        public bool Exported { get; set; }
    }

}
