using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Common
{
    public static class AppConstants
    {
        public static readonly int NumberOfGroup = 86400;
        /// <summary>
        /// Number of days not processed or deleted
        /// </summary>
        public const int OutOfDateDays = -14;
        /// <summary>
        /// Cache Timeout
        /// </summary>
        public const int CacheTimeout = 60;
        /// <summary>
        /// Active Rolling Period
        /// </summary>
        public const uint ActiveRollingPeriod = 144;
        /// <summary>
        /// Max Error Count
        /// </summary>
        public const int MaxErrorCount = 4;

    }
}
