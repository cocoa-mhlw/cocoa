/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Newtonsoft.Json;

namespace Covid19Radar.Common
{
    public static class Utils
    {
        #region Other Public Methods

        public static DateTime JstNow()
        {
            return TimeZoneInfo.ConvertTime(DateTime.Now, JstTimeZoneInfo());
        }

        public static DateTime[] JstDateTimes(int days)
        {
            if (days < 0)
            {
                return new DateTime[0];
            }
            DateTime[] dateTimes = new DateTime[days];
            for (int index = 0; index < days; index++)
            {
                dateTimes[index] = TimeZoneInfo.ConvertTime(DateTime.Now.AddDays(-index), JstTimeZoneInfo());
            }
            return dateTimes;
        }

        #endregion

        #region Other Private Methods

        private static TimeZoneInfo JstTimeZoneInfo()
        {
            // iOS/Android/Unix
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
            }
            catch (TimeZoneNotFoundException)
            {
                // Not iOS/Android/Unix
            }

            // Windows
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                // Not Windows
            }

            // Emergency fallback
            return TimeZoneInfo.CreateCustomTimeZone("JST", new TimeSpan(9, 0, 0), "(GMT+09:00) JST", "JST");
        }

        #endregion
    }
}
