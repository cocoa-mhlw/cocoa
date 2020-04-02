using System;
using System.IO;
using System.Linq;

namespace UniversalBeacon.Library.Core.Entities
{
    /// <summary>
    /// An Eddystone EID (encrypted ephemeral identifier) frame, according to the 
    /// Google specification at: 
    /// https://github.com/google/eddystone/tree/master/eddystone-eid
    /// 
    /// This implementation does not contain cryptography or key management.
    /// It parses the Eddystone frame and the 8 byte Ephemeral Identifier out
    /// of the beacon advertisment.
    /// For more information on how to compute the EID, see:
    /// https://github.com/google/eddystone/blob/master/eddystone-eid/eid-computation.md
    /// </summary>
    public class EidEddystoneFrame : BeaconFrameBase
    {
        private sbyte _rangingData;

        /// <summary>
        /// Tx power level - the received power level at 0 m, in dBm.
        /// Values range from -100 to +20 dBM, with a resolution of 1 dBm.
        /// Signed 8 bit integer according to:
        /// https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.tx_power_level.xml
        /// Conversion between sbyte and byte works at runtime with simple casting for variables.
        /// In order to convert constants at compile time, you might need to run the conversion in
        /// an unchecked() context. For example:
        /// sbyte rangingData = unchecked((sbyte)0xEE);
        /// According to the documentation, this corresponds to the dBm value of -18.
        /// </summary>
        public sbyte RangingData
        {
            get => _rangingData;
            set
            {
                if (_rangingData == value) return;
                _rangingData = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        private const int EphemeralIdentifierLength = 8;
        private byte[] _ephemeralIdentifier;

        /// <summary>
        /// 8-byte Ephemeral Identifier, used for cryptography.
        /// For hints how to compute the EID, see:
        /// https://github.com/google/eddystone/blob/master/eddystone-eid/eid-computation.md
        /// </summary>
        public byte[] EphemeralIdentifier
        {
            get => _ephemeralIdentifier;
            set
            {
                if (_ephemeralIdentifier == value) return;
                if (value == null || value.Length != EphemeralIdentifierLength)
                {
                    _ephemeralIdentifier = null;
                    return;
                }
                if (_ephemeralIdentifier != null && _ephemeralIdentifier.SequenceEqual(value)) return;
                _ephemeralIdentifier = new byte[value.Length];
                Array.Copy(value, _ephemeralIdentifier, value.Length);
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Create new instance of the EID Eddystone frame based on the provided payload.
        /// Parses the payload and initializes the instance.
        /// </summary>
        /// <param name="payload">Payload to parse for this frame type.</param>
        public EidEddystoneFrame(byte[] payload) : base(payload)
        {
            ParsePayload();
        }

        /// <summary>
        /// Create new EID Eddystone frame based on the provided data.
        /// </summary>
        /// <param name="rangingData">Ranging data (Tx power level) - see property documentation.</param>
        /// <param name="ephemeralIdentifier">Ephemeral Identifier (8 bytes) - see property documentation.</param>
        public EidEddystoneFrame(sbyte rangingData, byte[] ephemeralIdentifier)
        {
            _rangingData = rangingData;
            if (ephemeralIdentifier != null && ephemeralIdentifier.Length == EphemeralIdentifierLength)
            {
                _ephemeralIdentifier = new byte[EphemeralIdentifierLength];
                Array.Copy(ephemeralIdentifier, _ephemeralIdentifier, ephemeralIdentifier.Length);
            }
            UpdatePayload();
        }

        /// <summary>
        /// Parse the current payload into the properties exposed by this class.
        /// Has to be called if manually modifying the raw payload.
        /// </summary>
        public void ParsePayload()
        {
            if (!IsValid()) return;

            // The Ranging Data is the Tx power in dBm emitted by the beacon at 0 meters.
            // Note that this is different from other beacon protocol specifications that require the Tx power 
            // to be measured at 1 m.The value is an 8-bit integer as specified by the Tx Power Level Characteristic 
            // and ranges from -100 dBm to +20 dBm to a resolution of 1 dBm.
            var newRangingData = (sbyte)Payload[BeaconFrameHelper.EddystoneHeaderSize];
            if (newRangingData != RangingData)
            {
                _rangingData = newRangingData;
                OnPropertyChanged(nameof(RangingData));
            }

            // 8-byte Ephemeral Identifier
            var newEphemeralIdentifier = new byte[EphemeralIdentifierLength];
            Array.Copy(Payload, BeaconFrameHelper.EddystoneHeaderSize + 1, newEphemeralIdentifier, 0, EphemeralIdentifierLength);
            if (EphemeralIdentifier == null || !newEphemeralIdentifier.SequenceEqual(EphemeralIdentifier))
            {
                _ephemeralIdentifier = newEphemeralIdentifier;
                OnPropertyChanged(nameof(EphemeralIdentifier));
            }

            //Debug.WriteLine("Eddystone EID Frame: Ranging data = "
            //    + RangingData + ", Ephemeral Identifier = " + BitConverter.ToString(InstanceId));
        }

        /// <summary>
        /// Update the raw payload when properties have changed.
        /// </summary>
        private void UpdatePayload()
        {
            var header =
                BeaconFrameHelper.CreateEddystoneHeader(BeaconFrameHelper.EddystoneFrameType.EidFrameType);
            using (var ms = new MemoryStream())
            {
                // Frame header
                ms.Write(header, 0, header.Length);
            }
        }

        /// <summary>
        /// Update the information stored in this frame with the information from the other frame.
        /// Useful for example when binding the UI to beacon information, as this will emit
        /// property changed notifications whenever a value changes - which would not be possible if
        /// you would overwrite the whole frame.
        /// </summary>
        /// <param name="otherFrame">Frame to use as source for updating the information in this beacon
        /// frame.</param>
        public override void Update(BeaconFrameBase otherFrame)
        {
            base.Update(otherFrame);
            ParsePayload();
        }

        /// <summary>
        /// Check if the contents of this frame are generally valid.
        /// Does not currently perform a deep analysis, but checks the header as well
        /// as the frame length.
        /// </summary>
        /// <returns>True if the frame is a valid Eddystone EID frame.</returns>
        public override bool IsValid()
        {
            if (!base.IsValid()) return false;

            // 2 bytes ID: AA FE
            // 1 byte frame type
            if (!Payload.IsEddystoneFrameType()) return false;

            // Check if the frame type is correct for EID
            var eddystoneFrameType = Payload.GetEddystoneFrameType();
            if (eddystoneFrameType == null || eddystoneFrameType !=
                BeaconFrameHelper.EddystoneFrameType.EidFrameType) return false;

            // 1 byte ranging data
            // 8 bytes 8-byte Ephemeral Identifier
            return true; // TODO: check, seems like EID frame can be longer (0-padding at the end) in practice. 
            // Payload.Length == BeaconFrameHelper.EddystoneHeaderSize + 9;
        }
    }
}
