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
        void StartRagingBeacons(UserDataModel userData);

        /// <summary>
        /// Stop Ranging Beacon
        /// </summary>
        void StopRagingBeacons();

        /// <summary>
        /// Start Advertising Beacon
        /// </summary>
        /// <param name="beacons">Beacon List</param>
        void StartAdvertisingBeacons(UserDataModel userData);

        /// <summary>
        /// Stop Advertising Beacon
        /// </summary>
        void StopAdvertisingBeacons();

        /// <summary>
        /// Get beacon data model
        /// </summary>
        List<BeaconDataModel> GetBeaconData();

    }
}
