using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata.Ecma335;

namespace Covid19Radar.Api
{
    public static class IConfigurationExtension
    {
        public static string ExportKeyUrl(this IConfiguration config) => config["ExportKeyUrl"];
        public static string TekExportBlobStorageContainerPrefix(this IConfiguration config) => config["TekExportBlobStorageContainerPrefix"];
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
    }
    
}
