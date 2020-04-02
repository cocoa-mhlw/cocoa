// Copyright 2015 - 2017 Andreas Jakl. All rights reserved. 
// https://github.com/andijakl/universal-beacon 
// 
// Based on the Eddystone specification by Google, 
// available under Apache License, Version 2.0 from
// https://github.com/google/eddystone
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
//    http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License. 

using System;
using System.IO;

namespace UniversalBeacon.Library.Core.Entities
{
    /// <summary>
    /// An Eddystone Telemetry frame, according to the Google Specification from
    /// https://github.com/google/eddystone/tree/master/eddystone-tlm
    /// </summary>
    public class TlmEddystoneFrame : BeaconFrameBase
    {
        private byte _version;
        private ushort _batteryInMilliV;
        private float _temperatureInC;
        private uint _advertisementFrameCount;
        private uint _timeSincePowerUp;

        /// <summary>
        /// Version of the Eddystone Telemetry (TLM) frame.
        /// Current and supported value is 0.
        /// </summary>
        public byte Version
        {
            get => _version;
            set
            {
                if (_version == value) return;
                _version = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The current battery voltage in millivolts [mV].
        /// If not supported (e.g., USB powerd beacon) = 0.
        /// </summary>
        public ushort BatteryInMilliV
        {
            get => _batteryInMilliV;
            set
            {
                if (_batteryInMilliV == value) return;
                _batteryInMilliV = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Beacon temperature in degrees Celsius.
        /// Note: does not have full float precision, not more than 2 decimal places.
        /// If not supported, -128 °C.
        /// </summary>
        public float TemperatureInC
        {
            get => _temperatureInC;
            set
            {
                if (Math.Abs(_temperatureInC - value) < 0.0001) return;
                _temperatureInC = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Running count of advertisement frames of all types emitted
        /// by the beacon since the last power up / reboot.
        /// </summary>
        public uint AdvertisementFrameCount
        {
            get => _advertisementFrameCount;
            set
            {
                if (_advertisementFrameCount == value) return;
                _advertisementFrameCount = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Time in 0.1 second resolution since the last beacon power up / reboot.
        /// </summary>
        public uint TimeSincePowerUp
        {
            get => _timeSincePowerUp;
            set
            {
                if (_timeSincePowerUp == value) return;
                _timeSincePowerUp = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        public TlmEddystoneFrame(byte version, ushort batteryInMv, float temperatureInC,
            uint advertisementFrameCount, uint timeSincePowerUp)
        {
            _version = version;
            _batteryInMilliV = batteryInMv;
            _temperatureInC = temperatureInC;
            _advertisementFrameCount = advertisementFrameCount;
            _timeSincePowerUp = timeSincePowerUp;
            UpdatePayload();
        }

        /// <summary>
        /// Create new instance of the TLM Eddystone frame based on the provided payload.
        /// Parses the payload and initializes the instance.
        /// </summary>
        /// <param name="payload">Payload to parse for this frame type.</param>
        public TlmEddystoneFrame(byte[] payload) : base(payload)
        {
            ParsePayload();
        }

        /// <summary>
        /// Parse the current payload into the properties exposed by this class.
        /// Has to be called if manually modifying the raw payload.
        /// </summary>
        public void ParsePayload()
        {
            using (var ms = new MemoryStream(Payload, false))
            {
                using (var reader = new BinaryReader(ms))
                {
                    // Skip frame header
                    ms.Position = BeaconFrameHelper.EddystoneHeaderSize;

                    // At present the value must be 0x00 
                    var newVersion = reader.ReadByte();
                    if (newVersion != Version)
                    {
                        _version = newVersion;
                        OnPropertyChanged(nameof(Version));
                    }

                    // Battery voltage is the current battery charge in millivolts, expressed as 1 mV per bit.
                    // If not supported (for example in a USB-powered beacon) the value should be zeroed.
                    var batteryBytes = reader.ReadBytes(2);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(batteryBytes);
                    }
                    var newBatteryInMilliV = BitConverter.ToUInt16(batteryBytes, 0);
                    if (BatteryInMilliV != newBatteryInMilliV)
                    {
                        _batteryInMilliV = newBatteryInMilliV;
                        OnPropertyChanged(nameof(BatteryInMilliV));
                    }

                    // Beacon temperature is the temperature in degrees Celsius sensed by the beacon and 
                    // expressed in a signed 8.8 fixed-point notation. 
                    // If not supported the value should be set to 0x8000, -128 °C.
                    // use signed https://courses.cit.cornell.edu/ee476/Math/
                    // #define float2fix(a) ((int)((a)*256.0))         //Convert float to fix. a is a float
                    // #define fix2float(a) ((float)(a)/256.0)         //Convert fix to float. a is an int 
                    var temperatureBytes = reader.ReadBytes(2);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(temperatureBytes);
                    }
                    var newTemperatureInC = BitConverter.ToInt16(temperatureBytes, 0) / (float)256.0;
                    if (Math.Abs(newTemperatureInC - TemperatureInC) > 0.0001 )
                    {
                        _temperatureInC = newTemperatureInC;
                        OnPropertyChanged(nameof(TemperatureInC));
                    }

                    // ADV_CNT is the running count of advertisement frames of all types 
                    // emitted by the beacon since power-up or reboot, useful for monitoring 
                    // performance metrics that scale per broadcast frame.
                    // If this value is reset (e.g.on reboot), the current time field must also be reset.
                    var advCntBytes = reader.ReadBytes(4);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(advCntBytes);
                    }
                    var newAdvertisementFrameCount = BitConverter.ToUInt32(advCntBytes, 0);
                    if (newAdvertisementFrameCount != AdvertisementFrameCount)
                    {
                        _advertisementFrameCount = newAdvertisementFrameCount;
                        OnPropertyChanged(nameof(AdvertisementFrameCount));
                    }

                    // SEC_CNT is a 0.1 second resolution counter that represents time since beacon power - 
                    // up or reboot. If this value is reset (e.g.on a reboot), the ADV count field must also be reset.
                    var secCntBytes = reader.ReadBytes(4);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(secCntBytes);
                    }
                    var newTimeSincePowerUp = BitConverter.ToUInt32(secCntBytes, 0);
                    if (newTimeSincePowerUp != TimeSincePowerUp)
                    {
                        _timeSincePowerUp = newTimeSincePowerUp;
                        OnPropertyChanged(nameof(TimeSincePowerUp));
                    }
                    
                    //Debug.WriteLine("Eddystone Tlm Frame: Version = "
                    //    + Version + ", Battery = " + (BatteryInMilliV / 1000.0) + "V, Temperature = " + TemperatureInC
                    //    + "°C, Frame count = " + AdvertisementFrameCount + ", Time since power up = " + TimeSincePowerUp);
                }
            }
        }

        /// <summary>
        /// Update the raw payload when properties have changed.
        /// </summary>
        private void UpdatePayload()
        {
            var header = BeaconFrameHelper.CreateEddystoneHeader(BeaconFrameHelper.EddystoneFrameType.TelemetryFrameType);
            using (var ms = new MemoryStream())
            {
                // Frame header
                ms.Write(header, 0, header.Length);
                // Version
                ms.WriteByte(Version);
                // Battery
                var batBytes = BitConverter.GetBytes(BatteryInMilliV);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(batBytes);
                ms.Write(batBytes, 0, batBytes.Length);
                // Temperature
                // #define float2fix(a) ((int)((a)*256.0))         //Convert float to fix. a is a float
                var temp = ((int)((TemperatureInC) * 256.0));
                var tempBytes = BitConverter.GetBytes(temp);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(tempBytes);
                ms.Write(tempBytes, 2, 2); 
                // ADV_CNT
                var advCntBytes = BitConverter.GetBytes(AdvertisementFrameCount);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(advCntBytes);
                ms.Write(advCntBytes, 0, 4);
                // SEC_CNT
                var secCntBytes = BitConverter.GetBytes(TimeSincePowerUp);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(secCntBytes);
                ms.Write(secCntBytes, 0, 4);
                // Save to payload (to direct array to prevent re-parsing and a potential endless loop of updating and parsing)
                _payload = ms.ToArray();
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
        /// <returns>True if the frame is a valid Eddystone TLM frame.</returns>
        public override bool IsValid()
        {
            if (!base.IsValid()) return false;

            // 2 bytes ID: AA FE
            // 1 byte frame type
            if (!Payload.IsEddystoneFrameType()) return false;

            // Check if the frame type is correct for TLM
            var eddystoneFrameType = Payload.GetEddystoneFrameType();
            if (eddystoneFrameType == null || eddystoneFrameType !=
                BeaconFrameHelper.EddystoneFrameType.TelemetryFrameType) return false;

            // 1 byte version
            // 2 bytes battery voltage
            // 2 bytes beacon temperature
            // 4 bytes adv_cnt (AdvertisementFrameCount)
            // 4 bytes sec_cnt (TimeSincePowerUp)
            return Payload.Length == BeaconFrameHelper.EddystoneHeaderSize + 13;
        }
    }
}
