/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
namespace Covid19Radar.Api.Extensions
{
    public static class ENExtensions
    {
        private const int TIME_WINDOW_IN_SEC = 60 * 10;

        // https://covid19-static.cdn-apple.com/applications/covid19/current/static/contact-tracing/pdf/ExposureNotification-CryptographySpecificationv1.2.pdf
        public static uint ToRollingStartNumber(this DateTime dateTime)
            => (uint)(dateTime.ToUnixEpochTime() / TIME_WINDOW_IN_SEC);

        public static long ToUnixEpochTime(this DateTime dateTime)
        {
            TimeSpan elapsedTime = dateTime.ToUniversalTime() - DateTime.UnixEpoch;
            return (long)elapsedTime.TotalSeconds;
        }

    }
}
