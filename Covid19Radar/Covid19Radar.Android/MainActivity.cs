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

        private const int BEACONS_UPDATES_IN_SECONDS = 5;
        private const long BEACONS_UPDATES_IN_MILLISECONDS = BEACONS_UPDATES_IN_SECONDS * 1000;

        private Region _fieldRegion;

        private RangeNotifier _rangeNotifier;
        private MonitorNotifier _monitorNotifier;
        private List<BeaconModel> _listOfBeacons;
        private BeaconManager _beaconManager;

        public void StartBeacon()

        {
            _beaconManager = BeaconManager.GetInstanceForApplication(this);
            _monitorNotifier = new MonitorNotifier();
            _rangeNotifier = new RangeNotifier();
            //_listOfBeacons = beacons;



            //iBeacon
            BeaconParser beaconParser = new BeaconParser().SetBeaconLayout(AppConstants.IBEACON_FORMAT);
            _beaconManager.BeaconParsers.Add(beaconParser);
            _beaconManager.Bind(this);

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
            ICollection<Beacon> beacons = rangeEventArgs.Beacons;
            if (beacons != null && beacons.Count > 0)
            {
                var foundBeacons = beacons.ToList();
                foreach (Beacon beacon in beacons)
                {
                    /*
                    Mvx.Resolve<IMvxMessenger>().Publish<BeaconFoundMessage>(
                        new BeaconFoundMessage(
                            this,
                            beacon.Id1.ToString(),
                            (ushort)Convert.ToInt32(beacon.Id2.ToString()),
                            (ushort)Convert.ToInt32(beacon.Id3.ToString()))
                    );
                    */
//                    _listOfBeacons.Add(beacon);
                    Log.Debug("Covid19RadarBeacon", "DidRangeBeaconsInRegionComplete");
                    Log.Debug("Covid19RadarBeacon", beacon.ToString());
                }
            }

        }

        private void DetermineStateForRegionComplete(object sender, MonitorEventArgs e)
        {
            Log.Debug("Covid19RadarBeacon", "DetermineStateForRegionComplete");
            Log.Debug("Covid19RadarBeacon", e.Region.ToString());
        }

        private void EnterRegionComplete(object sender, MonitorEventArgs e)
        {
            Log.Debug("Covid19RadarBeacon", "EnterRegionComplete ---- StartRanging");

            MainActivity activity = Xamarin.Forms.Forms.Context as MainActivity;
            _beaconManager = BeaconManager.GetInstanceForApplication(activity);
            _beaconManager.StartRangingBeaconsInRegion(_fieldRegion);
        }

        private void ExitRegionComplete(object sender, MonitorEventArgs e)
        {
            Log.Debug("Covid19RadarBeacon", "ExitRegionComplete ---- StopRanging");
            MainActivity activity = Xamarin.Forms.Forms.Context as MainActivity;
            _beaconManager = BeaconManager.GetInstanceForApplication(activity);
            _beaconManager.StopRangingBeaconsInRegion(_fieldRegion);
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

