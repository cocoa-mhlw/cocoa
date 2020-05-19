using Covid19Radar.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
    public class TemporaryExposureKeyExportModel
    {
        public string id { get; set; }
        public ulong StartTimestamp { get; set; }
        public ulong EndTimestamp { get; set; }
        public int BatchNum { get; set; }
        public int BatchSize { get; set; }
        public string Region { get; set; }
        public SignatureInfo[] SignatureInfos { get; set; }
        public long TimestampSecondsSinceEpoch { get; set; }
    }
}
