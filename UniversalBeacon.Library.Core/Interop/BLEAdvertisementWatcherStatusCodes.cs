using System;
using System.Collections.Generic;
using System.Text;

namespace UniversalBeacon.Library.Core.Interop
{
    /// <summary>
    /// Possible status codes of the Bluetooth LE Advertisment Watcher.
    /// Binary compatible to the UWP implementation.
    /// </summary>
    public enum BLEAdvertisementWatcherStatusCodes
    {
        //
        // Summary:
        //     The initial status of the watcher.
        Created = 0,
        //
        // Summary:
        //     The watcher is started.
        Started = 1,
        //
        // Summary:
        //     The watcher stop command was issued.
        Stopping = 2,
        //
        // Summary:
        //     The watcher is stopped.
        Stopped = 3,
        //
        // Summary:
        //     An error occurred during transition or scanning that stopped the watcher due
        //     to an error.
        Aborted = 4
    }
}
