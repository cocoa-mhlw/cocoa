using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism;
using Prism.Ioc;
using Android.Runtime;
using Android.Content;
using Plugin.CurrentActivity;
using Covid19Radar.Model;
using AltBeaconOrg.BoundBeacon;
using System.Collections.Generic;
using System.Linq;
using System;
using Covid19Radar.Common;
using Covid19Radar.Droid.Services;
using Covid19Radar.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using SQLite;
using AltBeaconOrg.BoundBeacon.Startup;
using Region = AltBeaconOrg.BoundBeacon.Region;
using Acr.UserDialogs;

namespace Covid19Radar.Droid
{
    [Activity(Label = "Covid19Radar", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme.Splash", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IBeaconConsumer, Android.App.Application.IActivityLifecycleCallbacks
    {
        //public static MainActivity Instance { get; private set; }
        public static object dataLock = new object();

        private Region _rangingRegion;
        private RangeNotifier _rangeNotifier;
        private BeaconManager _beaconManager;
        private SQLiteConnection _connection;
        private BeaconTransmitter _beaconTransmitter;

        protected override void OnCreate(Bundle bundle)
        {
            base.SetTheme(Resource.Style.MainTheme);
            base.OnCreate(bundle);

            CrossCurrentActivity.Current.Init(this, bundle);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            //Instance = this;

            Xamarin.Essentials.Platform.Init(this, bundle);
            global::Rg.Plugins.Popup.Popup.Init(this, bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            global::Xamarin.Forms.FormsMaterial.Init(this, bundle);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            global::FFImageLoading.ImageService.Instance.Initialize(new FFImageLoading.Config.Configuration()
            {
                Logger = new DebugLogger()
            });

            UserDialogs.Init(this);

            LoadApplication(new App(new AndroidInitializer()));
            CreateNotificationFromIntent(Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            CreateNotificationFromIntent(intent);
        }

        void CreateNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.Extras.GetString(Services.NotificationService.TitleKey);
                string message = intent.Extras.GetString(Services.NotificationService.MessageKey);
                DependencyService.Get<NotificationService>().ReceiveNotification(title, message);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        public class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                containerRegistry.RegisterSingleton<ISQLiteConnectionProvider, SQLiteConnectionProvider>();
                containerRegistry.RegisterSingleton<UserDataService, UserDataService>();
                containerRegistry.RegisterSingleton<INotificationService, NotificationService>();
                //containerRegistry.RegisterSingleton<IBeaconService, BeaconService>();

            }
        }

        #region AltBeacon
        private async void DidRangeBeaconsInRegionComplete(object sender, ICollection<Beacon> beacons)
        {
            await Task.Run(() =>
            {
                System.Diagnostics.Debug.WriteLine("DidRangeBeaconsInRegionComplete");
                var now = DateTime.UtcNow;
                var keyTime = now.ToString("yyyyMMddHH");
                System.Diagnostics.Debug.WriteLine(Utils.SerializeToJson(beacons));
                var foundBeacons = beacons.ToList();

                if (foundBeacons != null && foundBeacons.Count > 0)
                {
                    foreach (Beacon beacon in foundBeacons)
                    {
                        var key = $"{beacon.Id1}{beacon.Id2}{beacon.Id3}.{keyTime}";
                        lock (dataLock)
                        {
                            var result = _connection.Table<BeaconDataModel>().SingleOrDefault(x => x.Id == key);
                            if (result == null)
                            {
                                // New
                                BeaconDataModel data = new BeaconDataModel();
                                data.Id = key;
                                data.Count = 0;
                                data.UserBeaconUuid = AppConstants.iBeaconAppUuid;
                                data.BeaconUuid = beacon.Id1.ToString();
                                data.Major = beacon.Id2.ToString();
                                data.Minor = beacon.Id3.ToString();
                                data.Distance = beacon.Distance;
                                data.MinDistance = beacon.Distance;
                                data.MaxDistance = beacon.Distance;
                                data.Rssi = beacon.Rssi;
                                //                       data.TXPower = beacon.TxPower;
                                data.ElaspedTime = new TimeSpan();
                                data.LastDetectTime = now;
                                data.FirstDetectTime = now;
                                data.KeyTime = keyTime;
                                data.IsSentToServer = false;
                                _connection.Insert(data);

                            }
                            else
                            {
                                // Update
                                BeaconDataModel data = result;
                                data.Id = key;
                                data.Count++;
                                data.UserBeaconUuid = AppConstants.iBeaconAppUuid;
                                data.BeaconUuid = beacon.Id1.ToString();
                                data.Major = beacon.Id2.ToString();
                                data.Minor = beacon.Id3.ToString();
                                data.Distance += (beacon.Distance - data.Distance) / data.Count;
                                data.MinDistance = (beacon.Distance < data.MinDistance ? beacon.Distance : data.MinDistance);
                                data.MaxDistance = (beacon.Distance > data.MaxDistance ? beacon.Distance : data.MaxDistance);
                                data.Rssi = beacon.Rssi;
                                //                        data.TXPower = beacon.TxPower;
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
            });
        }

        public async void StartRagingBeacons()
        {
            await Task.Run(() =>
            {
                _beaconManager = BeaconManager.GetInstanceForApplication(this);
                _rangeNotifier = new RangeNotifier();

                _connection = DependencyService.Get<ISQLiteConnectionProvider>().GetConnection();
                _connection.CreateTable<BeaconDataModel>();

                //iBeacon
                _beaconManager.BeaconParsers.Add(new BeaconParser().SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24"));
                _beaconManager.Bind(this);

                _rangeNotifier.DidRangeBeaconsInRegionComplete += DidRangeBeaconsInRegionComplete;
                _beaconManager.AddRangeNotifier(_rangeNotifier);

                _rangingRegion = new Region(AppConstants.AppName, Identifier.Parse(AppConstants.iBeaconAppUuid), null, null);

                try
                {
                    _beaconManager.StartRangingBeaconsInRegion(_rangingRegion);
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine("StartRangingException: " + ex.Message);
                }
            });
        }

        public async void StopRagingBeacons()
        {
            await Task.Run(() =>
            {
                try
                {
                    // 複数OK
                    _beaconManager.StopRangingBeaconsInRegion(_rangingRegion);
                    _beaconManager.RemoveRangeNotifier(_rangeNotifier);
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine("StopRangingException: " + ex.Message);
                }
                _beaconManager.Unbind(this);
            });
        }

        public async void StartAdvertisingBeacons(UserDataModel userData)
        {
            await Task.Run(() =>
            {
                Beacon beacon = new Beacon.Builder()
                            .SetId1(AppConstants.iBeaconAppUuid)
                            .SetId2(userData.Major)
                            .SetId3(userData.Minor)
                            .SetTxPower(-59)
                            .SetManufacturer(AppConstants.CompanyCodeApple)
                            .Build();

                BeaconParser beaconParser = new BeaconParser().SetBeaconLayout(AppConstants.iBeaconFormat);

                _beaconTransmitter = new BeaconTransmitter(this, beaconParser);
                _beaconTransmitter.StartAdvertising(beacon);
            });
        }

        public async void StopAdvertisingBeacons()
        {
            await Task.Run(() =>
            {
                _beaconTransmitter.StopAdvertising();
            });
        }

        public List<BeaconDataModel> GetBeaconData()
        {
            return _connection.Table<BeaconDataModel>().ToList();
        }


        #endregion

        #region IBeaconConsumer implementation

        public void OnBeaconServiceConnect()
        {
            RequestPermission();

            _beaconManager.SetForegroundScanPeriod(5000);
            _beaconManager.SetForegroundBetweenScanPeriod(100);
            _beaconManager.SetBackgroundScanPeriod(500);
            _beaconManager.SetBackgroundBetweenScanPeriod(30000);

            _beaconManager.UpdateScanPeriods();

        }

        #endregion

        private void RequestPermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                string[] permissions = new string[] {
                    Android.Manifest.Permission.Bluetooth,
                    Android.Manifest.Permission.BluetoothAdmin,
                    Android.Manifest.Permission.AccessCoarseLocation,
                    Android.Manifest.Permission.AccessFineLocation
                };

                RequestPermissions(permissions, 0);
            }
        }

        #region IActivityLifecycleCallbacks

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
            if (_beaconManager.IsBound(this))
                _beaconManager.SetBackgroundMode(true);
        }

        public void OnActivityResumed(Activity activity)
        {
            if (_beaconManager.IsBound(this))
                _beaconManager.SetBackgroundMode(false);
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
        }

        public void OnActivityStopped(Activity activity)
        {
        }
        #endregion

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}

