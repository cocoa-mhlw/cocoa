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

[assembly: Dependency(typeof(Covid19Radar.iOS.Services.BeaconService))]
namespace Covid19Radar.iOS.Services
{
    public class BeaconService : IBeaconService
    {
        private CLBeaconRegion _fieldRegion;
        private Dictionary<string, BeaconDataModel> _dictionaryOfBeaconData;
        private CLLocationManager _beaconManager;
        private readonly List<CLBeaconRegion> _listOfCLBeaconRegion;

        public BeaconService()
        {
            _beaconManager = new CLLocationManager();
            _listOfCLBeaconRegion = new List<CLBeaconRegion>();
            _fieldRegion = new CLBeaconRegion(new NSUuid(AppConstants.AppUUID), "");
            _fieldRegion.NotifyEntryStateOnDisplay = true;
            _fieldRegion.NotifyOnEntry = true;
            _fieldRegion.NotifyOnExit = true;
            _dictionaryOfBeaconData = new Dictionary<string, BeaconDataModel>();
        }

        public CLLocationManager BeaconManagerImpl
        {
            get
            {
                if (_beaconManager == null)
                {
                    _beaconManager = InitializeBeaconManager();
                }
                return _beaconManager;
            }
        }

        public void InitializeService()
        {
            _beaconManager = InitializeBeaconManager();
        }

        private CLLocationManager InitializeBeaconManager()
        {
            // Enable the BeaconManager 
            _beaconManager = new CLLocationManager();

            #region Set up Beacon Simulator for TEST USE
            #endregion Set up Beacon Simulator for TEST USE

            _dictionaryOfBeaconData = new Dictionary<string, BeaconDataModel>();

            // BeaconManager Setting
            /*
            _beaconManager.SetForegroundScanPeriod(AppConstants.BEACONS_UPDATES_IN_MILLISECONDS);
            _beaconManager.SetForegroundBetweenScanPeriod(AppConstants.BEACONS_UPDATES_IN_MILLISECONDS);
            _beaconManager.SetBackgroundScanPeriod(AppConstants.BEACONS_UPDATES_IN_MILLISECONDS);
            _beaconManager.SetBackgroundBetweenScanPeriod(AppConstants.BEACONS_UPDATES_IN_MILLISECONDS);
            _beaconManager.UpdateScanPeriods();
            */

            _beaconManager.RequestAlwaysAuthorization();
            _beaconManager.DidRangeBeacons += HandleDidRangeBeacons;
            _beaconManager.DidFailRangingBeacons += HandleDidFailRangingBeacons;
            _beaconManager.DidRangeBeaconsSatisfyingConstraint += HandleDidRangeBeaconsSatisfyingConstraint;
            _beaconManager.DidStartMonitoringForRegion += HandleDidStartMonitoringForRegion;
            _beaconManager.DidVisit += HandeDidVisit;
            _beaconManager.DidDetermineState += HandleDidDetermineState;
            _beaconManager.RangingBeaconsDidFailForRegion += HandleRangingBeaconsDidFailForRegion;
            _beaconManager.RegionEntered += HandleRegionEntered;
            _beaconManager.RegionLeft += HadleRegionLeft;

            _beaconManager.PausesLocationUpdatesAutomatically = false;
            _beaconManager.StartUpdatingLocation();
            _beaconManager.RequestAlwaysAuthorization();

            _listOfCLBeaconRegion.Add(_fieldRegion);
            _beaconManager.StartMonitoring(_fieldRegion);
            _beaconManager.StartRangingBeacons(_fieldRegion);

            return _beaconManager;
        }

        private void HadleRegionLeft(object sender, CLRegionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("HadleRegionLeft");
        }

        private void HandleRegionEntered(object sender, CLRegionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("HandleRegionEntered");
        }

        private void HandleRangingBeaconsDidFailForRegion(object sender, CLRegionBeaconsFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("HandleRangingBeaconsDidFailForRegion");
        }

        private void HandeDidVisit(object sender, CLVisitedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("HandeDidVisit");
        }

        private void HandleDidStartMonitoringForRegion(object sender, CLRegionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("HandleDidStartMonitoringForRegion");
        }

        private void HandleDidRangeBeaconsSatisfyingConstraint(object sender, CLRegionBeaconsConstraintRangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("HandleDidRangeBeaconsSatisfyingConstraint");
        }

        private void HandleDidFailRangingBeacons(object sender, CLRegionBeaconsConstraintFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("HandleDidFailRangingBeacons");
        }

        public void StartBeacon()
        {
            System.Diagnostics.Debug.WriteLine("StartBeacon");

            _beaconManager.RequestAlwaysAuthorization();
            _beaconManager.DidRangeBeacons += HandleDidRangeBeacons;
            _beaconManager.DidDetermineState += HandleDidDetermineState;
            _beaconManager.PausesLocationUpdatesAutomatically = false;
            _beaconManager.StartUpdatingLocation();
            _beaconManager.RequestAlwaysAuthorization();

            _listOfCLBeaconRegion.Add(_fieldRegion);
            _beaconManager.StartMonitoring(_fieldRegion);
            _beaconManager.StartRangingBeacons(_fieldRegion);

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

            _beaconManager.StopRangingBeacons(_fieldRegion);
            _beaconManager.StopMonitoring(_fieldRegion);

            _listOfCLBeaconRegion.Clear();
            _beaconManager.DidRangeBeacons -= HandleDidRangeBeacons;
            _beaconManager.StopUpdatingLocation();
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
            return _dictionaryOfBeaconData;
        }

        public void StartAdvertising(UserDataModel userData)
        {
        }

        public void StopAdvertising()
        {
            //throw new NotImplementedException();
        }


    }
}
