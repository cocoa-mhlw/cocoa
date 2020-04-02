namespace UniversalBeacon.Library.Core.Interop
{
    public class BTError
    {
        /// <summary>
        /// Possible error codes for Bluetooth handling.
        /// Binary compatible to the UWP implementation.
        /// </summary>
        public enum BluetoothError
        {
            //
            // Summary:
            //     The operation was successfully completed or serviced.
            Success = 0,
            //
            // Summary:
            //     The Bluetooth radio was not available. This error occurs when the Bluetooth radio
            //     has been turned off.
            RadioNotAvailable = 1,
            //
            // Summary:
            //     The operation cannot be serviced because the necessary resources are currently
            //     in use.
            ResourceInUse = 2,
            //
            // Summary:
            //     The operation cannot be completed because the remote device is not connected.
            DeviceNotConnected = 3,
            //
            // Summary:
            //     An unexpected error has occurred.
            OtherError = 4,
            //
            // Summary:
            //     The operation is disabled by policy.
            DisabledByPolicy = 5,
            //
            // Summary:
            //     The operation is not supported on the current Bluetooth radio hardware.
            NotSupported = 6,
            //
            // Summary:
            //     The operation is disabled by the user.
            DisabledByUser = 7,
            //
            // Summary:
            //     The operation requires consent.
            ConsentRequired = 8,
            //
            TransportNotSupported = 9
        }


        public BluetoothError BluetoothErrorCode { get; set; }

        public BTError(BluetoothError btErrorCode)
        {
            BluetoothErrorCode = btErrorCode;
        }
    }
}
