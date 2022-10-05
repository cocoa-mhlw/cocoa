/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;

namespace Covid19Radar.Common
{
    public static class Utils
    {
        #region Other Public Methods

        public static DateTime JstNow()
            => TimeZoneInfo.ConvertTime(DateTime.Now, AppConstants.TIMEZONE_JST);

        public static DateTime[] JstDateTimes(int days)
        {
            if (days < 0)
            {
                return new DateTime[0];
            }
            DateTime[] dateTimes = new DateTime[days];
            for (int index = 0; index < days; index++)
            {
                dateTimes[index] = TimeZoneInfo.ConvertTime(DateTime.Now.AddDays(-index), AppConstants.TIMEZONE_JST);
            }
            return dateTimes;
        }

        public static bool IsCurrentUICultureJaJp()
        {
            return "ja-JP".Equals(System.Globalization.CultureInfo.CurrentUICulture.Name);
        }

        #endregion

    }
}
