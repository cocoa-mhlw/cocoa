/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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
