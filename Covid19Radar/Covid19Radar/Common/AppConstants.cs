using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Common
{
    public static class AppConstants
    {
        public static readonly string AppCenterTokensAndroid = "APPCENTER_ANDROID";
        public static readonly string AppCenterTokensIOS = "APPCENTER_IOS";

        public static readonly string ApiUserSecretKeyPrefix = "Bearer";
        public static readonly string AppName = "COVID-19Radar";
        public static readonly string AppVersion = "Ver 1.0.0";
        public static readonly string AppStoreUrl = "https://www.apple.com/jp/ios/app-store/";
        public static readonly string GooglePlayUrl = "https://play.google.com/store";
        //public static readonly string ApiBaseUrl = "http://192.168.1.2:7071/api";

        public static readonly string ApiBaseUrl = "https://covid19radar-jpn-prod.trafficmanager.net/api";
        //        public static readonly string ApiSecret = "API_SECRET";
        public static readonly string ApiSecret = "qa4uj6Dzamn0W7nQBfStzqctargTjFD8vNq0S11rO5e4ThNYt6vBpg==";

        public static readonly int NumberOfGroup = 86400;

        public static readonly string SqliteFilename = "local.db3";

        public static readonly string LicenseUrl = "https://covid19radar.z11.web.core.windows.net/license.html";

        // Android Safetynet API Key
        public static readonly string safetyNetApiKey = "YOUR-KEY-HERE";
    }
}
