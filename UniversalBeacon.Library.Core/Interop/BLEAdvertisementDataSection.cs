// Copyright 2015 - 2017 Chris Tacke. All rights reserved. 
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
// limitations under the License. using System;

using System;

namespace UniversalBeacon.Library.Core.Interop
{
    public sealed class BLEAdvertisementDataSection : BLERecord
    {
        public ushort Manufacturer { get; private set; }

        public byte[] Data { get; set; }
        public byte DataType { get; set; }

        public BLEAdvertisementDataSection()
            : base(BLEPacketType.ServiceData)
        {
        }

        public BLEAdvertisementDataSection(BLEPacketType packetType, byte[] data)
            : base(packetType)
        {
            Manufacturer = BitConverter.ToUInt16(data, 0);
            Data = new byte[data.Length];
            Buffer.BlockCopy(data, 0, Data, 0, Data.Length);

            // TODO:        
            // it's unclear to me where this comes from
            // The WIndows API sets it for Eddystone packets, but I can't find it in any documentation
            //DataType = 0x16;
        }
    }

}
