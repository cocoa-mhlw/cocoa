using Microsoft.Extensions.Configuration;
using System;

namespace Covid19Radar.Api
{
    public static class IConfigurationExtension
    {
        public static string CosmosEndpointUri(this IConfiguration config) => config["COSMOS_ENDPOINT_URI"];
        public static string CosmosPrimaryKey(this IConfiguration config) => config["COSMOS_PRIMARY_KEY"];
        public static string CosmosDatabaseId(this IConfiguration config) => config["COSMOS_DATABASE_ID"];
        public static bool CosmosAutoGenerate(this IConfiguration config) => bool.Parse(config["COSMOS_AUTO_GENERATE"]);

        public static byte[] CryptionKey(this IConfiguration config) => Convert.FromBase64String(config["CRYPTION_KEY"]);
        public static byte[] CryptionIV(this IConfiguration config) => Convert.FromBase64String(config["CRYPTION_IV"]);
        public static string CryptionHash(this IConfiguration config) => config["CRYPTION_HASH"];
        public static byte[] CryptionKey2(this IConfiguration config) => Convert.FromBase64String(config["CRYPTION_KEY2"]);
        public static byte[] CryptionIV2(this IConfiguration config) => Convert.FromBase64String(config["CRYPTION_IV2"]);

    }
}
