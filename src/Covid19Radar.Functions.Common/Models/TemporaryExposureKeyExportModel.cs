using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
    public class TemporaryExposureKeyExportModel
    {
        public string id { get; set; }
        public string Url { get; set; }
        public ulong startTimestamp { get; set; }
        public ulong endTimestamp { get; set; }
        public long TimestampSecondsSinceEpoch { get; set; }
    }
}
