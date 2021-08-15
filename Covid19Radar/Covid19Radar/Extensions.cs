using System;
using Chino;

namespace Covid19Radar
{
    public static class Extensions
    {
        private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        // https://covid19-static.cdn-apple.com/applications/covid19/current/static/contact-tracing/pdf/ExposureNotification-CryptographySpecificationv1.2.pdf
        public static int ToEnInterval(this DateTime dateTime)
            => (int)(dateTime.ToUnixEpochTime() / (60 * 10));

        public static long ToUnixEpochTime(this DateTime dateTime)
        {
            TimeSpan elapsedTime = dateTime.ToUniversalTime() - UNIX_EPOCH;
            return (long)elapsedTime.TotalSeconds;
        }

        public static long GetRollingStartIntervalNumberAsUnixTimeInSec(this TemporaryExposureKey temporaryExposureKey)
            => temporaryExposureKey.RollingStartIntervalNumber * (60 * 10);

        public static DateTime GetDateTime(this DailySummary dailySummary)
            => DateTimeOffset.UnixEpoch.AddMilliseconds(dailySummary.DateMillisSinceEpoch).UtcDateTime;

        public static DateTime GetDateTime(this ExposureWindow exposureWindow)
            => DateTimeOffset.UnixEpoch.AddMilliseconds(exposureWindow.DateMillisSinceEpoch).UtcDateTime;
    }
}
