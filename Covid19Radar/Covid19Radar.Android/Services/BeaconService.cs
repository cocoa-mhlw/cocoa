using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltBeaconOrg.BoundBeacon;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Covid19Radar.Common;
using Covid19Radar.Droid.Services;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(BeaconService))]
namespace Covid19Radar.Droid.Services
{
    public class BeaconService : IBeaconService
    {

        private AltBeaconOrg.BoundBeacon.Region _fieldRegion;

        private RangeNotifier _rangeNotifier;
        private MonitorNotifier _monitorNotifier;
        private Dictionary<string, BeaconDataModel> _dictionaryOfBeaconData;
        private BeaconManager _beaconManager;
        private readonly MainActivity _mainActivity;
        private BeaconTransmitter _beaconTransmitter;

        public BeaconService()
        {
            _mainActivity = MainActivity.Instance;
            _monitorNotifier = new MonitorNotifier();
            _rangeNotifier = new RangeNotifier();
            _dictionaryOfBeaconData = new Dictionary<string, BeaconDataModel>();
            /*
            _beaconManager.SetForegroundScanPeriod(BEACONS_UPDATES_IN_MILLISECONDS);
            _beaconManager.SetForegroundBetweenScanPeriod(BEACONS_UPDATES_IN_MILLISECONDS);

            _beaconManager.SetBackgroundScanPeriod(BEACONS_UPDATES_IN_MILLISECONDS);
            _beaconManager.SetBackgroundBetweenScanPeriod(BEACONS_UPDATES_IN_MILLISECONDS);

            _beaconManager.UpdateScanPeriods();

            // MonitorNotifier
            _monitorNotifier.DetermineStateForRegionComplete += DetermineStateForRegionComplete;
            _monitorNotifier.EnterRegionComplete += EnterRegionComplete;
            _monitorNotifier.ExitRegionComplete += ExitRegionComplete;
            _beaconManager.AddMonitorNotifier(_monitorNotifier);

            // RangeNotifier
            _rangeNotifier.DidRangeBeaconsInRegionComplete += DidRangeBeaconsInRegionComplete;
            _beaconManager.AddRangeNotifier(_rangeNotifier);

            _fieldRegion = new AltBeaconOrg.BoundBeacon.Region(AppConstants.AppUUID, null, null, null);
            */
        }

        public BeaconManager BeaconManagerImpl
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

        private BeaconManager InitializeBeaconManager()
        {
            // Enable the BeaconManager 
            _beaconManager = BeaconManager.GetInstanceForApplication(_mainActivity);

            #region Set up Beacon Simulator for TEST USE
            // Beacon Simulator
            var beaconSimulator = new BeaconSimulator();
            beaconSimulator.CreateBasicSimulatedBeacons();
            BeaconManager.BeaconSimulator = beaconSimulator;
            // Beacon Simulator
            #endregion Set up Beacon Simulator for TEST USE

            _monitorNotifier = new MonitorNotifier();
            _rangeNotifier = new RangeNotifier();
            _dictionaryOfBeaconData = new Dictionary<string, BeaconDataModel>();

            //iBeacon
            BeaconParser beaconParser = new BeaconParser().SetBeaconLayout(AppConstants.IBEACON_FORMAT);
            _beaconManager.BeaconParsers.Add(beaconParser);


            // BeaconManager Setting
            /*
            _beaconManager.SetForegroundScanPeriod(AppConstants.BEACONS_UPDATES_IN_MILLISECONDS);
            _beaconManager.SetForegroundBetweenScanPeriod(AppConstants.BEACONS_UPDATES_IN_MILLISECONDS);
            _beaconManager.SetBackgroundScanPeriod(AppConstants.BEACONS_UPDATES_IN_MILLISECONDS);
            _beaconManager.SetBackgroundBetweenScanPeriod(AppConstants.BEACONS_UPDATES_IN_MILLISECONDS);
            _beaconManager.UpdateScanPeriods();
            */

            // MonitorNotifier
            _monitorNotifier.DetermineStateForRegionComplete += DetermineStateForRegionComplete;
            _monitorNotifier.EnterRegionComplete += EnterRegionComplete;
            _monitorNotifier.ExitRegionComplete += ExitRegionComplete;
            _beaconManager.AddMonitorNotifier(_monitorNotifier);

            // RangeNotifier
            _rangeNotifier.DidRangeBeaconsInRegionComplete += DidRangeBeaconsInRegionComplete;
            _beaconManager.AddRangeNotifier(_rangeNotifier);


            _fieldRegion = new AltBeaconOrg.BoundBeacon.Region(AppConstants.AppUUID, null, null, null);
            _beaconManager.Bind(_mainActivity);
            return _beaconManager;
        }

