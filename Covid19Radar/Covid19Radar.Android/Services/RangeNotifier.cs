using AltBeaconOrg.BoundBeacon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Covid19Radar.Droid.Services
{
    public class RangeNotifier : Java.Lang.Object, IRangeNotifier
    {
        public event EventHandler<RangeEventArgs> DidRangeBeaconsInRegionComplete;

        public void DidRangeBeaconsInRegion(ICollection<Beacon> beacons, Region region)
        {
            DidRangeBeaconsInRegionComplete?.Invoke(this, new RangeEventArgs { Beacons = beacons, Region = region });
        }
    }
}