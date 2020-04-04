using Covid19Radar.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Services
{
    public interface IBeaconService
    {
        /// <summary>
        /// Start Ranging Beacon
        /// </summary>
        /// <param name="beacons">Beacon List</param>
        void StartBeacon();

        /// <summary>
        /// Stop Ranging Beacon
        /// </summary>
        void StopBeacon();

  
    }
}
