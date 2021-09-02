/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Diagnostics;

namespace Covid19Radar
{
    public static class Extensions
    {
        private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixEpoch(this DateTime dateTime)
        {
            TimeSpan elapsedTime = dateTime.ToUniversalTime() - UNIX_EPOCH;
            return (long)elapsedTime.TotalSeconds;
        }
    }
}
