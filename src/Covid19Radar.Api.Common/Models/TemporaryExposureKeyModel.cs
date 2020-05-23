using Covid19Radar.Api.Common;
using Covid19Radar.Api.Protobuf;
using Google.Protobuf;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Api.Models
{
    public class TemporaryExposureKeyModel
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string PartitionKey { get; set; }
        public byte[] KeyData { get; set; }
        public int RollingPeriod { get; set; }
        public int RollingStartIntervalNumber { get; set; }
        public int TransmissionRiskLevel { get; set; }
        public string Region { get; set; }

        public long RollingStartUnixTimeSeconds { get => DateTimeOffset.FromUnixTimeSeconds(RollingStartIntervalNumber * 10 * 60).ToUnixTimeSeconds(); }
        public long RollingPeriodSeconds { get => RollingPeriod * 10 * 60; }

        public ulong Timestamp { get; set; }
        public string ExportId { get; set; }

        public TemporaryExposureKey ToKey()
        {
            return new TemporaryExposureKey()
            {
                KeyData = ByteString.CopyFrom(KeyData),
                RollingStartIntervalNumber = RollingStartIntervalNumber,
                RollingPeriod = RollingPeriod,
                TransmissionRiskLevel = TransmissionRiskLevel
            };
        }
    }

}
