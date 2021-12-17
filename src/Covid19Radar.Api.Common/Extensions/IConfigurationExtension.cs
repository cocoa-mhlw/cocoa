/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Extensions.Configuration;

namespace Covid19Radar.Api
{
    public static class IConfigurationExtension
    {
        public static string AzureFrontDoorId(this IConfiguration config) => config["AzureFrontDoorId"];
        public static bool AzureFrontDoorRestrictionEnabled(this IConfiguration config)
        {
            bool result;
            if (bool.TryParse(config["AzureFrontDoorRestrictionEnabled"], out result)) return result;
            return false;
        }

        public static string CosmosEndpointUri(this IConfiguration config) => config["COSMOS_ENDPOINT_URI"];
        public static string CosmosPrimaryKey(this IConfiguration config) => config["COSMOS_PRIMARY_KEY"];
        public static string CosmosDatabaseId(this IConfiguration config) => config["COSMOS_DATABASE_ID"];
        public static bool CosmosAutoGenerate(this IConfiguration config) => bool.Parse(config["COSMOS_AUTO_GENERATE"]);

        public static string InquiryLogApiKey(this IConfiguration config) => config["InquiryLogApiKey"];

    }
}
