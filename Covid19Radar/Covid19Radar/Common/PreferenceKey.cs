/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar.Common
{
    public static class PreferenceKey
    {
        // for UserDataRepository
        public static string AppVersion = "AppVersion";
        public static string LastProcessTekTimestamp = "LastProcessTekTimestamp";
        public static string ExposureNotificationConfiguration = "ExposureNotificationConfiguration";

        public static string StartDateTimeEpoch = "StartDateTimeEpoch";
        public static string TermsOfServiceLastUpdateDateTimeEpoch = "TermsOfServiceLastUpdateDateTimeEpoch";
        public static string PrivacyPolicyLastUpdateDateTimeEpoch = "PrivacyPolicyLastUpdateDateTimeEpoch";

        public static string CanConfirmExposure = "CanConfirmExposure";
        public static string IsMaxPerDayExposureDetectionAPILimitReached = "IsMaxPerDayExposureDetectionAPILimitReached";
        public static string LastConfirmedDateTimeEpoch = "LastConfirmedDateTimeEpoch";

        public static string SendEventLogState = "SendEventLogState";

        public const string DailySummaries = "DailySummaries";
        public const string ExposureWindows = "ExposureWindows";

        public const string EndOfServiceNotificationNextSchedule = "EndOfServiceNotificationNextSchedule";
        public const string EndOfServiceNotificationCount = "EndOfServiceNotificationCount";

        // for ExposureConfigurationRepository
        public const string IsDiagnosisKeysDataMappingConfigurationUpdated = "IsDiagnosisKeysDataMappingConfigurationUpdated";
        public const string ExposureConfigurationDownloadedEpoch = "ExposureConfigurationDownloadedEpoch";
        public const string ExposureConfigurationAppliedEpoch = "ExposureConfigurationAppliedEpoch";


        // for secure storage
        public static string ExposureInformation = "ExposureInformation";
    }
}
