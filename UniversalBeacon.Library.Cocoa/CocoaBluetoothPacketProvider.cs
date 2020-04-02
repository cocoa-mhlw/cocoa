using System;
using System.Diagnostics;
using CoreBluetooth;
using Foundation;
using UniversalBeacon.Library.Core.Interfaces;
using UniversalBeacon.Library.Core.Interop;

namespace UniversalBeacon.Library
{
    public class CocoaBluetoothPacketProvider : NSObject, IBluetoothPacketProvider
    {
        public event EventHandler<BLEAdvertisementPacketArgs> AdvertisementPacketReceived;
        public event EventHandler<BTError> WatcherStopped;

        private readonly CocoaBluetoothCentralDelegate centralDelegate;
        private readonly CBCentralManager central;

        public CocoaBluetoothPacketProvider()
        {
            Debug.WriteLine("BluetoothPacketProvider()");

            centralDelegate = new CocoaBluetoothCentralDelegate();
            central = new CBCentralManager(centralDelegate, null);
        }

        private void ScanCallback_OnAdvertisementPacketReceived(object sender, BLEAdvertisementPacketArgs e)
        {
            AdvertisementPacketReceived?.Invoke(this, e);
        }

        public void Start()
        {
            Debug.WriteLine("BluetoothPacketProvider:Start()");
            centralDelegate.OnAdvertisementPacketReceived += ScanCallback_OnAdvertisementPacketReceived;

            // Wait for the PoweredOn state

            //if(CBCentralManagerState.PoweredOn == central.State) {
            //    central.ScanForPeripherals(peripheralUuids: new CBUUID[] { },
            //                                               options: new PeripheralScanningOptions { AllowDuplicatesKey = false });
            //}
        }

        public void Stop()
        {
            Debug.WriteLine("BluetoothPacketProvider:Stop()");
            centralDelegate.OnAdvertisementPacketReceived -= ScanCallback_OnAdvertisementPacketReceived;
 
            central.StopScan();
            WatcherStopped?.Invoke(sender: this, e: new BTError(BTError.BluetoothError.Success));
        }
    }
}