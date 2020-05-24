using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Api.Common
{
    /// <summary>
    /// Constant values for the entire application
    /// </summary>
    public static class Constants
    {
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
    }
}
