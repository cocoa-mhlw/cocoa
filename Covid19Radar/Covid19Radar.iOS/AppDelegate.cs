using CoreLocation;
using Covid19Radar.Common;
using Covid19Radar.iOS.Services;
using Covid19Radar.Services;
using Foundation;
using Prism;
using Prism.Ioc;
using System;
using UIKit;

namespace Covid19Radar.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        #region Fields
        public static AppDelegate Instance;
        //private readonly CLLocationManager _locationMgr;
        //private CLBeaconRegion _fieldRegion;
        #endregion

        public AppDelegate()
        {
            //_locationMgr = new CLLocationManager();

        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App(new iOSInitializer()));

           // StartBeacon();

            return base.FinishedLaunching(app, options);
        }

        /*
        public void StartBeacon()
        {

            _locationMgr.RequestAlwaysAuthorization();

            // Authorization
            _locationMgr.AuthorizationChanged += DidAuthorizationChanged;

            // Monitoring
            _locationMgr.RegionEntered += DidRegionEntered;
            _locationMgr.RegionLeft += DidRegionLeft;

            // Ranging
            _locationMgr.DidRangeBeacons += DidRangeBeacons;
            _locationMgr.DidDetermineState += DidDetermineState;
            _locationMgr.PausesLocationUpdatesAutomatically = false;

            _locationMgr.StartUpdatingLocation();
            _locationMgr.RequestAlwaysAuthorization();

            _fieldRegion = new CLBeaconRegion(new NSUuid(AppConstants.AppUUID), "");
            _fieldRegion.NotifyOnEntry = true;
            _fieldRegion.NotifyOnExit = true;
            _fieldRegion.NotifyEntryStateOnDisplay = true;
            _locationMgr.StartMonitoring(_fieldRegion);

            System.Diagnostics.Debug.WriteLine("Start");

        }
        private void DidAuthorizationChanged(object sender, CLAuthorizationChangedEventArgs e)
        {
            if (e.Status == CLAuthorizationStatus.AuthorizedAlways)
            {

            }
            else
            {
                _locationMgr.StopRangingBeacons(_fieldRegion);
                _locationMgr.StopMonitoring(_fieldRegion);
            }
            System.Diagnostics.Debug.WriteLine("DidAuthorizationChanged");
        }

        private void DidRegionLeft(object sender, CLRegionEventArgs e)
        {
            _locationMgr.StopRangingBeacons(_fieldRegion);
        }

        private void DidRegionEntered(object sender, CLRegionEventArgs e)
        {
            _locationMgr.StartRangingBeacons(_fieldRegion);
        }

        public void StopBeacon()
        {
            _locationMgr.StopRangingBeacons(_fieldRegion);
            _locationMgr.StopMonitoring(_fieldRegion);
            _locationMgr.StopUpdatingLocation();
        }

        private void DidDetermineState(object sender, CLRegionStateDeterminedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DidDetermineState");
            System.Diagnostics.Debug.WriteLine(e.ToString());
        }

        private void DidRangeBeacons(object sender, CLRegionBeaconsRangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DidRangeBeacons");
            // Beacon Get
            foreach (var beacon in e.Beacons)
            {
                System.Diagnostics.Debug.WriteLine(beacon.ToString());

                if (beacon.Proximity == CLProximity.Unknown)
                {
                    return;
                }

                string uuid = beacon.ProximityUuid.AsString();
                var major = (ushort)beacon.Major;
                var minor = (ushort)beacon.Minor;
            }
        }
        */

    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
                BeaconService beaconService = new BeaconService();
                containerRegistry.RegisterSingleton<IBeaconService, BeaconService>();
        }
    }

}
