using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreBluetooth;
using Foundation;
using UniversalBeacon.Library.Core.Interop;

namespace UniversalBeacon.Library
{
    internal class CocoaBluetoothCentralDelegate : CBCentralManagerDelegate
    {
        public event EventHandler<BLEAdvertisementPacketArgs> OnAdvertisementPacketReceived;

        #region CBCentralManagerDelegate

        public override void ConnectedPeripheral(CBCentralManager central, CBPeripheral peripheral)
        {
            Debug.WriteLine($"ConnectedPeripheral(CBCentralManager central, CBPeripheral {peripheral})");
        }

        public override void DisconnectedPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            Debug.WriteLine($"DisconnectedPeripheral(CBCentralManager central, CBPeripheral {peripheral}, NSError {error})");
        }

        public override void DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
        {
            Debug.WriteLine($"Cocoa peripheral {peripheral}");
            Debug.WriteLine($"Cocoa advertisementData {advertisementData}");
            Debug.WriteLine($"Cocoa RSSI {RSSI}");

            var bLEAdvertisementPacket = new BLEAdvertisementPacket()
            {
                Advertisement = new BLEAdvertisement()
                {
                    LocalName = peripheral.Name,
                    ServiceUuids = new List<Guid>(),
                    DataSections = new List<BLEAdvertisementDataSection>(),
                    ManufacturerData = new List<BLEManufacturerData>()
                },
                AdvertisementType = BLEAdvertisementType.ScanResponse,
                BluetoothAddress = (ulong)peripheral.Identifier.GetHashCode(),
                RawSignalStrengthInDBm = RSSI.Int16Value,
                Timestamp = DateTimeOffset.Now
            };

            //https://developer.apple.com/documentation/corebluetooth/cbadvertisementdataserviceuuidskey
            //if (advertisementData.ContainsKey(CBAdvertisement.DataServiceUUIDsKey))
            //{
            //    bLEAdvertisementPacket.Advertisement.ServiceUuids.Add(
            //        item: new BLEManufacturerData(packetType: BLEPacketType.UUID16List,
            //                                      data: (advertisementData[CBAdvertisement.DataServiceUUIDsKey])));
            //}

            //https://developer.apple.com/documentation/corebluetooth/cbadvertisementdataservicedatakey
            //if (advertisementData.ContainsKey(CBAdvertisement.DataServiceDataKey))
            //{
            //    bLEAdvertisementPacket.Advertisement.DataSections.Add(
            //        item: new BLEManufacturerData(packetType: BLEPacketType.ServiceData,
            //                                      data: advertisementData[CBAdvertisement.DataServiceDataKey]));
            //}

            //https://developer.apple.com/documentation/corebluetooth/cbadvertisementdatamanufacturerdatakey
            if (advertisementData.ContainsKey(CBAdvertisement.DataManufacturerDataKey))
            {
                bLEAdvertisementPacket.Advertisement.ManufacturerData.Add(
                    item: new BLEManufacturerData(packetType: BLEPacketType.ManufacturerData,
                                                  data: (advertisementData[CBAdvertisement.DataManufacturerDataKey]
                                                         as NSData).ToArray()));
            }

            // Missing CBAdvertisement.DataTxPowerLevelKey

            var bLEAdvertisementPacketArgs = new BLEAdvertisementPacketArgs(data: bLEAdvertisementPacket);
            OnAdvertisementPacketReceived?.Invoke(this, bLEAdvertisementPacketArgs);
        }

        public override void FailedToConnectPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            Debug.WriteLine($"FailedToConnectPeripheral(CBCentralManager central, CBPeripheral {peripheral}, NSError {error})");
        }

        public override void UpdatedState(CBCentralManager central)
        {
            switch (central.State)
            {
                case CBCentralManagerState.Unknown:
                    Debug.WriteLine("CBCentralManagerState.Unknown");
                    break;
                case CBCentralManagerState.Resetting:
                    Debug.WriteLine("CBCentralManagerState.Resetting");
                    break;
                case CBCentralManagerState.Unsupported:
                    Debug.WriteLine("CBCentralManagerState.Unsupported");
                    break;
                case CBCentralManagerState.Unauthorized:
                    Debug.WriteLine("CBCentralManagerState.Unauthorized");
                    break;
                case CBCentralManagerState.PoweredOff:
                    Debug.WriteLine("CBCentralManagerState.PoweredOff");
                    break;
                case CBCentralManagerState.PoweredOn:
                    Debug.WriteLine("CBCentralManagerState.PoweredOn");
                    central.ScanForPeripherals(peripheralUuids: new CBUUID[] { },
                                                               options: new PeripheralScanningOptions { AllowDuplicatesKey = true });
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void WillRestoreState(CBCentralManager central, NSDictionary dict)
        {
            Debug.WriteLine($"WillRestoreState(CBCentralManager central, NSDictionary {dict})");
        }

        #endregion CBCentralManagerDelegate
    }
}
