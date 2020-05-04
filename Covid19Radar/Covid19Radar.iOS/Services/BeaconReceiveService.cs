using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreBluetooth;
using CoreFoundation;
using CoreLocation;
using Covid19Radar.Common;
using Covid19Radar.iOS.Model;
using Covid19Radar.iOS.Services;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Foundation;
using Prism.Services;
using UIKit;
using Xamarin.Forms;

/*
[assembly: Dependency(typeof(BeaconReceiveService))]
namespace Covid19Radar.iOS.Services
{

    public class BeaconReceiveService : CBCentralManagerDelegate, IBeaconReceiveService
    {
        private static string BEACONS_REGION_HEADER = "beacons_";

        private readonly List<CLBeaconRegion> _listOfCLBeaconRegion;

        private static DispatchQueue beacon_operations_queue;
        private double _onLostTimeout = 15.0;
        private bool _shouldBeScanning = false;

        private readonly CBCentralManager _centralManager;

        private NSMutableDictionary _seenEddystoneCache;
        private Dictionary<NSUuid, NSData> _deviceIDCache;

        private Dictionary<string, bool> _regionsCache;

        private static CLLocationManager _locationManager;

        public BeaconReceiveService()
        {
            BEACONS_REGION_HEADER = "HEADERREGION";
            beacon_operations_queue = new DispatchQueue("beacon_operations_queue");

            _deviceIDCache = new Dictionary<NSUuid, NSData>();
            _seenEddystoneCache = new NSMutableDictionary();

            _centralManager = new CBCentralManager(this, beacon_operations_queue);
            _centralManager.Delegate = this;

            // _locationManager = new CLLocationManager();
            _listOfCLBeaconRegion = new List<CLBeaconRegion>();

            _regionsCache = new Dictionary<string, bool>();
        }

        public List<BeaconDataModel> GetBeaconData()
        {
            throw new NotImplementedException();
        }

        public async Task Start()
        {
            var result = await RequestPermissionAsync();
            if (!result)
                return;
            _locationManager.DidRangeBeacons += HandleDidRangeBeacons;
            _locationManager.DidDetermineState += HandleDidDetermineState;
            _locationManager.PausesLocationUpdatesAutomatically = false;
            _locationManager.StartUpdatingLocation();

            beacon_operations_queue.DispatchAsync(StartScanningSynchronized);

            //            var location = await BeaconsUtil.GetCurrentLocationAsync();
            //            var ibeacons = await BeaconsService.Instance.LoadBeaconsByUserLocation(location.Coordinate.Latitude, location.Coordinate.Longitude);


            //            foreach (var ibeacon in ibeacons)
            //            {

            //var clBeaconRegion = new CLBeaconRegion(new NSUuid(ibeacon.UUID), (ushort)ibeacon.Major, (ushort)ibeacon.Minor, $"{BEACONS_REGION_HEADER}.{ibeacon.ToString()}");
            var clBeaconRegion = new CLBeaconRegion(new NSUuid(AppConstants.iBeaconAppUuid), "");
            clBeaconRegion.NotifyEntryStateOnDisplay = true;
            clBeaconRegion.NotifyOnEntry = true;
            clBeaconRegion.NotifyOnExit = true;

            _listOfCLBeaconRegion.Add(clBeaconRegion);

            //   _locationManager.StartMonitoring(clBeaconRegion);
            //   _locationManager.StartRangingBeacons(clBeaconRegion);

            //            }
        }

        public async Task Stop()
        {
            _centralManager.StopScan();
            foreach (var beaconRegion in _listOfCLBeaconRegion)
            {
                _locationManager.StopRangingBeacons(beaconRegion);
                _locationManager.StopMonitoring(beaconRegion);
            }

            _listOfCLBeaconRegion.Clear();

            _locationManager.DidRangeBeacons -= HandleDidRangeBeacons;
            _locationManager.StopUpdatingLocation();
        }

        #region iBeacon monitoring

        private void HandleDidDetermineState(object sender, CLRegionStateDeterminedEventArgs e)
        {
            if (e.State == CLRegionState.Inside)
            {
                if (!_regionsCache.ContainsKey(e.Region.Identifier))
                    _regionsCache.Add(e.Region.Identifier, false);
            }
            else if (e.State == CLRegionState.Outside)
            {
                if (_regionsCache.ContainsKey(e.Region.Identifier))
                    _regionsCache.Remove(e.Region.Identifier);
            }
        }

        private void HandleDidRangeBeacons(object sender, CLRegionBeaconsRangedEventArgs e)
        {
            if (_regionsCache.ContainsKey(e.Region.Identifier) && !_regionsCache[e.Region.Identifier])
            {
                _regionsCache[e.Region.Identifier] = true;
            foreach (var beacon in e.Beacons)
            {
                string uuid = beacon.ProximityUuid.AsString();
                var major = (int)beacon.Major;
                var minor = (int)beacon.Minor;

                //SendBeaconChangeProximity(uuid, major, minor);
            }
            //          }
        }

        #endregion

        #region CBCentralManagerDelegate callbacks

        public override void UpdatedState(CBCentralManager central)
        {
            if (central.State == CBCentralManagerState.PoweredOn && _shouldBeScanning)
                StartScanningSynchronized();
        }

        public override void DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
        {
            if (advertisementData.ContainsKey(CBAdvertisement.DataServiceDataKey))
            {
                var tmp = advertisementData.ObjectForKey(CBAdvertisement.DataManufacturerDataKey);
                var serviceData = advertisementData.ValueForKey(CBAdvertisement.DataServiceDataKey) as NSDictionary;

                var eft = BeaconInfo.FrameTypeForFrame(serviceData);
                if (eft == EddystoneFrameType.UIDFrameType || eft == EddystoneFrameType.EIDFrameType)
                {
                    var telemetry = BeaconInfo.TelemetryDataForFrame(serviceData);
                    var rssi = RSSI.Int32Value;
                    var beaconInfo = (eft == EddystoneFrameType.UIDFrameType
                        ? BeaconInfo.BeaconInfoForUIDFrameData(serviceData, telemetry, rssi)
                        : BeaconInfo.BeaconInfoForEIDFrameData(serviceData, telemetry, rssi));
                    if (beaconInfo != null)
                    {
                        DidFindBeacon(beaconInfo);
                    }

                    if (eft == EddystoneFrameType.TelemetryFrameType)
                    {
                        if (!_deviceIDCache.ContainsKey(peripheral.Identifier)) {
                            _deviceIDCache.Add(peripheral.Identifier, BeaconInfo.TelemetryDataForFrame(serviceData));
                        }
                    }
                    else if (eft == EddystoneFrameType.UIDFrameType || eft == EddystoneFrameType.EIDFrameType)
                    {
                        if (peripheral.Identifier != null && _deviceIDCache.ContainsKey(peripheral.Identifier))
                        {
                            var telemetry = _deviceIDCache[peripheral.Identifier];
                            var rssi = RSSI.Int32Value;

                            var beaconInfo = (eft == EddystoneFrameType.UIDFrameType
                                ? BeaconInfo.BeaconInfoForUIDFrameData(serviceData, telemetry, rssi)
                                : BeaconInfo.BeaconInfoForEIDFrameData(serviceData, telemetry, rssi));

                            if (beaconInfo != null)
                            {
                                _deviceIDCache.Remove(peripheral.Identifier);

                                if (_seenEddystoneCache.ContainsKey(new NSString(beaconInfo.BeaconID.Description)))
                                {
                                    var timer = _seenEddystoneCache.ObjectForKey(new NSString(beaconInfo.BeaconID.Description + "_onLostTimer")) as NSTimer;
                                    timer.Invalidate();
                                    timer = NSTimer.CreateScheduledTimer(_onLostTimeout, t =>
                                    {
                                        var cacheKey = beaconInfo.BeaconID.Description;
                                        if (_seenEddystoneCache.ContainsKey(new NSString(cacheKey)))
                                        {
                                            var lostBeaconInfo = _seenEddystoneCache.ObjectForKey(new NSString(cacheKey)) as BeaconInfo;
                                            if (lostBeaconInfo != null)
                                            {
                                                _seenEddystoneCache.Remove(new NSString(beaconInfo.BeaconID.Description));
                                                _seenEddystoneCache.Remove(new NSString(beaconInfo.BeaconID.Description + "_onLostTimer"));

                                                DidLoseBeacon(lostBeaconInfo);
                                            }
                                        }
                                    });

                                    DidUpdateBeacon(beaconInfo);
                                }
                                else
                                {
                                    DidFindBeacon(beaconInfo);

                                    var timer = NSTimer.CreateScheduledTimer(_onLostTimeout, t =>
                                    {
                                        var cacheKey = beaconInfo.BeaconID.Description;
                                        if (_seenEddystoneCache.ContainsKey(new NSString(cacheKey)))
                                        {
                                            var lostBeaconInfo = _seenEddystoneCache.ObjectForKey(new NSString(cacheKey)) as BeaconInfo;
                                            if (lostBeaconInfo != null)
                                            {
                                                _seenEddystoneCache.Remove(new NSString(beaconInfo.BeaconID.Description));
                                                _seenEddystoneCache.Remove(new NSString(beaconInfo.BeaconID.Description + "_onLostTimer"));

                                                DidLoseBeacon(lostBeaconInfo);
                                            }
                                        }
                                    });

                                    _seenEddystoneCache.SetValueForKey(beaconInfo, new NSString(beaconInfo.BeaconID.Description));
                                    _seenEddystoneCache.SetValueForKey(timer, new NSString(beaconInfo.BeaconID.Description + "_onLostTimer"));
                                }
                            }
                        }
                    }
                    else if (eft == EddystoneFrameType.URLFrameType)
                    {
                        var rssi = RSSI.Int32Value;
                        var url = BeaconInfo.ParseUrlFromFrame(serviceData);

                        var name = peripheral.Name;
                        var services = peripheral.Services;

                        DidObserveURLBeacon(url, rssi);
                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }
        }
        #endregion
        #region Eddystone monitoring

        private void StartScanningSynchronized()
        {
            if (_centralManager.State != CBCentralManagerState.PoweredOn)
            {
                _shouldBeScanning = true;
                System.Diagnostics.Debug.WriteLine($"CentralManager state is {_centralManager.State}, cannot start scan");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Starting to scan for Eddystones");

                var peripheralUuids = new List<CBUUID>();
                peripheralUuids.Add(CBUUID.FromString("FEAA")); //eddystone service id

                //peripheralUuids.Add(CBUUID.FromString("0215")); //ibeacon service id, but don't work
                //peripheralUuids.Add(CBUUID.FromBytes(new byte[] { 0x02, 0x15 }));

                peripheralUuids.Add(CBUUID.FromBytes(new byte[] { 0x02, 0x01 }));
                peripheralUuids.Add(CBUUID.FromBytes(new byte[] { 0x02, 0x01, 0x1a, 0x1a }));
                peripheralUuids.Add(CBUUID.FromBytes(new byte[] { 0x1a, 0x1a }));
                peripheralUuids.Add(CBUUID.FromBytes(new byte[] { 0x4c, 0x00 }));
                peripheralUuids.Add(CBUUID.FromBytes(new byte[] { 0x4c, 0x00, 0x02, 0x15 }));

                // 02 01 1a 1a ff 4c 00 02 15 – #Apple's fixed iBeacon advertising prefix
                // start searching for all BLE devices and select among them those who have a sequence of bytes as in the previous line

                //https://github.com/AltBeacon/android-beacon-library/blob/master/src/main/java/org/altbeacon/beacon/BeaconParser.java
                //https://glimwormbeacons.com/learn/what-makes-an-ibeacon-an-ibeacon/
                //http://stackoverflow.com/questions/20387327/using-corebluetooth-with-ibeacons

                _centralManager.ScanForPeripherals(
                    peripheralUuids.ToArray(),
                    new PeripheralScanningOptions(NSDictionary.FromObjectAndKey(NSObject.FromObject(true), CBCentralManager.ScanOptionAllowDuplicatesKey))
                );
            }
        }

        private void DidUpdateBeacon(BeaconInfo beaconInfo)
        {

        }

        private async Task DidFindBeacon(BeaconInfo beaconInfo)
        {
            System.Diagnostics.Debug.WriteLine(Utils.SerializeToJson(beaconInfo.BeaconID));
        }

        private void DidLoseBeacon(BeaconInfo lostBeaconInfo)
        {

        }

        private async Task DidObserveURLBeacon(string url, int rssi)
        {
            System.Diagnostics.Debug.WriteLine(Utils.SerializeToJson(url));
            System.Diagnostics.Debug.WriteLine(Utils.SerializeToJson(rssi));
        }

        #endregion

        private Task<bool> RequestPermissionAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            EventHandler<CLAuthorizationChangedEventArgs> handler = null;
            handler = (object sender, CLAuthorizationChangedEventArgs e) =>
            {
                if (e.Status == CLAuthorizationStatus.AuthorizedAlways)
                {
                    //_locationManager.AuthorizationChanged -= handler;
                    tcs.TrySetResult(true);
                }
                else if (e.Status != CLAuthorizationStatus.NotDetermined)
                {
                    //_locationManager.AuthorizationChanged -= handler;
                    tcs.TrySetResult(false);
                }
            };

            //_locationManager.AuthorizationChanged += handler;

            //_locationManager.RequestAlwaysAuthorization();
            return tcs.Task;
        }
    }
}
*/