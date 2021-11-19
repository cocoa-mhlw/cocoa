/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Common;
using Covid19Radar.Api.Models;
using Covid19Radar.Background.Protobuf;
using Google.Protobuf;

namespace Covid19Radar.Background.Extentions
{
    public static class TemporaryExposureKeyModelExtension
    {
        public static TemporaryExposureKey ToKey(this TemporaryExposureKeyModel tek)
        {
            var key = new TemporaryExposureKey()
            {
                KeyData = ByteString.CopyFrom(tek.KeyData),
                RollingStartIntervalNumber = tek.RollingStartIntervalNumber,
                RollingPeriod = tek.RollingPeriod,
                TransmissionRiskLevel = tek.TransmissionRiskLevel,
            };

            if (tek.ReportType != Constants.ReportTypeMissingValue)
            {
                key.ReportType = ConvertToReportType(tek.ReportType);
            }

            if (tek.DaysSinceOnsetOfSymptoms != Constants.DaysSinceOnsetOfSymptomsMissingValue)
            {
                key.DaysSinceOnsetOfSymptoms = tek.DaysSinceOnsetOfSymptoms;
            }

            return key;
        }

        /*
         * enum ReportType {
         *      UNKNOWN = 0;  // Never returned by the client API.
         *      CONFIRMED_TEST = 1;
         *      CONFIRMED_CLINICAL_DIAGNOSIS = 2;
         *      SELF_REPORT = 3;
         *      RECURSIVE = 4;  // Reserved for future use.
         *      REVOKED = 5;  // Used to revoke a key, never returned by client API.
         * }
         */
        private static TemporaryExposureKey.Types.ReportType ConvertToReportType(int reportType)
        {
            return reportType switch
            {
                1 => TemporaryExposureKey.Types.ReportType.ConfirmedTest,
                2 => TemporaryExposureKey.Types.ReportType.ConfirmedClinicalDiagnosis,
                3 => TemporaryExposureKey.Types.ReportType.SelfReport,
                4 => TemporaryExposureKey.Types.ReportType.Recursive,
                5 => TemporaryExposureKey.Types.ReportType.Revoked,
                _ => TemporaryExposureKey.Types.ReportType.Unknown,
            };
        }
    }
}
