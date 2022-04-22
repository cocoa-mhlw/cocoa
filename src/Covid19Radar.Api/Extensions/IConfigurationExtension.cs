﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Common;
using Microsoft.Extensions.Configuration;

namespace Covid19Radar.Api
{
    public static class IConfigurationExtension
    {
        public static string ExportKeyUrl(this IConfiguration config) => config["ExportKeyUrl"];
        public static string TekExportBlobStorage(this IConfiguration config) => config["TekExportBlobStorage"];
        public static string TekExportBlobStorageContainerPrefix(this IConfiguration config) => config["TekExportBlobStorageContainerPrefix"];
        public static string InquiryLogBlobUrl(this IConfiguration config) => config["InquiryLogBlobUrl"];
        public static string InquiryLogBlobAccountName(this IConfiguration config) => config["InquiryLogBlobAccountName"];
        public static string InquiryLogBlobAccountKey(this IConfiguration config) => config["InquiryLogBlobAccountKey"];
        public static string InquiryLogBlobStorageContainerPrefix(this IConfiguration config) => config["InquiryLogBlobStorageContainerPrefix"];
        public static string[] SupportRegions(this IConfiguration config) => config["SupportRegions"].Split(',');
        public static string AndroidPackageName(this IConfiguration config) => config["AndroidPackageName"];
        public static string AndroidSafetyNetSecret(this IConfiguration config) => config["AndroidSafetyNetSecret"];
        public static bool AndroidDeviceValidationEnabled(this IConfiguration config)
        {
            bool result;
            if (bool.TryParse(config["AndroidDeviceValidationEnabled"], out result)) return result;
            return false;
        }
        public static string iOSBundleId(this IConfiguration config) => config["iOSBundleId"];
        public static string iOSDeviceCheckKeyId(this IConfiguration config) => config["iOSDeviceCheckKeyId"];
        public static string iOSDeviceCheckPrivateKey(this IConfiguration config) => config["iOSDeviceCheckPrivateKey"];
        public static string iOSDeviceCheckTeamId(this IConfiguration config) => config["iOSDeviceCheckTeamId"];
        public static bool iOSDeviceValidationEnabled(this IConfiguration config)
        {
            bool result;
            if (bool.TryParse(config["iOSDeviceValidationEnabled"], out result)) return result;
            return false;
        }
        public static string VerificationPayloadParameterName(this IConfiguration config) => config["VerificationPayloadParameterName"];
        public static string VerificationPayloadApiSecret(this IConfiguration config) => config["VerificationPayloadApiSecret"];
        public static string VerificationPayloadPfx(this IConfiguration config) => config["VerificationPayloadPfx"];
        public static string VerificationPayloadUrl(this IConfiguration config) => config["VerificationPayloadUrl"];

        public static int InfectiousFilterDaysSinceOnsetOfSymptomsFrom(this IConfiguration config)
        {
            if (int.TryParse(config["InfectiousFilterDaysSinceOnsetOfSymptomsFrom"], out int result))
            {
                return result;
            }
            return Constants.MIN_DAYS_SINCE_ONSET_OF_SYMPTOMS;
        }
        public static int InfectiousFilterDaysSinceOnsetOfSymptomsTo(this IConfiguration config)
        {
            if (int.TryParse(config["InfectiousFilterDaysSinceOnsetOfSymptomsTo"], out int result))
            {
                return result;
            }
            return Constants.MAX_DAYS_SINCE_ONSET_OF_SYMPTOMS;
        }
        public static int InfectiousFilterDaysSinceTestFrom(this IConfiguration config)
        {
            if (int.TryParse(config["InfectiousFilterDaysSinceTestFrom"], out int result))
            {
                return result;
            }
            return Constants.MIN_DAYS_SINCE_ONSET_OF_SYMPTOMS;
        }
        public static int InfectiousFilterDaysSinceTestTo(this IConfiguration config)
        {
            if (int.TryParse(config["InfectiousFilterDaysSinceTestTo"], out int result))
            {
                return result;
            }
            return Constants.MAX_DAYS_SINCE_ONSET_OF_SYMPTOMS;
        }
    }

}
