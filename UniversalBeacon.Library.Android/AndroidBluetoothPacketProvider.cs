using System;
using Android.Bluetooth;
using Android.Content;
using UniversalBeacon.Library.Core.Interfaces;
using UniversalBeacon.Library.Core.Interop;

namespace UniversalBeacon.Library
{
    public class AndroidBluetoothPacketProvider : Java.Lang.Object, IBluetoothPacketProvider
    {        
        public event EventHandler<BLEAdvertisementPacketArgs> AdvertisementPacketReceived;
        public event EventHandler<BTError> WatcherStopped;

        private readonly BluetoothAdapter _adapter;
        private readonly BLEScanCallback _scanCallback;

        public AndroidBluetoothPacketProvider(Context context)
        {
            var manager = (BluetoothManager)context.GetSystemService("bluetooth");
            _adapter = manager.Adapter;
            _scanCallback = new BLEScanCallback();
        }

        private void ScanCallback_OnAdvertisementPacketReceived(object sender, BLEAdvertisementPacketArgs e)
        {
            AdvertisementPacketReceived?.Invoke(this, e);
        }

        public void Start()
        {
            _scanCallback.OnAdvertisementPacketReceived += ScanCallback_OnAdvertisementPacketReceived;
            _adapter.BluetoothLeScanner.StartScan(_scanCallback);
        }

        public void Stop()
        {
            _scanCallback.OnAdvertisementPacketReceived -= ScanCallback_OnAdvertisementPacketReceived;
            _adapter.CancelDiscovery();
        }
    }
}
