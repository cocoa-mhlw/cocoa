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
using System.Collections.ObjectModel;
using UniversalBeacon.Library.Core.Interfaces;
using UniversalBeacon.Library.Core.Interop;

namespace UniversalBeacon.Library.Core.Entities
{
    /// <summary>
    /// Manages multiple beacons and distributes received Bluetooth LE
    /// Advertisements based on unique Bluetooth beacons.
    /// 
    /// Whenever your app gets a callback that it has received a new Bluetooth LE
    /// advertisement, send it to the ReceivedAdvertisement() method of this class,
    /// which will handle the data and either add a new Bluetooth beacon to the list
    /// of beacons observed so far, or update a previously known beacon with the
    /// new advertisement information.
    /// </summary>
    public class BeaconManager
    {
        /// <summary>
        /// Event that is invoked whenever a new (unknown) beacon is discovered and added to the list
        /// of known beacons (BluetoothBeacons).
        /// To subscribe to updates of every single received Bluetooth advertisment packet, 
        /// subscribe to the OnAdvertisementPacketReceived event of the IBluetoothPacketProvider.
        /// </summary>
        public event EventHandler<Beacon> BeaconAdded;

        /// <summary>
        /// List of known beacons so far, which all have a unique Bluetooth MAC address
        /// and can have multiple data frames.
        /// </summary>
        public ObservableCollection<Beacon> BluetoothBeacons { get; set; } = new ObservableCollection<Beacon>();

        /// <summary>
        /// Provider that emits events whenever new Bluetooth advertisement packets have been received.
        /// </summary>
        private readonly IBluetoothPacketProvider _provider;

        /// <summary>
        /// Optional action that is used to handle the received Bluetooth event, e.g., to handle it in a different
        /// thread. Can be used to handle the added beacons on the UI thread, if these are received in a background thread.
        /// </summary>
        private readonly Action<Action> _invokeAction;

        /// <summary>
        /// Create new Beacon Manager based on the provider that is going to update the manager with
        /// new received Bluetooth Packets.
        /// </summary>
        /// <param name="provider">Package provider that emits events whenever Bluetooth advertisement packets
        /// have been received.</param>
        /// <param name="invokeAction">Optional invoke action, e.g., to run the code to handle the received
        /// event in a different thread.</param>
        public BeaconManager(IBluetoothPacketProvider provider, Action<Action> invokeAction = null)
        {
            _provider = provider;
            _provider.AdvertisementPacketReceived += OnAdvertisementPacketReceived;
            _invokeAction = invokeAction;
        }

        /// <summary>
        /// Relay the start event to the Blueooth packet provider.
        /// </summary>
        public void Start()
        {
            _provider.Start();
        }

        /// <summary>
        /// Relay the stop event to the Blueooth packet provider.
        /// </summary>
        public void Stop()
        {
            _provider.Stop();
        }

        private void OnAdvertisementPacketReceived(object sender, BLEAdvertisementPacketArgs e)
        {
            if (_invokeAction != null)
            {
                _invokeAction(() => { ReceivedAdvertisement(e.Data); });
            }
            else
            {
                ReceivedAdvertisement(e.Data);
            }
        }

        /// <summary>
        /// Analyze the received Bluetooth LE advertisement, and either add a new unique
        /// beacon to the list of known beacons, or update a previously known beacon
        /// with the new information.
        /// </summary>
        /// <param name="btAdv">Bluetooth advertisement to parse, as received from
        /// the Windows Bluetooth LE API.</param>
        private void ReceivedAdvertisement(BLEAdvertisementPacket btAdv)
        {
            if (btAdv == null) return;

            // Check if we already know this bluetooth address
            foreach (var bluetoothBeacon in BluetoothBeacons)
            {
                if (bluetoothBeacon.BluetoothAddress == btAdv.BluetoothAddress)
                {
                    // We already know this beacon
                    // Update / Add info to existing beacon
                    bluetoothBeacon.UpdateBeacon(btAdv);
                    return;
                }
            }

            // Beacon was not yet known - add it to the list.
            var newBeacon = new Beacon(btAdv);
            BluetoothBeacons.Add(newBeacon);
            BeaconAdded?.Invoke(this, newBeacon);
        }
    }
}
