namespace Covid19Radar.Api.Models
{
    public class TemporaryExposureKeyExportModel
    {
        public string id { get; set; }
        public string PartitionKey { get; set; }
        public ulong StartTimestamp { get; set; }
        public ulong EndTimestamp { get; set; }
        public int BatchNum { get; set; }
        public int BatchSize { get; set; }
        public string Region { get; set; }
        public bool Uploaded { get; set; }
        public bool Deleted { get; set; }
        public long TimestampSecondsSinceEpoch { get; set; }
    }
}
