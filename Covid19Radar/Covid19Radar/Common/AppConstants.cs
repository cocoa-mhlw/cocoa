using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Common
{
    public class AppConstants
    {
        public static readonly string AppCenterTokensAndroid = "AppIdForAndroidPlaceholder";
        public static readonly string AppCenterTokensIOS = "AppIdForiOSPlaceholder";
		/// <summary>
		/// Start Transmit
		/// </summary>
		public static readonly string STR_TRANSMIT_START = "Start Transmit";

		/// <summary>
		/// Stop transmit
		/// </summary>
		public static readonly string STR_TRANSMIT_STOP = "Stop Transmit";

		/// <summary>
		/// Error Dialog title
		/// </summary>
		public static readonly string STR_DIALOG_TITLE_ERROR = "Error";

		/// <summary>
		/// Error Dialog messenge. 
		/// </summary>
		public static readonly string STR_DIALOG_MSG_CANNOT_TRANSMIT = "This device does not support Bluetooth Low Energy calling or the Bluetooth function is turned off.";

		/// <summary>
		/// Error Dialog button OK
		/// </summary>
		public static readonly string STR_DIALOG_BUTTON_OK = "OK";

		/// <summary>
		/// Apple's company ibeacon code
		/// </summary>
		public static readonly byte COMPANY_CODE_APPLE = 0x004C;
		/// <summary>
		/// iBeacon byte format
		/// m：beacon type
		/// i：identifier（UUID / Major / Minor）
		/// p：Power calibration value
		/// </summary>

		public static readonly string IBEACON_FORMAT = "m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24";

		/// <summary>
		/// iBeacon region name - Find regions with the same value
		/// </summary>

		public static readonly string AppUUID = "7822fa0f-ce38-48ea-a7e8-e72af4e42c1c";

		public static readonly long BEACONS_UPDATES_IN_MILLISECONDS = 5 * 1000;

	}
}
