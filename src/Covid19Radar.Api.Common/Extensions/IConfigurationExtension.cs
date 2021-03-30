/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using Microsoft.Extensions.Configuration;
using System;

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

        public static byte[] CryptionKey(this IConfiguration config) => Convert.FromBase64String(config["CRYPTION_KEY"]);
        public static byte[] CryptionIV(this IConfiguration config) => Convert.FromBase64String(config["CRYPTION_IV"]);
        public static string CryptionHash(this IConfiguration config) => config["CRYPTION_HASH"];
        public static byte[] CryptionKey2(this IConfiguration config) => Convert.FromBase64String(config["CRYPTION_KEY2"]);
        public static byte[] CryptionIV2(this IConfiguration config) => Convert.FromBase64String(config["CRYPTION_IV2"]);
        public static string InquiryLogApiKey(this IConfiguration config) => config["InquiryLogApiKey"];

    }
}
