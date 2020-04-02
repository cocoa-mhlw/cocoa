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
    public sealed class BLEManufacturerData : BLERecord
    {
        public ushort CompanyId { get; set; }
        public byte[] Data { get; set; }

        public BLEManufacturerData(BLEPacketType packetType, byte[] data)
            : base(packetType)
        {
            CompanyId = BitConverter.ToUInt16(data, 0);
            Data = new byte[data.Length - 2];
            Buffer.BlockCopy(data, 2, Data, 0, Data.Length);
        }

        public BLEManufacturerData()
            : base(BLEPacketType.ManufacturerData)
        {
        }

    }
}
