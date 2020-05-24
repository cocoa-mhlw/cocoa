using Microsoft.Extensions.Configuration;

namespace Covid19Radar.Api
{
    public static class IConfigurationExtension
    {
        public static string ExportKeyUrl(this IConfiguration config) => config["ExportKeyUrl"];
        public static string TekExportBlobStorageContainerPrefix(this IConfiguration config) => config["TekExportBlobStorageContainerPrefix"];
        public static string AndroidBearerToken(this IConfiguration config) => config["AndroidBearerToken"];
        public static string AppleBearerToken(this IConfiguration config) => config["AppleBearerToken"];
        public static string[] SupportRegions(this IConfiguration config) => config["SupportRegions"].Split(',');
    }
}
