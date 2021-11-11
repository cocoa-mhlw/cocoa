/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Prism.Navigation;

namespace Covid19Radar
{
    public static class Extensions
    {
        public static long ToUnixEpoch(this DateTime dateTime)
        {
            TimeSpan elapsedTime = dateTime.ToUniversalTime() - DateTime.UnixEpoch;
            return (long)elapsedTime.TotalSeconds;
        }

        public static void CopyFrom(this INavigationParameters param, INavigationParameters? from)
        {
            if (from is null)
            {
                return;
            }

            foreach (var key in from.Keys)
            {
                param.Add(key, from[key]);
            }
        }
    }
}
