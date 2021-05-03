/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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
