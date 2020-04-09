using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreLocation;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Foundation;
using UIKit;
using Xamarin.Forms;

//[assembly: Dependency(typeof(Covid19Radar.iOS.Model.iBeaconService))]

namespace Covid19Radar.iOS.Services
{
    public class BeaconService : IBeaconService
    {
        private readonly CLLocationManager _locationMgr;
        private readonly List<CLBeaconRegion> _listOfCLBeaconRegion;
        private Dictionary<string, BeaconDataModel> _dictionaryOfBeaconData;
        private List<BeaconDataModel> _beacons;
        private CLBeaconRegion clBeaconRegion;
        public BeaconService()
        {
            _locationMgr = new CLLocationManager();
            _listOfCLBeaconRegion = new List<CLBeaconRegion>();
            clBeaconRegion= new CLBeaconRegion(new NSUuid(AppConstants.AppUUID), "");
            clBeaconRegion.NotifyEntryStateOnDisplay = true;
            clBeaconRegion.NotifyOnEntry = true;
            clBeaconRegion.NotifyOnExit = true;

        }

        public void StartBeacon()
        {
            System.Diagnostics.Debug.WriteLine("StartBeacon");

            _locationMgr.RequestAlwaysAuthorization();
            _locationMgr.DidRangeBeacons += HandleDidRangeBeacons;
            _locationMgr.DidDetermineState += HandleDidDetermineState;
            _locationMgr.PausesLocationUpdatesAutomatically = false;
            _locationMgr.StartUpdatingLocation();
            _locationMgr.RequestAlwaysAuthorization();

                _listOfCLBeaconRegion.Add(clBeaconRegion);
                _locationMgr.StartMonitoring(clBeaconRegion);
                _locationMgr.StartRangingBeacons(clBeaconRegion);

            //            #if DEBUG
            //            var beacon = _listOfCLBeaconRegion.First();
            //            string uuid = beacon.ProximityUuid.AsString ();
            //            var major = (ushort)beacon.Major;
            //            var minor = (ushort)beacon.Minor;
            //
            //            Mvx.Resolve<IMvxMessenger>().Publish<BeaconChangeProximityMessage> (
            //                new BeaconChangeProximityMessage (this, uuid, major, minor)
            //            );
            //            #endif
        }

        public void StopBeacon()
        {
            System.Diagnostics.Debug.WriteLine("StopBeacon");

                _locationMgr.StopRangingBeacons(clBeaconRegion);
                _locationMgr.StopMonitoring(clBeaconRegion);

            _listOfCLBeaconRegion.Clear();
            _locationMgr.DidRangeBeacons -= HandleDidRangeBeacons;
            _locationMgr.StopUpdatingLocation();
        }

        #region Beacon monitoring

        private void HandleDidDetermineState(object sender, CLRegionStateDeterminedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("HandleDidDetermineState");
        }

        private void HandleDidRangeBeacons(object sender, CLRegionBeaconsRangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("HandleDidDetermineState");
            foreach (var beacon in e.Beacons)
            {
                SendBeaconChangeProximity(beacon);
            }
        }

        private void SendBeaconChangeProximity(CLBeacon beacon)
        {
            System.Diagnostics.Debug.WriteLine("SendBeaconChangeProximity");

            if (beacon.Proximity == CLProximity.Unknown)
            {
                System.Diagnostics.Debug.WriteLine(CLProximity.Unknown);
                return;
            }

            // TODO ビーコン追加
            string uuid = beacon.ProximityUuid.AsString();
            var major = (ushort)beacon.Major;
            var minor = (ushort)beacon.Minor;

            System.Diagnostics.Debug.WriteLine(beacon.ToString());

        }

        #endregion


        public Dictionary<string, BeaconDataModel> GetBeaconData()
        {
            throw new NotImplementedException();
        }

        public void InitializeService()
        {
        }

        public void StartAdvertising(UserData userData)
        {
            //throw new NotImplementedException();
        }

        public void StopAdvertising()
        {
            //throw new NotImplementedException();
        }


    }
}
