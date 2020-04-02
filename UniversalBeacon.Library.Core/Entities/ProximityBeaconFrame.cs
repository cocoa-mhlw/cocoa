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

namespace UniversalBeacon.Library.Core.Entities
{
    /// <summary>
    /// Frame type for Proximity Beacon frames (compatible to iBeacon).
    /// </summary>
    public class ProximityBeaconFrame : BeaconFrameBase
    {
        public const ushort CompanyId = 0x004C;
        public const byte DataType = 0x02;
        public const byte DataLength = 0x15;

        private Guid _uuid;
        private ushort _major;
        private ushort _minor;
        private sbyte _txPower;

        public ProximityBeaconFrame(byte[] payload)
            : base(payload)
        {
            ParsePayload();
        }

        public string UuidAsString => Uuid.ToString("D");

        public Guid Uuid
        {
            get => _uuid;
            set
            {
                if (_uuid == value) return;
                _uuid = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UuidAsString));
            }
        }

        public string MajorAsString => Major.ToString("x4");

        public ushort Major
        {
            get => _major;
            set
            {
                if (_major == value) return;
                _major = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MajorAsString));
            }
        }

        public string MinorAsString => Minor.ToString("x4");

        public ushort Minor
        {
            get => _minor;
            set
            {
                if (_minor == value) return;
                _minor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MinorAsString));
            }
        }

        public sbyte TxPower
        {
            get => _txPower;
            set
            {
                if (_txPower == value) return;
                _txPower = value;
                OnPropertyChanged();
            }
        }

        private void ParsePayload()
        {
            var dataType = Payload[0];
            var dataLength = Payload[1];

            if (dataType != DataType || dataLength != DataLength)
                throw new InvalidOperationException();

            // need to swap to big-endian
            var uuidBytes = new byte[16];
            if (BitConverter.IsLittleEndian)
            {
                Array.ConstrainedCopy(Payload, 2, uuidBytes, 0, 16);
                Array.Reverse(uuidBytes, 0, 4);
                Array.Reverse(uuidBytes, 4, 2);
                Array.Reverse(uuidBytes, 6, 2);
            }
            Uuid = new Guid(uuidBytes);

            var majorBytes = new byte[2];
            Array.ConstrainedCopy(Payload, 18, majorBytes, 0, 2);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(majorBytes); 
            Major = BitConverter.ToUInt16(majorBytes,0);

            var minorBytes = new byte[2];
            Array.ConstrainedCopy(Payload, 20, minorBytes, 0, 2);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(minorBytes);
            Minor = BitConverter.ToUInt16(minorBytes, 0);

            TxPower = (sbyte) Payload[22];
        }

        public override void Update(BeaconFrameBase otherFrame)
        {
            base.Update(otherFrame);
            ParsePayload();
        }
    }
}
