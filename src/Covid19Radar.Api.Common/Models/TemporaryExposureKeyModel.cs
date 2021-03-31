/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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
