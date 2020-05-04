using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreLocation;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Foundation;
using CoreBluetooth;
using UIKit;
using Xamarin.Forms;
using SQLite;
using Xamarin.Forms.Internals;

[assembly: Dependency(typeof(Covid19Radar.iOS.Services.BeaconService))]
namespace Covid19Radar.iOS.Services
{
    public class BeaconService : CLLocationManagerDelegate, IBeaconService
    {
        private static object dataLock = new object();
        private UserDataModel _userData;
        private bool _transmitterFlg = false;
        private CBPeripheralManager _beaconTransmitter = new CBPeripheralManager();
        private CLBeaconRegion _fieldRegion;
        private readonly CLLocationManager _locationManager;
        private List<CLBeaconRegion> _listOfCLBeaconRegion;
        private readonly SQLiteConnection _connection;

        public BeaconService()
        {
            _connection = DependencyService.Resolve<SQLiteConnectionProvider>().GetConnection();
            _connection.CreateTable<BeaconDataModel>();

            _beaconTransmitter = new CBPeripheralManager();
            _beaconTransmitter.StateUpdated += DidStateUpdated;

            _listOfCLBeaconRegion = new List<CLBeaconRegion>();
            _fieldRegion = new CLBeaconRegion(new NSUuid(AppConstants.iBeaconAppUuid), "");
            _fieldRegion.NotifyEntryStateOnDisplay = true;
            _fieldRegion.NotifyOnEntry = true;
            _fieldRegion.NotifyOnExit = true;

            // Monitoring
            _locationManager = new CLLocationManager();
            if (CLLocationManager.LocationServicesEnabled)
            {
                _locationManager.Delegate = this;
                _locationManager.PausesLocationUpdatesAutomatically = false;
                _locationManager.ShowsBackgroundLocationIndicator = true;
                _locationManager.DistanceFilter = 1.0;
                _locationManager.AllowsBackgroundLocationUpdates = true;
                _locationManager.DidRangeBeacons += DidRangeBeconsInRegionComplete;
                _locationManager.AuthorizationChanged += HandleAuthorizationChanged;
                _locationManager.RequestAlwaysAuthorization();
            }

        }

        public List<BeaconDataModel> GetBeaconData()
        {
            return _connection.Table<BeaconDataModel>().ToList();
        }

        public void OnSleep()
        {
            _locationManager.StartMonitoringSignificantLocationChanges();
        }

        public void OnResume()
        {
            _locationManager.StopMonitoringSignificantLocationChanges();
        }

        private void DidStateUpdated(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DidStateUpdated");
            CBPeripheralManager trasmitter = sender as CBPeripheralManager;

            if (trasmitter.State < CBPeripheralManagerState.PoweredOn)
            {
                System.Diagnostics.Debug.WriteLine("Bluetooth must be enabled");
                // new UIAlertView("Bluetooth must be enabled", "To configure your device as a beacon", null, "OK", null).Show();
                return;
            }

            if (_transmitterFlg)
            {
                CLBeaconRegion region = new CLBeaconRegion(new NSUuid(AppConstants.iBeaconAppUuid), ushort.Parse(_userData.Major), ushort.Parse(_userData.Minor), _userData.UserUuid);
                NSNumber txPower = new NSNumber(-59);
                trasmitter.StartAdvertising(region.GetPeripheralData(txPower));
            }
            else
            {
                trasmitter.StopAdvertising();
            }
        }

        private async void DidRangeBeconsInRegionComplete(object sender, CLRegionBeaconsRangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("HandleDidDetermineState");
            var now = DateTime.UtcNow;
            var keyTime = now.ToString("yyyyMMddHH");

            foreach (var beacon in e.Beacons)
            {
                if (beacon.Proximity == CLProximity.Unknown)
                {
                    return;
                }
                string key;
                try
                {
                    key = $"{beacon.Uuid}{beacon.Major}{beacon.Minor}.{keyTime}";
                }
                catch { continue; }
                lock (dataLock)
                {
                    var result = _connection.Table<BeaconDataModel>().SingleOrDefault(x => x.Id == key);
                    if (result == null)
                    {
                        BeaconDataModel data = new BeaconDataModel();
                        data.Id = key;
                        data.Count = 0;
                        data.UserBeaconUuid = AppConstants.iBeaconAppUuid;
                        data.BeaconUuid = beacon.Uuid.ToString();
                        data.Major = beacon.Major.ToString();
                        data.Minor = beacon.Minor.ToString();
                        data.Distance = beacon.Accuracy;
                        data.MinDistance = beacon.Accuracy;
                        data.MaxDistance = beacon.Accuracy;
                        data.Rssi = (short)beacon.Rssi;
                        //                        data.TXPower = beacon.tr;
                        data.ElaspedTime = new TimeSpan();
                        data.LastDetectTime = now;
                        data.FirstDetectTime = now;
                        data.KeyTime = keyTime;
                        data.IsSentToServer = false;
                        _connection.Insert(data);
                    }
                    else
                    {
                        BeaconDataModel data = result;
                        data.Id = key;
                        data.Count++;
                        data.UserBeaconUuid = AppConstants.iBeaconAppUuid;
                        data.BeaconUuid = beacon.Uuid.ToString();
                        data.Major = beacon.Major.ToString();
                        data.Minor = beacon.Minor.ToString();
                        data.Distance += (beacon.Accuracy - data.Distance) / data.Count;
                        data.MinDistance = (beacon.Accuracy < data.MinDistance ? beacon.Accuracy : data.MinDistance);
                        data.MaxDistance = (beacon.Accuracy > data.MaxDistance ? beacon.Accuracy : data.MaxDistance);
                        data.Rssi = (short)beacon.Rssi;
                        //                        data.TXPower = beacon.tr;
                        data.ElaspedTime += now - data.LastDetectTime;
                        data.LastDetectTime = now;
                        data.KeyTime = keyTime;
                        data.IsSentToServer = false;
                        _connection.Update(data);
                        System.Diagnostics.Debug.WriteLine(Utils.SerializeToJson(data));
                    }
                }

            }

        }

        #region Auth

        private void HandleAuthorizationChanged(object sender, CLAuthorizationChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("HandleAuthorizationChanged");
            // Do That Stop beacons
            if (e.Status == CLAuthorizationStatus.NotDetermined)
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                {
                    if (UIDevice.CurrentDevice.CheckSystemVersion(13, 4))
                    {
                        _locationManager.RequestWhenInUseAuthorization();
                    }
                    else
                    {
                        _locationManager.RequestAlwaysAuthorization();
                    }
                }

            }
            else if (e.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
            {
                _locationManager.RequestAlwaysAuthorization();
            }
        }

        public void StartRagingBeacons(UserDataModel userData)
        {
            System.Diagnostics.Debug.WriteLine("StartBeacon");

            _listOfCLBeaconRegion.Add(_fieldRegion);
            _locationManager.StartRangingBeacons(_fieldRegion);
        }

        public void StopRagingBeacons()
        {
            System.Diagnostics.Debug.WriteLine("StopBeacon");
            _locationManager.StopRangingBeacons(_fieldRegion);

        }

        public void StartAdvertisingBeacons(UserDataModel userData)
        {
            _userData = userData;
            _transmitterFlg = true;
        }

        public void StopAdvertisingBeacons()
        {
            _transmitterFlg = false;
        }

        #endregion
    }
}
