using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltBeaconOrg.BoundBeacon;
using AltBeaconOrg.BoundBeacon.Startup;
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
using Java.Util;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(BeaconService))]
namespace Covid19Radar.Droid.Services
{
    public class BeaconService : IBeaconService, IDisposable
    {

        private AltBeaconOrg.BoundBeacon.Region _fieldRegion;
        private UserDataModel _userData;
        private RangeNotifier _rangeNotifier;
        private MonitorNotifier _monitorNotifier;
        private BeaconManager _beaconManager;
        private readonly MainActivity _mainActivity;
        private BeaconTransmitter _beaconTransmitter;
        private readonly SQLiteConnection _connection;
        private readonly UserDataService _userDataService;
        private readonly HttpDataService _httpDataService;
        private readonly MinutesTimer _uploadTimer;

        public BeaconService()
        {
            _mainActivity = MainActivity.Instance;
            _connection = MainActivity.sqliteConnectionProvider.GetConnection();
            _connection.CreateTable<BeaconDataModel>();
            _userDataService = DependencyService.Resolve<UserDataService>();
            _userData = _userDataService.Get();
            _httpDataService = DependencyService.Resolve<HttpDataService>();
            _uploadTimer = new MinutesTimer(_userData.GetJumpHashTimeDifference());
            _uploadTimer.Start();
            _uploadTimer.TimeOutEvent += TimerUpload;
        }

        private async void TimerUpload(EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString());
            List<BeaconDataModel> beacons = _connection.Table<BeaconDataModel>().ToList();
            foreach (var beacon in beacons)
            {
                await _httpDataService.PostBeaconDataAsync(_userData, beacon);
            }
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

            /*
            #region Set up Beacon Simulator for TEST USE
            // Beacon Simulator
            var beaconSimulator = new BeaconSimulator();
            beaconSimulator.CreateBasicSimulatedBeacons();
            BeaconManager.BeaconSimulator = beaconSimulator;
            // Beacon Simulator
            #endregion Set up Beacon Simulator for TEST USE
            */

            _monitorNotifier = new MonitorNotifier();
            _rangeNotifier = new RangeNotifier();

            //iBeacon
            BeaconParser beaconParser = new BeaconParser().SetBeaconLayout(AppConstants.iBeaconFormat);
            _beaconManager.BeaconParsers.Add(beaconParser);

            // BeaconManager Setting
            // Check Touch おそらくmain activity beacon consumer側で設定
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


            _fieldRegion = new AltBeaconOrg.BoundBeacon.Region("AppAppApp", Identifier.Parse(AppConstants.iBeaconAppUuid), null, null);

            _beaconManager.Bind(_mainActivity);
            return _beaconManager;
        }

        public List<BeaconDataModel> GetBeaconData()
        {
            return _connection.Table<BeaconDataModel>().ToList();
        }


        public void StartBeacon()
        {
            BeaconManagerImpl.SetForegroundBetweenScanPeriod(AppConstants.BeaconsUpdateInMillisec);
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

        public void StartAdvertising(UserDataModel userData)
        {

            // TODO 出力調整どうするか考える。

            Beacon beacon = new Beacon.Builder()
                                .SetId1(AppConstants.iBeaconAppUuid)
                                .SetId2(userData.Major)
                                .SetId3(userData.Minor)
                                .SetTxPower(-59)
                                .SetManufacturer(AppConstants.CompanyCoreApple)
                                .Build();

            // iBeaconのバイト列フォーマットをBeaconParser（アドバタイズ時のバイト列定義）にセットする。
            BeaconParser beaconParser = new BeaconParser().SetBeaconLayout(AppConstants.iBeaconFormat);

            // iBeaconの発信を開始する。
            _beaconTransmitter = new BeaconTransmitter(_mainActivity, beaconParser);
            _beaconTransmitter.StartAdvertising(beacon);

        }

        private async void DidRangeBeaconsInRegionComplete(object sender, RangeEventArgs rangeEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("DidRangeBeaconsInRegionComplete");

            ICollection<Beacon> beacons = rangeEventArgs.Beacons;
            if (beacons != null && beacons.Count > 0)
            {
                var foundBeacons = beacons.ToList();
                foreach (Beacon beacon in foundBeacons)
                {
                    var key = beacon.Id1.ToString() + beacon.Id2.ToString() + beacon.Id3.ToString();
                    var result = _connection.Table<BeaconDataModel>().SingleOrDefault(x => x.Id == key);
                    if (result == null)
                    {
                        // New
                        BeaconDataModel data = new BeaconDataModel();
                        data.Id = key;
                        data.Count = 0;
                        data.BeaconUuid = beacon.Id1.ToString();
                        data.Major = beacon.Id2.ToString();
                        data.Minor = beacon.Id3.ToString();
                        data.Distance = beacon.Distance;
                        data.Rssi = beacon.Rssi;
                        //                       data.TXPower = beacon.TxPower;
                        data.ElaspedTime = new TimeSpan();
                        data.LastDetectTime = DateTime.Now;
                        _connection.Insert(data);
                    }
                    else
                    {
                        // Update
                        BeaconDataModel data = result;
                        data.Id = key;
                        data.Count++;
                        data.BeaconUuid = beacon.Id1.ToString();
                        data.Major = beacon.Id2.ToString();
                        data.Minor = beacon.Id3.ToString();
                        data.Distance += (beacon.Distance - data.Distance) / data.Count;
                        data.Rssi = beacon.Rssi;
                        //                        data.TXPower = beacon.TxPower;
                        data.ElaspedTime += DateTime.Now - data.LastDetectTime;
                        data.LastDetectTime = DateTime.Now;
                        _connection.Update(data);
                        System.Diagnostics.Debug.WriteLine(Utils.SerializeToJson(data));
                    }
                }
            }
        }

        private void DetermineStateForRegionComplete(object sender, MonitorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DetermineStateForRegionComplete");
            System.Diagnostics.Debug.WriteLine(e.ToString());
            _beaconManager.StartRangingBeaconsInRegion(_fieldRegion);
        }

        private void EnterRegionComplete(object sender, MonitorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("EnterRegionComplete ---- StartRanging");
            System.Diagnostics.Debug.WriteLine(e.ToString());
            _beaconManager.StartRangingBeaconsInRegion(_fieldRegion);
        }

        private void ExitRegionComplete(object sender, MonitorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ExitRegionComplete ---- StopRanging");
            System.Diagnostics.Debug.WriteLine(e.ToString());
            _beaconManager.StopRangingBeaconsInRegion(_fieldRegion);
        }

        public void Dispose()
        {
            _uploadTimer.Stop();
        }
    }
}