        public Dictionary<string, BeaconDataModel> GetBeaconData()
        {
            return _dictionaryOfBeaconData;
        }




        public void StartBeacon()
        {
            BeaconManagerImpl.SetForegroundBetweenScanPeriod(AppConstants.BEACONS_UPDATES_IN_MILLISECONDS);
            BeaconManagerImpl.AddMonitorNotifier(_monitorNotifier);
            BeaconManagerImpl.AddRangeNotifier(_rangeNotifier);

            _beaconManager.StartMonitoringBeaconsInRegion(_fieldRegion);
            _beaconManager.StartRangingBeaconsInRegion(_fieldRegion);
        }



        public void StopBeacon()
        {
            _beaconManager.StopMonitoringBeaconsInRegion(_fieldRegion);
            _beaconManager.StopRangingBeaconsInRegion(_fieldRegion);
            _beaconManager.Unbind(_mainActivity);
        }

        public void StopAdvertising()
        {
            _beaconTransmitter.StopAdvertising();
        }

        public void StartAdvertising(UserData userData)
        {

            // TODO 出力調整どうするか考える。

            Beacon beacon = new Beacon.Builder()
                                .SetId1(AppConstants.AppUUID)
                                .SetId2(userData.Major)
                                .SetId3(userData.Minor)
                                .SetTxPower(-59)
                                .SetManufacturer(AppConstants.COMPANY_CODE_APPLE)
                                .Build();

            // iBeaconのバイト列フォーマットをBeaconParser（アドバタイズ時のバイト列定義）にセットする。
            BeaconParser beaconParser = new BeaconParser().SetBeaconLayout(AppConstants.IBEACON_FORMAT);

            // iBeaconの発信を開始する。
            _beaconTransmitter = new BeaconTransmitter(_mainActivity, beaconParser);
            _beaconTransmitter.StartAdvertising(beacon);

        }

        private void DidRangeBeaconsInRegionComplete(object sender, RangeEventArgs rangeEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("DidRangeBeaconsInRegionComplete");

            ICollection<Beacon> beacons = rangeEventArgs.Beacons;
            if (beacons != null && beacons.Count > 0)
            {
                var foundBeacons = beacons.ToList();
                foreach (Beacon beacon in foundBeacons)
                {
                    var key = beacon.Id1.ToString() + beacon.Id2.ToString() + beacon.Id3.ToString();
                    BeaconDataModel data = new BeaconDataModel();
                    if (!_dictionaryOfBeaconData.ContainsKey(key))
                    {
                        data.UUID = beacon.Id1.ToString();
                        data.Major = beacon.Id2.ToString();
                        data.Minor = beacon.Id3.ToString();
                        data.Distance = beacon.Distance;
                        data.Rssi = beacon.Rssi;
                        data.TXPower = beacon.TxPower;
                        data.ElaspedTime = new TimeSpan();
                        data.LastDetectTime = DateTime.Now;
                    }
                    else
                    {
                        data = _dictionaryOfBeaconData.GetValueOrDefault(key);
                        data.UUID = beacon.Id1.ToString();
                        data.Major = beacon.Id2.ToString();
                        data.Minor = beacon.Id3.ToString();
                        data.Distance = beacon.Distance;
                        data.Rssi = beacon.Rssi;
                        data.TXPower = beacon.TxPower;
                        data.ElaspedTime += DateTime.Now - data.LastDetectTime;
                        data.LastDetectTime = DateTime.Now;
                        _dictionaryOfBeaconData.Remove(key);

                    }

                    _dictionaryOfBeaconData.Add(key, data);
                    System.Diagnostics.Debug.WriteLine(key.ToString());
                    System.Diagnostics.Debug.WriteLine(data.Distance);
                    System.Diagnostics.Debug.WriteLine(data.ElaspedTime.TotalSeconds);
                }
            }
        }

        private void DetermineStateForRegionComplete(object sender, MonitorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DetermineStateForRegionComplete");
            System.Diagnostics.Debug.WriteLine(e.ToString());
        }

        private void EnterRegionComplete(object sender, MonitorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("EnterRegionComplete ---- StartRanging");
            System.Diagnostics.Debug.WriteLine(e.ToString());

            //_beaconManager.StartRangingBeaconsInRegion(_fieldRegion);
        }

        private void ExitRegionComplete(object sender, MonitorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ExitRegionComplete ---- StopRanging");
            System.Diagnostics.Debug.WriteLine(e.ToString());

            //_beaconManager.StopRangingBeaconsInRegion(_fieldRegion);
        }

    }
}

