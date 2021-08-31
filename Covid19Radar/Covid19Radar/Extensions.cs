using System;
namespace Covid19Radar
{
    public static class Extensions
    {
        private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static long ToUnixEpoch(this DateTime dateTime)
        {
            TimeSpan elapsedTime = dateTime.ToUniversalTime() - UNIX_EPOCH;
            return (long)elapsedTime.TotalSeconds;
        }
    }
}
