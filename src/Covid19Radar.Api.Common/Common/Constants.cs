/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar.Api.Common
{
    /// <summary>
    /// Constant values for the entire application
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Timestamp format - RFC 3339
        /// </summary>
        public const string FORMAT_TIMESTAMP = "yyyy-MM-dd'T'HH:mm:ss.fffzzz";

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
        /// Number of days relative that have infectiousness from the date of diagnosis or onset.
        /// </summary>
        public const int DaysHasInfectiousness = -3;

        /// <summary>
        /// Extra value when TemporaryExposureKey reoprtType missing.
        /// </summary>
        public const int ReportTypeMissingValue = -1;

        /// <summary>
        /// Extra value when TemporaryExposureKey daysSinceOnsetOfSymptoms missing.
        /// </summary>
        /// <seealso href="https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/TemporaryExposureKey#public-static-final-int-days_since_onset_of_symptoms_unknown">
        /// TemporaryExposureKey.DAYS_SINCE_ONSET_OF_SYMPTOMS_UNKNOWN
        /// </seealso>
        public const int DaysSinceOnsetOfSymptomsMissingValue = int.MaxValue;

        /// <summary>
        /// Limit to the size of an EventLog-payload.
        /// </summary>
        public const long MAX_SIZE_EVENT_LOG_PAYLOAD_BYTES = 1024 * 1024 * 10;
    }
}
