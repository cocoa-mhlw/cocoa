using AltBeaconOrg.BoundBeacon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Covid19Radar.Droid.Services
{
    public class MonitorEventArgs : EventArgs
    {
        public Region Region { get; set; }
        public int State { get; set; }
    }
}