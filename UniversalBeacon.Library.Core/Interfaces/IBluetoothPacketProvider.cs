using System;
using UniversalBeacon.Library.Core.Interop;

namespace UniversalBeacon.Library.Core.Interfaces
{    
    public interface IBluetoothPacketProvider
    {
        /// <summary>
        /// Event is invoked whenever a new Bluetooth LE Advertisment Packet has been received.
        /// Usually handled directly by the library. If required, your app implementation can
        /// also subscribe to get notified about events.
        /// </summary>
        event EventHandler<BLEAdvertisementPacketArgs> AdvertisementPacketReceived;
        /// <summary>
        /// Wrapper for the Bluetooth LE Watcher stopped event of the underlying OS.
        /// Currently used by UWP platform and invoked whenever there has been an issue
        /// starting the Bluetooth LE watcher or if an issue occured while watching for
        /// beacons (e.g., if the user turned off Bluetooth while the app is running).
        /// </summary>
        event EventHandler<BTError> WatcherStopped;
        /// <summary>
        /// Start watching for Bluetooth beacons.
        /// </summary>
        void Start();
        /// <summary>
        /// Stop watching for Bluetooth beacons.
        /// </summary>
        void Stop();
    }
}
