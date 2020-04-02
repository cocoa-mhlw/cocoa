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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UniversalBeacon.Library.Core.Entities
{
    /// <summary>
    /// Abstract class for every Bluetooth Beacon frame.
    /// Common is that every frame has a payload / data, which derived classes can
    /// further parse and make it more accessible through custom properties depending
    /// on the beacon specification.
    /// </summary>
    public abstract class BeaconFrameBase : INotifyPropertyChanged
    {
        protected byte[] _payload;
        
        /// <summary>
        /// The raw byte payload of this beacon frame.
        /// Derived classes can add additional functionality to parse or update the payload.
        /// 
        /// Note: directly setting the payload does not lead to the class re-parsing the payload
        /// into its instance variables (if applicable in the derived class).
        /// Call ParsePayload() manually from the derived class if you wish to enable this behavior.
        /// </summary>
        public byte[] Payload
        {
            get => _payload;
            set
            {
                if (value == null)
                {
                    _payload = null;
                    return;
                }
                if (_payload != null && _payload.SequenceEqual(value)) return;
                _payload = new byte[value.Length];
                Array.Copy(value, _payload, value.Length);
                OnPropertyChanged();
            }
        }

        protected BeaconFrameBase()
        {
            
        }

        protected BeaconFrameBase(byte[] payload)
        {
            Payload = payload;
        }

        protected BeaconFrameBase(BeaconFrameBase other)
        {
            Payload = other.Payload;
        }

        /// <summary>
        /// Update payload based on supplied other frame.
        /// </summary>
        /// <param name="otherFrame"></param>
        public virtual void Update(BeaconFrameBase otherFrame)
        {
            Payload = otherFrame.Payload;
        }

        /// <summary>
        /// Checks if the payload is valid.
        /// Base implementation only checks if payload is not null.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid()
        {
            return Payload != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
