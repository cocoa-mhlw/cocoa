using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Common
{
    public class AppConstants
    {
        public static readonly string AppCenterTokensAndroid = "APPCENTER_ANDROID";
        public static readonly string AppCenterTokensIOS = "APPCENTER_IOS";

        public static readonly string ApiBaseUrl = "https://covid19radar.azurewebsites.net/api";
        public static readonly string ApiSecret = "API_SECRET";
        /// <summary>
        /// Apple's company ibeacon code
        /// </summary>
        public static readonly byte CompanyCoreApple = 0x004C;

        /// <summary>
        /// iBeacon byte format
        /// m：beacon type
        /// i：identifier（UUID / Major / Minor）
        /// p：Power calibration value
        /// </summary>

        public static readonly string iBeaconFormat = "m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24";

        public static readonly int NumberOfGroup = 60;

        /// <summary>
        /// iBeacon region name - Find regions with the same value
        /// </summary>

        public static readonly string iBeaconAppUuid = "7822fa0f-ce38-48ea-a7e8-e72af4e42c1c";

        public static readonly long BeaconsUpdateInMillisec = 5 * 1000;

        public static readonly string SqliteFilename = "local.db3";

        public static readonly int ElapsedTimeOfTransmitStart = 5;

    }
}
