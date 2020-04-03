using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Model
{
    /// <summary>
    /// iBeacon Infomation
    /// </summary>
    public class iBeacon
    {
        #region Constants

        /// <summary>
        /// Major Default
        /// </summary>
        public const ushort DEFAULT_MAJOR = 0;

        /// <summary>
        /// Minor Default
        /// </summary>
        public const ushort DEFAULT_MINOR = 0;

        /// <summary>
        /// TxPower Default
        /// </summary>
        public const sbyte DEFAULT_TXPOWER = -59;

        #endregion

        #region Valiables

        /// <summary>
        /// iBeacon UUID
        /// </summary>
        /// <value>iBeacon UUID</value>
        public Guid Uuid { get; set; }

        /// <summary>
        /// iBeacon Major
        /// </summary>
        /// <value>iBeacon Major</value>
        public ushort Major { get; set; }

        /// <summary>
        /// iBeacon Minor
        /// </summary>
        /// <value>iBeacon Minor</value>
        public ushort Minor { get; set; }

        /// <summary>
        /// iBeacon TxPower
        /// </summary>
        /// <value>iBeacon TxPower</value>
        public sbyte TxPower { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// UUID / Major / Minor All Default value instance
        /// </summary>
        public iBeacon()
        {
            this.Uuid = Guid.Empty;
            this.Major = DEFAULT_MAJOR;
            this.Minor = DEFAULT_MINOR;
            this.TxPower = DEFAULT_TXPOWER;
        }

        /// <summary>
        /// arng's UUID Major Minor TxPower Values iBeacon
        /// </summary>
        /// <param name="uuid">iBeacon UUID</param>
        /// <param name="major">Major</param>
        /// <param name="minor">Minor</param>
        public iBeacon(Guid uuid, ushort major, ushort minor, sbyte txPower)
        {
            this.Uuid = uuid;
            this.Major = major;
            this.Minor = minor;
            this.TxPower = txPower;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// UUID Major Minor Validator
        /// </summary>
        /// <returns>
        /// <c>true</c>
        /// <c>false</c>
        /// </returns>
        /// <param name="uuid">UUID</param>
        /// <param name="major">Major</param>
        /// <param name="minor">Minor</param>
        /// <param name="errorMsg">Check Result message</param>
        public static bool IsValidInput(string uuid, string major, string minor, out string errorMsg)
        {
            bool isValidUuid = IsValidInputForUuid(uuid);
            bool isValidMajor = IsValidInputForMajorOrMinor(major);
            bool isValidMinor = IsValidInputForMajorOrMinor(minor);

            errorMsg = string.Empty;

            if (isValidUuid && isValidMajor && isValidMinor)
            {
                return true;
            }

            if (!isValidUuid)
            {
                errorMsg += "UUID format is incorrect";
            }
            if (!isValidMajor)
            {
                if (errorMsg != string.Empty)
                {
                    errorMsg += Environment.NewLine;
                }
                errorMsg += "An integer value between 0 and 65535 has not been entered for Major.";
            }
            if (!isValidMinor)
            {
                if (errorMsg != string.Empty)
                {
                    errorMsg += Environment.NewLine;
                }
                errorMsg += "An integer value between 0 and 65535 has not been entered for Minor.";
            }
            return false;
        }

        public static bool IsValidInputForUuid(string strUuid)
        {
            Guid uuid;
            bool checkResult = Guid.TryParse(strUuid, out uuid);
            return checkResult;
        }

        public static bool IsValidInputForMajorOrMinor(string strInputValue)
        {
            ushort parsedValue;
            bool checkResult = ushort.TryParse(strInputValue, out parsedValue);
            return checkResult;
        }

        #endregion
    }
}
