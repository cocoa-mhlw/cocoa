using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltBeaconOrg.BoundBeacon;
using AltBeaconOrg.BoundBeacon.Startup;
using AltBeaconOrg.BoundBeacon.Powersave;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Covid19Radar.Common;

namespace Covid19Radar.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
        [Application(Debuggable = false)]
#endif
    public class MainApplication : Application//, IBootstrapNotifier, IBeaconConsumer
    {

        private const string TAG = "Covid19Radar";

        BeaconManager _beaconManager;

        private RegionBootstrap regionBootstrap;
        private Region _backgroundRegion;
        private BackgroundPowerSaver backgroundPowerSaver;
        private bool haveDetectedBeaconsSinceBoot = false;

        private MainActivity mainActivity = null;

        public MainActivity MainActivity
        {
            get { return mainActivity; }
            set { mainActivity = value; }
        }

        public MainApplication() : base() { }
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }
        /*
        public override void OnCreate()
        {
            base.OnCreate();

            _beaconManager = BeaconManager.GetInstanceForApplication(this);
            var iBeaconParser = new BeaconParser();
            //	Estimote > 2013
            iBeaconParser.SetBeaconLayout(Const.IBEACON_FORMAT);
            _beaconManager.BeaconParsers.Add(iBeaconParser);

            Log.Debug(TAG, "setting up background monitoring for beacons and power saving");
            // wake up the app when a beacon is seen
            _backgroundRegion = new Region("backgroundRegion", null, null, null);
            regionBootstrap = new RegionBootstrap(this, _backgroundRegion);

            // simply constructing this class and holding a reference to it in your custom Application
            // class will automatically cause the BeaconLibrary to save battery whenever the application
            // is not visible.  This reduces bluetooth power usage by about 60%
            backgroundPowerSaver = new BackgroundPowerSaver(this);
        }

        public void DidDetermineStateForRegion(int state, Region region)
        {
        }

        public void DidEnterRegion(Region region)
        {
            // In this example, this class sends a notification to the user whenever a Beacon
            // matching a Region (defined above) are first seen.
            Log.Debug(TAG, "did enter region.");
            if (!haveDetectedBeaconsSinceBoot)
            {
                Log.Debug(TAG, "auto launching MonitoringActivity");

                // The very first time since boot that we detect an beacon, we launch the
                // MainActivity
                var intent = new Intent(this, typeof(MainActivity));
                intent.SetFlags(ActivityFlags.NewTask);
                // Important:  make sure to add android:launchMode="singleInstance" in the manifest
                // to keep multiple copies of this activity from getting created if the user has
                // already manually launched the app.
                this.StartActivity(intent);
                haveDetectedBeaconsSinceBoot = true;
            }
            else
            {
                if (mainActivity != null)
                {
                    Log.Debug(TAG, "I see a beacon again");
                }
                else
                {
                    // If we have already seen beacons before, but the monitoring activity is not in
                    // the foreground, we send a notification to the user on subsequent detections.
                    Log.Debug(TAG, "Sending notification.");
                    SendNotification();
                }
            }
        }

        public void DidExitRegion(Region region)
        {
            Log.Debug(TAG, "did exit region.");
        }
        private void SendNotification()
        {
            var builder =
                new NotificationCompat.Builder(this)
                    .SetContentTitle("AltBeacon Reference Application")
                    .SetContentText("A beacon is nearby.")
                    .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo);

            var stackBuilder = Android.App.TaskStackBuilder.Create(this);
            stackBuilder.AddNextIntent(new Intent(this, typeof(MainActivity)));
            var resultPendingIntent =
                stackBuilder.GetPendingIntent(
                    0,
                    PendingIntentFlags.UpdateCurrent
                );
            builder.SetContentIntent(resultPendingIntent);
            var notificationManager =
                (NotificationManager)this.GetSystemService(Context.NotificationService);
            notificationManager.Notify(1, builder.Build());
        }

        public void OnBeaconServiceConnect()
        {
            throw new NotImplementedException();
        }
        */
    }
}