namespace Covid19Radar.Common
{
    public static class PreferenceKey
    {
        // for preferences
        public static string StartDateTime = "StartDateTime";
        public static string LastProcessTekTimestamp = "LastProcessTekTimestamp";
        public static string ExposureNotificationConfiguration = "ExposureNotificationConfiguration";
        public static string TermsOfServiceLastUpdateDateTime = "TermsOfServiceLastUpdateDateTime";
        public static string PrivacyPolicyLastUpdateDateTime = "PrivacyPolicyLastUpdateDateTime";

        // for secure storage
        public static string ExposureInformation = "ExposureInformation";
        public static string ExposureSummary = "ExposureSummary";
    }
}
