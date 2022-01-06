﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Api.Common;

namespace Covid19Radar.Api.Models
{
    public class TemporaryExposureKeyModel
    {
        public const int TIME_WINDOW_IN_SEC = 60 * 10;

        public string id { get; set; } = Guid.NewGuid().ToString("N");
        public string PartitionKey { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; } = string.Empty;
        public byte[] KeyData { get; set; }
        public int RollingPeriod { get; set; }
        public int RollingStartIntervalNumber { get; set; }
        public int TransmissionRiskLevel { get; set; }
        public int ReportType { get; set; } = Constants.ReportTypeMissingValue;
        public int DaysSinceOnsetOfSymptoms { get; set; } = Constants.DaysSinceOnsetOfSymptomsMissingValue;
        public long GetRollingStartUnixTimeSeconds() => RollingStartIntervalNumber * TIME_WINDOW_IN_SEC;
        public long GetRollingPeriodSeconds() => RollingPeriod * TIME_WINDOW_IN_SEC;
        public ulong Timestamp { get; set; }
        public bool Exported { get; set; }

        public TemporaryExposureKeyModel Copy()
        {
            return new TemporaryExposureKeyModel()
            {
                id = id,
                PartitionKey = PartitionKey,
                Region = Region,
                SubRegion = SubRegion,
                KeyData = KeyData,
                RollingPeriod = RollingPeriod,
                RollingStartIntervalNumber = RollingStartIntervalNumber,
                TransmissionRiskLevel = TransmissionRiskLevel,
                ReportType = ReportType,
                DaysSinceOnsetOfSymptoms = DaysSinceOnsetOfSymptoms,
                Timestamp = Timestamp,
                Exported = Exported,
            };
        }

        public bool HasValidDaysSinceOnsetOfSymptoms()
        {
            if (DaysSinceOnsetOfSymptoms == Constants.DaysSinceOnsetOfSymptomsMissingValue)
            {
                return true;
            }

            if (DaysSinceOnsetOfSymptoms >= Constants.MinDaysSinceOnsetOfSymptoms
                && DaysSinceOnsetOfSymptoms <= Constants.MaxDaysSinceOnsetOfSymptoms)
            {
                return true;
            }

            return false;
        }

    }
}
