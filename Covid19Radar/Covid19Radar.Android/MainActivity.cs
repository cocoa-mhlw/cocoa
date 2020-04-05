using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism;
using Prism.Ioc;
using Android.Runtime;
using Android.Content;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using Covid19Radar.Model;
using AltBeaconOrg.BoundBeacon;
using System.Collections.Generic;
using System.Linq;
using Android.Util;
using System;
using Covid19Radar.Common;
using System.Threading;
using Covid19Radar.Droid.Services;

namespace Covid19Radar.Droid
{
    [Activity(Label = "Covid19Radar", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IBeaconConsumer
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            CrossCurrentActivity.Current.Init(this, bundle);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Xamarin.Essentials.Platform.Init(this, bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App(new AndroidInitializer()));

            StartBeacon();
            StartAdvertising();

        }
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        public class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
            }
        }

        #region Beacon

        private const int BEACONS_UPDATES_IN_SECONDS = 1;
        private const long BEACONS_UPDATES_IN_MILLISECONDS = BEACONS_UPDATES_IN_SECONDS * 1000;

        private Region _fieldRegion;

        private RangeNotifier _rangeNotifier;
        private MonitorNotifier _monitorNotifier;
        private Dictionary<string, BeaconDataModel> _dictionaryOfBeaconData;
        private BeaconManager _beaconManager;

        private BeaconTransmitter _beaconTransmitter;

        public void StartBeacon()

        {
            _beaconManager = BeaconManager.GetInstanceForApplication(this);
            _monitorNotifier = new MonitorNotifier();
            _rangeNotifier = new RangeNotifier();
            _dictionaryOfBeaconData = new Dictionary<string, BeaconDataModel>();

            //iBeacon
            BeaconParser beaconParser = new BeaconParser().SetBeaconLayout(AppConstants.IBEACON_FORMAT);
            _beaconManager.BeaconParsers.Add(beaconParser);
            _beaconManager.Bind(this);

        }

        public void StartAdvertising()
        {
            Beacon beacon = new Beacon.Builder()
                                .SetId1(AppConstants.AppUUID)
                                .SetId2("2111")
                                .SetId3("3123")
                                .SetTxPower(-59)
                                .SetManufacturer(AppConstants.COMPANY_CODE_APPLE)
                                .Build();

            // iBeaconのバイト列フォーマットをBeaconParser（アドバタイズ時のバイト列定義）にセットする。
            BeaconParser beaconParser = new BeaconParser().SetBeaconLayout(AppConstants.IBEACON_FORMAT);

            // iBeaconの発信を開始する。
            _beaconTransmitter = new BeaconTransmitter(Application.Context, beaconParser);
            _beaconTransmitter.StartAdvertising(beacon);

        }

        public void StopAdvertising()
        {
            _beaconTransmitter.StopAdvertising();

        }

        public void StopBeacon()
        {
            _beaconManager.StopMonitoringBeaconsInRegion(_fieldRegion);
            _beaconManager.StopRangingBeaconsInRegion(_fieldRegion);
            _beaconManager.Unbind(this);
        }


        #endregion

        #region Event Handlers
        private void DidRangeBeaconsInRegionComplete(object sender, RangeEventArgs rangeEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("DidRangeBeaconsInRegionComplete");

            ICollection<Beacon> beacons = rangeEventArgs.Beacons;
            if (beacons != null && beacons.Count > 0)
            {
                ProcessBeaconData(beacons);
            }
        }

        private void ProcessBeaconData(ICollection<Beacon> beacons)
        {
            var foundBeacons = beacons.ToList();
            foreach (Beacon beacon in beacons)
            {
                var key = beacon.Id1.ToString() + beacon.Id2.ToString() + beacon.Id3.ToString();
                BeaconDataModel data = new BeaconDataModel();
                if (_dictionaryOfBeaconData.ContainsKey(key))
                {
                    data = _dictionaryOfBeaconData.GetValueOrDefault(key);
                    data.UUID = beacon.Id1.ToString();
                    data.Major = beacon.Id2.ToString();
                    data.Minor = beacon.Id3.ToString();
                    data.Distance = beacon.Distance;
                    data.Rssi = beacon.Rssi;
                    data.TXPower = beacon.TxPower;
                    _dictionaryOfBeaconData.Remove(key);
                    data.ElaspedTime += DateTime.Now - data.LastDetectTime;
                    data.LastDetectTime = DateTime.Now;
                }
                else
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
                _dictionaryOfBeaconData.Add(key, data);

                System.Diagnostics.Debug.WriteLine(key.ToString());
                System.Diagnostics.Debug.WriteLine(data.Distance);
                System.Diagnostics.Debug.WriteLine(data.ElaspedTime.TotalSeconds);

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

            MainActivity activity = Xamarin.Forms.Forms.Context as MainActivity;
            _beaconManager = BeaconManager.GetInstanceForApplication(activity);
            //_beaconManager.StartRangingBeaconsInRegion(_fieldRegion);
        }

        private void ExitRegionComplete(object sender, MonitorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ExitRegionComplete ---- StopRanging");
            System.Diagnostics.Debug.WriteLine(e.ToString());

            MainActivity activity = Xamarin.Forms.Forms.Context as MainActivity;
            _beaconManager = BeaconManager.GetInstanceForApplication(activity);
            //_beaconManager.StopRangingBeaconsInRegion(_fieldRegion);
        }

        #endregion

        public void OnBeaconServiceConnect()
        {
            // BeaconManager Setting
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

            _fieldRegion = new Region(AppConstants.AppUUID, null, null, null);
            _beaconManager.StartMonitoringBeaconsInRegion(_fieldRegion);
            _beaconManager.StartRangingBeaconsInRegion(_fieldRegion);
        }



        #region Class Notifier and EventArgs
        public class RangeNotifier : Java.Lang.Object, IRangeNotifier
        {
            public event EventHandler<RangeEventArgs> DidRangeBeaconsInRegionComplete;

            public void DidRangeBeaconsInRegion(ICollection<Beacon> beacons, Region region)
            {
                DidRangeBeaconsInRegionComplete?.Invoke(this, new RangeEventArgs { Beacons = beacons, Region = region });
            }
        }
        public class RangeEventArgs : EventArgs
        {
            public Region Region { get; set; }
            public ICollection<Beacon> Beacons { get; set; }
        }

        public class MonitorNotifier : Java.Lang.Object, IMonitorNotifier
        {
            public event EventHandler<MonitorEventArgs> DetermineStateForRegionComplete;
            public event EventHandler<MonitorEventArgs> EnterRegionComplete;
            public event EventHandler<MonitorEventArgs> ExitRegionComplete;

            public void DidDetermineStateForRegion(int state, Region region)
            {
                OnDetermineStateForRegionComplete(state, region);
            }

            public void DidEnterRegion(Region region)
            {
                OnEnterRegionComplete(region);
            }

            public void DidExitRegion(Region region)
            {
                OnExitRegionComplete(region);
            }

            private void OnDetermineStateForRegionComplete(int state, Region region)
            {
                DetermineStateForRegionComplete?.Invoke(this, new MonitorEventArgs { State = state, Region = region });
            }

            private void OnEnterRegionComplete(Region region)
            {
                EnterRegionComplete?.Invoke(this, new MonitorEventArgs { Region = region });
            }

            private void OnExitRegionComplete(Region region)
            {
                ExitRegionComplete?.Invoke(this, new MonitorEventArgs { Region = region });
            }

        }
        public class MonitorEventArgs : EventArgs
        {
            public Region Region { get; set; }
            public int State { get; set; }
        }
        #endregion
    }
}

