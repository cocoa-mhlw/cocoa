/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar.Common
{
    public static class AppConstants
    {
        /// <summary>
        /// Number of days covered from the date of diagnosis or onset
        /// </summary>
        public const int DaysToSendTek = -3;

        /// <summary>
        /// Max Error Count
        /// </summary>
        public const int MaxErrorCount = 3;

        /// <summary>
        /// Max diagnosis UID Count
        /// </summary>
        public const int MaxDiagnosisUidCount = 8;

        public const string positiveRegex = @"\b[0-9]{8}\b";
    }
}
