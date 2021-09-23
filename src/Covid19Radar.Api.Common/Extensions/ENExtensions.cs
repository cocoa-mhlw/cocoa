using System;
namespace Covid19Radar.Api.Extensions
{
    public static class ENExtensions
    {
        private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private const int TIME_WINDOW_IN_SEC = 60 * 10;

        // https://covid19-static.cdn-apple.com/applications/covid19/current/static/contact-tracing/pdf/ExposureNotification-CryptographySpecificationv1.2.pdf
        public static uint ToRollingStartNumber(this DateTime dateTime)
            => (uint)(dateTime.ToUnixEpochTime() / TIME_WINDOW_IN_SEC);

        public static long ToUnixEpochTime(this DateTime dateTime)
        {
            TimeSpan elapsedTime = dateTime.ToUniversalTime() - UNIX_EPOCH;
            return (long)elapsedTime.TotalSeconds;
        }

    }
}
