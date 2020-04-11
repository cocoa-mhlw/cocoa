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

            _dictionaryOfBeaconData = new Dictionary<string, BeaconDataModel>();

            // BeaconManager Setting

            // Monitoring
            _beaconManager.DidDetermineState += DetermineStateForRegionComplete;
            _beaconManager.RegionEntered += EnterRegionComplete;
            _beaconManager.RegionLeft += ExitRegionComplete;


            _beaconManager.RequestAlwaysAuthorization();
            _beaconManager.DidRangeBeacons += DidRangeBeconsInRegionComplete;

            _beaconManager.AuthorizationChanged += HandleAuthorizationChanged;
            return _beaconManager;
        }

        public Dictionary<string, BeaconDataModel> GetBeaconData()
        {
            return _dictionaryOfBeaconData;
        }

        public void StartBeacon()
        {
            System.Diagnostics.Debug.WriteLine("StartBeacon");

            _beaconManager.RequestAlwaysAuthorization();
            _beaconManager.PausesLocationUpdatesAutomatically = false;

            _listOfCLBeaconRegion.Add(_fieldRegion);
            _beaconManager.StartMonitoring(_fieldRegion);
        }

        public void StopBeacon()
        {
            System.Diagnostics.Debug.WriteLine("StopBeacon");

            _beaconManager.StopRangingBeacons(_fieldRegion);
            _beaconManager.StopMonitoring(_fieldRegion);
        }

        public void StartAdvertising(UserDataModel userData)
        {

        }

        public void StopAdvertising()
        {
            //throw new NotImplementedException();
        }

        private void DidRangeBeconsInRegionComplete(object sender, CLRegionBeaconsRangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("HandleDidDetermineState");

            foreach (var beacon in e.Beacons)
            {
                if (beacon.Proximity == CLProximity.Unknown)
                {
                    return;
                }
                var key = beacon.Uuid.ToString() + beacon.Major.ToString() + beacon.Minor.ToString();
                BeaconDataModel data = new BeaconDataModel();
                if (!_dictionaryOfBeaconData.ContainsKey(key))
                {
                    data.BeaconUuid = beacon.Uuid.ToString();
                    data.Major = beacon.Major.ToString();
                    data.Minor = beacon.Minor.ToString();
                    data.Distance = beacon.Accuracy;
                    data.Rssi = (short)beacon.Rssi;
                    //                        data.TXPower = beacon.tr;
                    data.ElaspedTime = new TimeSpan();
                    data.LastDetectTime = DateTime.Now;
                }
                else
                {
                    data = _dictionaryOfBeaconData.GetValueOrDefault(key);
                    data.BeaconUuid = beacon.Uuid.ToString();
                    data.Major = beacon.Major.ToString();
                    data.Minor = beacon.Minor.ToString();
                    data.Distance = beacon.Accuracy;
                    data.Rssi = (short)beacon.Rssi;
                    //                        data.TXPower = beacon.tr;
                    data.ElaspedTime = new TimeSpan();
                    data.LastDetectTime = DateTime.Now;
                    _dictionaryOfBeaconData.Remove(key);

                }

                _dictionaryOfBeaconData.Add(key, data);
                System.Diagnostics.Debug.WriteLine(key.ToString());
                System.Diagnostics.Debug.WriteLine(data.Distance);
                System.Diagnostics.Debug.WriteLine(data.ElaspedTime.TotalSeconds);
            }

        }

        #region beacon monitoring
        private void DetermineStateForRegionComplete(object sender, CLRegionStateDeterminedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DetermineStateForRegionComplete");
            System.Diagnostics.Debug.WriteLine(e.ToString());
            _beaconManager.StartRangingBeacons(_fieldRegion);
        }

        private void EnterRegionComplete(object sender, CLRegionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("EnterRegionComplete ---- StartRanging");
            System.Diagnostics.Debug.WriteLine(e.ToString());
            _beaconManager.StartRangingBeacons(_fieldRegion);

        }

        private void ExitRegionComplete(object sender, CLRegionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ExitRegionComplete ---- StopRanging");
            System.Diagnostics.Debug.WriteLine(e.ToString());
            _beaconManager.StopRangingBeacons(_fieldRegion);

        }

        #endregion
        #region Auth

        private void HandleAuthorizationChanged(object sender, CLAuthorizationChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("HandleAuthorizationChanged");
            // Do That Stop beacons
        }

        #endregion
    }
}
