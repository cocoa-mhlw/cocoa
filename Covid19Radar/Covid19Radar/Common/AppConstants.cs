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
        public const int MaxErrorCount = 3;

        public const string positiveRegex = @"\b[0-9]{8}\b";

        public static class StorageKey
        {
            public const string Secret = "Secret";
            public const string UserData = "UserData";
        }
    }
}
