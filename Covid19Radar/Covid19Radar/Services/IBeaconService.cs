using Covid19Radar.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Services
{
    public interface IBeaconService
    {

        void InitializeService();

        /// <summary>
        /// Start Ranging Beacon
        /// </summary>
        /// <param name="beacons">Beacon List</param>
        void StartBeacon();

        /// <summary>
        /// Stop Ranging Beacon
        /// </summary>
        void StopBeacon();

        /// <summary>
        /// Start Advertising Beacon
        /// </summary>
        /// <param name="beacons">Beacon List</param>
        void StartAdvertising(UserDataModel userData);

        /// <summary>
        /// Stop Advertising Beacon
        /// </summary>
        void StopAdvertising();

        /// <summary>
        /// Get beacon data model
        /// </summary>
        List<BeaconDataModel> GetBeaconData();
    }
}
