/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar.Common
{
    public static class AppConstants
    {
        /// <summary>
        /// Timestamp format - RFC 3339
        /// </summary>
        public const string FORMAT_TIMESTAMP = "yyyy-MM-dd'T'HH:mm:ss.fffzzz";

        /// <summary>
        /// Number of days covered from the date of diagnosis or onset
        /// </summary>
        public const int DaysToSendTek = -3;

        /// <summary>
        /// Max Error Count
        /// </summary>
        public const int MaxErrorCount = 3;

        /// <summary>
        /// Max Process Number length
        /// </summary>
        public const int MaxProcessingNumberLength = 8;

        public const string processingNumberRegex = @"\b[0-9]{8}\b";

        /// <summary>
        /// Number of days of exposure information to display
        /// </summary>
        public const int DaysOfExposureInformationToDisplay = -15;

        /// <summary>
        /// Message when `AppDelagate.OnActivated()` occurs on iOS.
        /// </summary>
        public const string IosOnActivatedMessage = "IosOnActivatedMessage";

        /// <summary>
        /// Key of processing-number in AppLinks(Universal Links) query parameters.
        /// </summary>
        public const string LinkQueryKeyProcessingNumber = "pn";

        /// <summary>
        /// DiagnosisApi version.
        /// (e.g. v2, v3)
        /// </summary>
        public const string DiagnosisApiVersionCode = "v2";

        /// <summary>
        /// Number of day(s) that ExposureConfiguration file downloaded cache.
        /// </summary>
        public const int ExposureConfigurationFileDownloadCacheRetentionDays = 2;

        /// <summary>
        /// Number of days that minimum applicable interval DiagnosisKeysDataMapping.
        /// </summary>
        /// <seealso href="https://developers.google.com/android/reference/com/google/android/gms/nearby/exposurenotification/ExposureNotificationClient#setDiagnosisKeysDataMapping(com.google.android.gms.nearby.exposurenotification.DiagnosisKeysDataMapping)">
        /// ExposureNotificationClient.setDiagnosisKeysDataMapping
        /// </seealso>
        public const int MinimumDiagnosisKeysDataMappingApplyIntervalDays = 7 + 1;

        /// <summary>
        /// Delay for error in TEK re-registration.
        /// </summary>
        public const int DelayForRegistrationErrorMillis = 5000;

        /// <summary>
        /// 
        /// </summary>
        public const bool DEFAULT_SEND_EVENT_LOG_ENABLED = true;
    }
}
