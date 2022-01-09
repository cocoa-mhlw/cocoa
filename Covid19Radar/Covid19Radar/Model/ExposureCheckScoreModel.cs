// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using System.Globalization;

namespace Covid19Radar.Model
{
    public class ExposureCheckScoreModel
    {
        public double DailySummaryScoreSum { get; set; }
        public long DateMillisSinceEpoch { get; set; }

        public string DateTimeString
        {
            get
            {
                return DateTimeOffset.UnixEpoch
                    .AddMilliseconds(DateMillisSinceEpoch).UtcDateTime
                    .ToLocalTime().ToString("D", CultureInfo.CurrentCulture);
            }
        }

        public string DailySummaryScoreSumString
        {
            get
            {
                return DailySummaryScoreSum.ToString("0.00");
            }
        }
    }
}
